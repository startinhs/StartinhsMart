using Volo.Abp.Settings;

namespace StartinhsMart.CoreService.Settings;

public class CoreServiceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(CoreServiceSettings.MySetting1));
    }
}
