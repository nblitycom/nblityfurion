using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using MudBlazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Identity.Localization;
using Volo.Abp.PermissionManagement.Blazor.Components;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Data;

namespace Volo.Abp.Identity.Blazor.Pages.Identity;

public partial class RoleManagement
{
    protected const string PermissionProviderName = "R";

    protected PermissionManagementModal PermissionManagementModal;

    protected string ManagePermissionsPolicyName;

    protected bool HasManagePermissionsPermission { get; set; }

    protected PageToolbar Toolbar { get; } = new();

    protected List<TableColumn> RoleManagementTableColumns => TableColumns.Get<RoleManagement>();

    // MudBlazor state
    private bool _createDialogVisible;
    private bool _editDialogVisible;
    private MudForm _createForm;
    private MudForm _editForm;
    private MudTable<IdentityRoleDto> _table;

    public RoleManagement()
    {
        ObjectMapperContext = typeof(AbpIdentityBlazorModule);
        LocalizationResource = typeof(IdentityResource);

        CreatePolicyName = IdentityPermissions.Roles.Create;
        UpdatePolicyName = IdentityPermissions.Roles.Update;
        DeletePolicyName = IdentityPermissions.Roles.Delete;
        ManagePermissionsPolicyName = IdentityPermissions.Roles.ManagePermissions;
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(LUiNavigation["Menu:Administration"].Value));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Menu:IdentityManagement"].Value));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Roles"].Value));
        return base.SetBreadcrumbItemsAsync();
    }

    private async Task<TableData<IdentityRoleDto>> LoadServerData(TableState state, CancellationToken ct)
    {
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

        return new TableData<IdentityRoleDto>
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

    protected override async Task OpenCreateModalAsync()
    {
        // base.OpenCreateModalAsync initializes NewEntity and has null-safe modal handling
        await base.OpenCreateModalAsync();
        _createDialogVisible = true;
    }

    protected override async Task OpenEditModalAsync(IdentityRoleDto entity)
    {
        // base.OpenEditModalAsync loads the entity and has null-safe modal handling
        await base.OpenEditModalAsync(entity);
        _editDialogVisible = true;
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
        NewEntity = new IdentityRoleCreateDto();
        return Task.CompletedTask;
    }

    protected override Task CloseEditModalAsync()
    {
        _editDialogVisible = false;
        return Task.CompletedTask;
    }

    private async Task OpenPermissionsModalAsync(IdentityRoleDto entity)
    {
        await PermissionManagementModal.OpenAsync(PermissionProviderName, entity.Name);
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<RoleManagement>()
            .AddRange(new EntityAction[]
            {
                    new EntityAction
                    {
                        Text = L["Edit"],
                        Visible = (data) => HasUpdatePermission,
                        Clicked = async (data) => { await OpenEditModalAsync(data.As<IdentityRoleDto>()); }
                    },
                    new EntityAction
                    {
                        Text = L["Permissions"],
                        Visible = (data) => HasManagePermissionsPermission,
                        Clicked = async (data) =>
                        {
                            await PermissionManagementModal.OpenAsync(PermissionProviderName,
                                data.As<IdentityRoleDto>().Name);
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => HasDeletePermission && !data.As<IdentityRoleDto>().IsStatic,
                        Clicked = async (data) => await DeleteEntityAsync(data.As<IdentityRoleDto>()),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<IdentityRoleDto>())
                    }
            });

        return base.SetEntityActionsAsync();
    }

    protected override async ValueTask SetTableColumnsAsync()
    {
        RoleManagementTableColumns
            .AddRange(new TableColumn[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<RoleManagement>(),
                    },
                    new TableColumn
                    {
                        Title = L["RoleName"],
                        Sortable = true,
                        Data = nameof(IdentityRoleDto.Name),
                        Component = typeof(RoleNameComponent)
                    },
            });

        RoleManagementTableColumns.AddRange(await GetExtensionTableColumnsAsync(IdentityModuleExtensionConsts.ModuleName,
            IdentityModuleExtensionConsts.EntityNames.Role));

        await base.SetTableColumnsAsync();
    }

    protected override async Task SetPermissionsAsync()
    {
        await base.SetPermissionsAsync();

        HasManagePermissionsPermission =
            await AuthorizationService.IsGrantedAsync(IdentityPermissions.Roles.ManagePermissions);
    }

    protected override string GetDeleteConfirmationMessage(IdentityRoleDto entity)
    {
        return string.Format(L["RoleDeletionConfirmationMessage"], entity.Name);
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewRole"],
            OpenCreateModalAsync,
            IconName.Add,
            requiredPolicyName: CreatePolicyName);

        return base.SetToolbarItemsAsync();
    }
}
