using StartinhsMart.CoreService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace StartinhsMart.CoreService.Permissions;

public class CoreServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CoreServicePermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(CoreServicePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreServiceResource>(name);
    }
}
