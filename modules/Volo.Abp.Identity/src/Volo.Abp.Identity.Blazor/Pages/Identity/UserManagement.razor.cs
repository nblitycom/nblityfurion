using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement.Blazor.Components;
using Volo.Abp.Users;

namespace Volo.Abp.Identity.Blazor.Pages.Identity;

public partial class UserManagement
{
    protected const string PermissionProviderName = "U";

    protected const string DefaultSelectedTab = "UserInformations";

    protected PermissionManagementModal PermissionManagementModal;

    protected IReadOnlyList<IdentityRoleDto> Roles;

    protected AssignedRoleViewModel[] NewUserRoles;

    protected AssignedRoleViewModel[] EditUserRoles;

    protected string ManagePermissionsPolicyName;

    protected bool HasManagePermissionsPermission { get; set; }

    protected string CreateModalSelectedTab = DefaultSelectedTab;

    protected string EditModalSelectedTab = DefaultSelectedTab;

    public bool IsEditCurrentUser { get; set; }

    protected PageToolbar Toolbar { get; } = new();

    private List<TableColumn> UserManagementTableColumns => TableColumns.Get<UserManagement>();

    // MudBlazor state
    private bool _createDialogVisible;
    private bool _editDialogVisible;
    private bool _showPassword;
    private string _searchText;
    private MudForm _createForm;
    private MudForm _editForm;
    private MudTable<IdentityUserDto> _table;

    [Inject]
    protected IPermissionChecker PermissionChecker { get; set; }

