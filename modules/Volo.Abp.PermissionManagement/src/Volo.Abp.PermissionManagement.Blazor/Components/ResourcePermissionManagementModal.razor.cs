using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.PermissionManagement.Localization;

namespace Volo.Abp.PermissionManagement.Blazor.Components;

public partial class ResourcePermissionManagementModal
{
    [Inject] protected IPermissionAppService PermissionAppService { get; set; }

    [Inject] protected IUiMessageService UiMessageService { get; set; }

    private bool _visible;
    private bool _createDialogVisible;
    private bool _editDialogVisible;
    private MudForm _createForm;
    private MudForm _editForm;
    private MudAutocomplete<SearchProviderKeyInfo> _providerKeyAutocomplete;
    private SearchProviderKeyInfo _selectedProviderKeyInfo;

    public bool HasAnyResourcePermission { get; set; }
    public bool HasAnyResourceProviderKeyLookupService { get; set; }
    protected string ResourceName { get; set; }
    protected string ResourceKey { get; set; }
    protected string ResourceDisplayName { get; set; }
    protected int PageSize { get; set; } = 10;

    protected CreateModel CreateEntity { get; set; } = new CreateModel
    {
        Permissions = []
    };

    public GetResourcePermissionDefinitionListResultDto ResourcePermissionDefinitions { get; set; } = new()
    {
        Permissions = []
    };
    protected string CurrentLookupService { get; set; }
    protected string ProviderKey { get; set; }
    protected string ProviderDisplayName { get; set; }
    protected List<ResourceProviderDto> ResourceProviderKeyLookupServices { get; set; } = new();
    protected GetResourcePermissionListResultDto ResourcePermissionList = new()
    {
        Permissions = []
    };

    protected EditModel EditEntity { get; set; } = new EditModel
    {
        Permissions = []
    };

    public ResourcePermissionManagementModal()
    {
        LocalizationResource = typeof(AbpPermissionManagementResource);
    }

