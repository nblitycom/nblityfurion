using Nblity.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Nblity.Blazor.Client;

public abstract class NblityComponentBase : AbpComponentBase
{
    protected NblityComponentBase()
    {
        LocalizationResource = typeof(NblityResource);
    }
}
