using Nblity.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Nblity.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class NblityController : AbpControllerBase
{
    protected NblityController()
    {
        LocalizationResource = typeof(NblityResource);
    }
}
