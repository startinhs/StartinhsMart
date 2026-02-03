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

        var petPermission = myGroup.AddPermission(CoreServicePermissions.Pets.Default, L("Permission:Pets"));
        petPermission.AddChild(CoreServicePermissions.Pets.Create, L("Permission:Create"));
        petPermission.AddChild(CoreServicePermissions.Pets.Edit, L("Permission:Edit"));
        petPermission.AddChild(CoreServicePermissions.Pets.Delete, L("Permission:Delete"));

        var categoryPermission = myGroup.AddPermission(CoreServicePermissions.Categories.Default, L("Permission:Categories"));
        categoryPermission.AddChild(CoreServicePermissions.Categories.Create, L("Permission:Create"));
        categoryPermission.AddChild(CoreServicePermissions.Categories.Edit, L("Permission:Edit"));
        categoryPermission.AddChild(CoreServicePermissions.Categories.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CoreServiceResource>(name);
    }
}