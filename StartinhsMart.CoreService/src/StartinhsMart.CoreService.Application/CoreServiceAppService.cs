using StartinhsMart.CoreService.Localization;
using Volo.Abp.Application.Services;

namespace StartinhsMart.CoreService;

/* Inherit your application services from this class.
 */
public abstract class CoreServiceAppService : ApplicationService
{
    protected CoreServiceAppService()
    {
        LocalizationResource = typeof(CoreServiceResource);
    }
}
