using Microsoft.Extensions.Localization;
using Nblity.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Nblity.Blazor;

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
