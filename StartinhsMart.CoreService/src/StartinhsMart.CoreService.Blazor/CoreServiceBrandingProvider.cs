using Microsoft.Extensions.Localization;
using StartinhsMart.CoreService.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace StartinhsMart.CoreService.Blazor;

[Dependency(ReplaceServices = true)]
public class CoreServiceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CoreServiceResource> _localizer;

    public CoreServiceBrandingProvider(IStringLocalizer<CoreServiceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
