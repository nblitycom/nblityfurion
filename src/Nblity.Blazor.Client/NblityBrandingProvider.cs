using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nblity.Localization;

namespace Nblity.Blazor.Client;

[Dependency(ReplaceServices = true)]
public class NblityBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<NblityResource> _localizer;

    public NblityBrandingProvider(IStringLocalizer<NblityResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
