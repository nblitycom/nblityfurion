using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.FeatureManagement.Blazor.Components;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement.Localization;

namespace Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement;

public partial class TenantManagement
{
    protected const string FeatureProviderName = "T";

    protected bool HasManageFeaturesPermission;
    protected string ManageFeaturesPolicyName;

    protected FeatureManagementModal FeatureManagementModal;

    protected PageToolbar Toolbar { get; } = new();

    protected List<TableColumn> TenantManagementTableColumns => TableColumns.Get<TenantManagement>();

    // MudBlazor state
    private bool _createDialogVisible;
    private bool _editDialogVisible;
    private bool _showPassword;
    private string _searchText;
    private MudForm _createForm;
    private MudForm _editForm;
    private MudTable<TenantDto> _table;

    public TenantManagement()
    {
        LocalizationResource = typeof(AbpTenantManagementResource);
        ObjectMapperContext = typeof(AbpTenantManagementBlazorModule);

        CreatePolicyName = TenantManagementPermissions.Tenants.Create;
        UpdatePolicyName = TenantManagementPermissions.Tenants.Update;
        DeletePolicyName = TenantManagementPermissions.Tenants.Delete;

        ManageFeaturesPolicyName = TenantManagementPermissions.Tenants.ManageFeatures;
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(LUiNavigation["Menu:Administration"]));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Menu:TenantManagement"]));
        BreadcrumbItems.Add(new BlazoriseUI.BreadcrumbItem(L["Tenants"]));
        return base.SetBreadcrumbItemsAsync();
    }

    private async Task<TableData<TenantDto>> LoadServerData(TableState state, CancellationToken ct)
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

        GetListInput.Filter = _searchText;

        var result = await AppService.GetListAsync(GetListInput);

        return new TableData<TenantDto>
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

        HasManageFeaturesPermission = await AuthorizationService.IsGrantedAsync(ManageFeaturesPolicyName);
    }

    protected override async Task OpenCreateModalAsync()
    {
        _showPassword = false;
        await base.OpenCreateModalAsync();
        _createDialogVisible = true;
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

    protected override async Task OpenEditModalAsync(TenantDto entity)
    {
        try
        {
            await base.OpenEditModalAsync(entity);
            _editDialogVisible = true;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
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
        NewEntity = new TenantCreateDto();
        return Task.CompletedTask;
    }

    protected override Task CloseEditModalAsync()
    {
        _editDialogVisible = false;
        return Task.CompletedTask;
    }

    protected override string GetDeleteConfirmationMessage(TenantDto entity)
    {
        return string.Format(L["TenantDeletionConfirmationMessage"], entity.Name);
    }

    private async Task OpenFeaturesModalAsync(TenantDto tenant)
    {
        await FeatureManagementModal.OpenAsync(FeatureProviderName, tenant.Id.ToString(), tenant.Name);
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewTenant"],
            OpenCreateModalAsync,
            IconName.Add,
            requiredPolicyName: CreatePolicyName);

        return base.SetToolbarItemsAsync();
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<TenantManagement>()
            .AddRange(new EntityAction[]
            {
                    new EntityAction
                    {
                        Text = L["Edit"],
                        Visible = (data) => HasUpdatePermission,
                        Clicked = async (data) => { await OpenEditModalAsync(data.As<TenantDto>()); }
                    },
                    new EntityAction
                    {
                        Text = L["Features"],
                        Visible = (data) => HasManageFeaturesPermission,
                        Clicked = async (data) =>
                        {
                            var tenant = data.As<TenantDto>();
                            await FeatureManagementModal.OpenAsync(FeatureProviderName, tenant.Id.ToString(), tenant.Name);
                        }
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => HasDeletePermission,
                        Clicked = async (data) => await DeleteEntityAsync(data.As<TenantDto>()),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<TenantDto>())
                    }
            });

        return base.SetEntityActionsAsync();
    }

    protected override async ValueTask SetTableColumnsAsync()
    {
        TenantManagementTableColumns
            .AddRange(new TableColumn[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<TenantManagement>(),
                    },
                    new TableColumn
                    {
                        Title = L["TenantName"],
                        Sortable = true,
                        Data = nameof(TenantDto.Name),
                    },
            });

        TenantManagementTableColumns.AddRange(await GetExtensionTableColumnsAsync(
            TenantManagementModuleExtensionConsts.ModuleName,
            TenantManagementModuleExtensionConsts.EntityNames.Tenant));

        await base.SetTableColumnsAsync();
    }

    protected virtual void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }
}
