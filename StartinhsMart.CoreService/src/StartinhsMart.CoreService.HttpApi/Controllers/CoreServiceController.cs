using StartinhsMart.CoreService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace StartinhsMart.CoreService.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CoreServiceController : AbpControllerBase
{
    protected CoreServiceController()
    {
        LocalizationResource = typeof(CoreServiceResource);
    }
}
