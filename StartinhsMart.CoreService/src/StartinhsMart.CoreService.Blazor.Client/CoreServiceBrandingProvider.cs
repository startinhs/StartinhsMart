using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using StartinhsMart.CoreService.Localization;

namespace StartinhsMart.CoreService.Blazor.Client;

public class CoreServiceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CoreServiceResource> _localizer;

    public CoreServiceBrandingProvider(IStringLocalizer<CoreServiceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