    public virtual async Task OpenAsync(string resourceName, string resourceKey, string resourceDisplayName)
    {
        try
        {
            ResourceName = resourceName;
            ResourceKey = resourceKey;
            ResourceDisplayName = resourceDisplayName;

            ResourcePermissionDefinitions = await PermissionAppService.GetResourceDefinitionsAsync(ResourceName);
            ResourceProviderKeyLookupServices = (await PermissionAppService.GetResourceProviderKeyLookupServicesAsync(ResourceName)).Providers;

            HasAnyResourcePermission = ResourcePermissionDefinitions.Permissions.Any();
            if (HasAnyResourcePermission)
            {
                HasAnyResourceProviderKeyLookupService = ResourceProviderKeyLookupServices.Count > 0;
            }

            await InvokeAsync(StateHasChanged);

            ResourcePermissionList = await PermissionAppService.GetResourceAsync(ResourceName, ResourceKey);

            _visible = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    protected virtual Task CloseModal()
    {
        _visible = false;
        return InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OpenCreateModalAsync()
    {
        CurrentLookupService = ResourceProviderKeyLookupServices.FirstOrDefault()?.Name;

        ProviderKey = null;
        ProviderDisplayName = null;
        _selectedProviderKeyInfo = null;

        if (_providerKeyAutocomplete != null)
        {
            await _providerKeyAutocomplete.ClearAsync();
        }

        CreateEntity = new CreateModel
        {
            Permissions = ResourcePermissionDefinitions.Permissions.Select(x => new ResourcePermissionModel
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                IsGranted = false
            }).ToList()
        };

        _createDialogVisible = true;
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnProviderKeySelectedAsync(SearchProviderKeyInfo info)
    {
        _selectedProviderKeyInfo = info;
        if (info == null)
        {
            ProviderKey = null;
            ProviderDisplayName = null;
            return;
        }

        ProviderKey = info.ProviderKey;
        ProviderDisplayName = info.ProviderDisplayName;

        var permissionGrants = await PermissionAppService.GetResourceByProviderAsync(ResourceName, ResourceKey, CurrentLookupService, ProviderKey);
        foreach (var permission in CreateEntity.Permissions)
        {
            permission.IsGranted = permissionGrants.Permissions.Any(p => p.Name == permission.Name && p.Providers.Contains(CurrentLookupService) && p.IsGranted);
        }

        await InvokeAsync(StateHasChanged);
    }

    private async Task<IEnumerable<SearchProviderKeyInfo>> SearchProviderKeysAsync(string value, CancellationToken cancellationToken)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return Array.Empty<SearchProviderKeyInfo>();
        }

        var result = await PermissionAppService.SearchResourceProviderKeyAsync(ResourceName, CurrentLookupService, value, 1);
        return result.Keys;
    }

    protected virtual async Task OnPermissionCheckedChanged(ResourcePermissionModel permission, bool value)
    {
        permission.IsGranted = value;
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task GrantAllAsync(bool value)
    {
        foreach (var permission in CreateEntity.Permissions)
        {
            permission.IsGranted = value;
        }

        foreach (var permission in EditEntity.Permissions)
        {
            permission.IsGranted = value;
        }

        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OpenEditModalAsync(ResourcePermissionGrantInfoDto permission)
    {
        var resourcePermissions = await PermissionAppService.GetResourceByProviderAsync(ResourceName, ResourceKey, permission.ProviderName, permission.ProviderKey);
        EditEntity = new EditModel
        {
            ProviderName = permission.ProviderName,
            ProviderKey = permission.ProviderKey,
            Permissions = resourcePermissions.Permissions.Select(x => new ResourcePermissionModel
            {
                Name = x.Name,
                DisplayName = x.DisplayName,
                IsGranted = x.IsGranted
            }).ToList()
        };

        _editDialogVisible = true;
        await InvokeAsync(StateHasChanged);
    }

    protected virtual Task CloseCreateModalAsync()
    {
        _createDialogVisible = false;
        return InvokeAsync(StateHasChanged);
    }

    protected virtual Task CloseEditModalAsync()
    {
        _editDialogVisible = false;
        return InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnLookupServiceCheckedValueChanged(string value)
    {
        CurrentLookupService = value;
        ProviderKey = null;
        ProviderDisplayName = null;
        _selectedProviderKeyInfo = null;

        if (_providerKeyAutocomplete != null)
        {
            await _providerKeyAutocomplete.ClearAsync();
        }

        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task CreateResourcePermissionAsync()
    {
        await _createForm.Validate();
        if (_createForm.IsValid)
        {
            await PermissionAppService.UpdateResourceAsync(
                ResourceName,
                ResourceKey,
                new UpdateResourcePermissionsDto
                {
                    ProviderName = CurrentLookupService,
                    ProviderKey = ProviderKey,
                    Permissions = CreateEntity.Permissions.Where(p => p.IsGranted).Select(p => p.Name).ToList()
                }
            );

            await CloseCreateModalAsync();
            ResourcePermissionList = await PermissionAppService.GetResourceAsync(ResourceName, ResourceKey);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual async Task UpdateResourcePermissionAsync()
    {
        await _editForm.Validate();
        if (_editForm.IsValid)
        {
            await PermissionAppService.UpdateResourceAsync(
                ResourceName,
                ResourceKey,
                new UpdateResourcePermissionsDto
                {
                    ProviderName = EditEntity.ProviderName,
                    ProviderKey = EditEntity.ProviderKey,
                    Permissions = EditEntity.Permissions.Where(p => p.IsGranted).Select(p => p.Name).ToList()
                }
            );

            await CloseEditModalAsync();
            ResourcePermissionList = await PermissionAppService.GetResourceAsync(ResourceName, ResourceKey);
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual async Task DeleteResourcePermissionAsync(ResourcePermissionGrantInfoDto permission)
    {
        if (await UiMessageService.Confirm(L["ResourcePermissionDeletionConfirmationMessage"]))
        {
            await PermissionAppService.DeleteResourceAsync(
                ResourceName,
                ResourceKey,
                permission.ProviderName,
                permission.ProviderKey
            );

            ResourcePermissionList = await PermissionAppService.GetResourceAsync(ResourceName, ResourceKey);
            await Notify.Success(L["DeletedSuccessfully"]);
            await InvokeAsync(StateHasChanged);
        }
    }

    public class CreateModel
    {
        public List<ResourcePermissionModel> Permissions { get; set; }
    }

    public class EditModel
    {
        public string ProviderName { get; set; }

        public string ProviderKey { get; set; }

        public List<ResourcePermissionModel> Permissions { get; set; }
    }

    public class ResourcePermissionModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsGranted { get; set; }
    }
}
