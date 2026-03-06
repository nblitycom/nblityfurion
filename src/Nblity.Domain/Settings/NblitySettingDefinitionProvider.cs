using Volo.Abp.Settings;

namespace Nblity.Settings;

public class NblitySettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(NblitySettings.MySetting1));
    }
}