    public UserManagement()
    {
        ObjectMapperContext = typeof(AbpIdentityBlazorModule);
        LocalizationResource = typeof(IdentityResource);

        CreatePolicyName = IdentityPermissions.Users.Create;
        UpdatePolicyName = IdentityPermissions.Users.Update;
        DeletePolicyName = IdentityPermissions.Users.Delete;
        ManagePermissionsPolicyName = IdentityPermissions.Users.ManagePermissions;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {
            Roles = (await AppService.GetAssignableRolesAsync()).Items;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(LUiNavigation["Menu:Administration"].Value));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Menu:IdentityManagement"].Value));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Users"].Value));
        return base.SetBreadcrumbItemsAsync();
    }

    private async Task<TableData<IdentityUserDto>> LoadServerData(TableState state, CancellationToken ct)
    {
        GetListInput.Filter = _searchText;

        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            CurrentSorting = state.SortDirection == MudBlazor.SortDirection.Descending
                ? $"{state.SortLabel} DESC"
                : state.SortLabel;
        }
        else
        {
            CurrentSorting = string.Empty;
        }

        CurrentPage = state.Page + 1;

        if (GetListInput is ISortedResultRequest sorted)
        {
            sorted.Sorting = CurrentSorting;
        }

        if (GetListInput is IPagedResultRequest paged)
        {
            paged.SkipCount = state.Page * state.PageSize;
        }

        if (GetListInput is ILimitedResultRequest limited)
        {
            limited.MaxResultCount = state.PageSize;
        }

        var result = await AppService.GetListAsync(GetListInput);

        return new TableData<IdentityUserDto>
        {
            Items = result.Items,
            TotalItems = (int)result.TotalCount
        };
    }

    protected override async Task GetEntitiesAsync()
    {
        if (_table != null)
        {
            await _table.ReloadServerData();
        }
        else
        {
            await base.GetEntitiesAsync();
        }
    }

    private async Task OnSearchKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await GetEntitiesAsync();
        }
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManagePermissionsPermission =
            await AuthorizationService.IsGrantedAsync(IdentityPermissions.Users.ManagePermissions);
    }

    protected override async Task OpenCreateModalAsync()
    {
        CreateModalSelectedTab = DefaultSelectedTab;

        NewUserRoles = Roles.Select(x => new AssignedRoleViewModel
        {
            Name = x.Name,
            IsAssigned = x.IsDefault
        }).ToArray();

        _showPassword = false;

        // base.OpenCreateModalAsync initializes NewEntity and has null-safe modal handling
        await base.OpenCreateModalAsync();

        NewEntity.IsActive = true;
        NewEntity.LockoutEnabled = true;
        _createDialogVisible = true;
    }

    protected override Task OnCreatingEntityAsync()
    {
        // apply roles before saving
        NewEntity.RoleNames = NewUserRoles.Where(x => x.IsAssigned).Select(x => x.Name).ToArray();

        return base.OnCreatingEntityAsync();
    }

    protected override async Task OnCreatedEntityAsync()
    {
        _createDialogVisible = false;
        if (_table != null)
        {
            await _table.ReloadServerData();
        }
        await Notify.Success(GetCreateMessage());
    }

    protected override async Task OpenEditModalAsync(IdentityUserDto entity)
    {
        try
        {
            EditModalSelectedTab = DefaultSelectedTab;
            IsEditCurrentUser = entity.Id == CurrentUser.Id;

            if (await PermissionChecker.IsGrantedAsync(IdentityPermissions.Users.ManageRoles))
            {
                var userRoleIds = (await AppService.GetRolesAsync(entity.Id)).Items.Select(r => r.Id).ToList();

                EditUserRoles = Roles.Select(x => new AssignedRoleViewModel
                {
                    Name = x.Name,
                    IsAssigned = userRoleIds.Contains(x.Id)
                }).ToArray();
            }

            _showPassword = false;

            // base.OpenEditModalAsync loads the entity and has null-safe modal handling
            await base.OpenEditModalAsync(entity);
            _editDialogVisible = true;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected override Task OnUpdatingEntityAsync()
    {
        // apply roles before saving
        if (EditUserRoles != null)
        {
            EditingEntity.RoleNames = EditUserRoles.Where(x => x.IsAssigned).Select(x => x.Name).ToArray();
        }
        return base.OnUpdatingEntityAsync();
    }

    protected override async Task OnUpdatedEntityAsync()
    {
        _editDialogVisible = false;
        if (_table != null)
        {
            await _table.ReloadServerData();
        }
        await Notify.Success(GetUpdateMessage());
    }

    protected override Task CloseCreateModalAsync()
    {
        _createDialogVisible = false;
        NewEntity = new IdentityUserCreateDto();
        return Task.CompletedTask;
    }

    protected override Task CloseEditModalAsync()
    {
        _editDialogVisible = false;
        return Task.CompletedTask;
    }

    protected override string GetDeleteConfirmationMessage(IdentityUserDto entity)
    {
        return string.Format(L["UserDeletionConfirmationMessage"], entity.UserName);
    }

    private async Task OpenPermissionsModalAsync(IdentityUserDto entity)
    {
        await PermissionManagementModal.OpenAsync(PermissionProviderName,
            entity.Id.ToString(),
            entity.UserName);
    }

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<UserManagement>()
            .AddRange(new EntityAction[]
            {
                    new EntityAction
                    {
                        Text = L["Edit"],
                        Visible = (data) => HasUpdatePermission,
                        Clicked = async (data) => await OpenEditModalAsync(data.As<IdentityUserDto>())
                    },
                    new EntityAction
                    {
                        Text = L["Permissions"],
                        Visible = (data) => HasManagePermissionsPermission,
                        Clicked = async (data) =>
                        {
                            await PermissionManagementModal.OpenAsync(PermissionProviderName,
                                data.As<IdentityUserDto>().Id.ToString(),
                                data.As<IdentityUserDto>().UserName);
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => HasDeletePermission && CurrentUser.GetId() != data.As<IdentityUserDto>().Id,
                        Clicked = async (data) => await DeleteEntityAsync(data.As<IdentityUserDto>()),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<IdentityUserDto>())
                    }
            });

        return base.SetEntityActionsAsync();
    }

    protected override async ValueTask SetTableColumnsAsync()
    {
        UserManagementTableColumns
            .AddRange(new TableColumn[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<UserManagement>(),
                    },
                    new TableColumn
                    {
                        Title = L["UserName"],
                        Data = nameof(IdentityUserDto.UserName),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["EmailAddress"],
                        Data = nameof(IdentityUserDto.Email),
                        Sortable = true,
                    },
                    new TableColumn
                    {
                        Title = L["PhoneNumber"],
                        Data = nameof(IdentityUserDto.PhoneNumber),
                        Sortable = true,
                    }
            });

        UserManagementTableColumns.AddRange(await GetExtensionTableColumnsAsync(IdentityModuleExtensionConsts.ModuleName,
            IdentityModuleExtensionConsts.EntityNames.User));
        await base.SetTableColumnsAsync();
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewUser"], OpenCreateModalAsync,
            IconName.Add,
            requiredPolicyName: CreatePolicyName);

        return base.SetToolbarItemsAsync();
    }
}

public class AssignedRoleViewModel
{
    public string Name { get; set; }

    public bool IsAssigned { get; set; }
}
