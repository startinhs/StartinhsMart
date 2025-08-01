using StartinhsMart.CoreService.Localization;
using Volo.Abp.AspNetCore.Components;

namespace StartinhsMart.CoreService.Blazor.Client;

public abstract class CoreServiceComponentBase : AbpComponentBase
{
    protected CoreServiceComponentBase()
    {
        LocalizationResource = typeof(CoreServiceResource);
    }
}
