using Nblity.Localization;
using Volo.Abp.Application.Services;

namespace Nblity;

/* Inherit your application services from this class.
 */
public abstract class NblityAppService : ApplicationService
{
    protected NblityAppService()
    {
        LocalizationResource = typeof(NblityResource);
    }
}
