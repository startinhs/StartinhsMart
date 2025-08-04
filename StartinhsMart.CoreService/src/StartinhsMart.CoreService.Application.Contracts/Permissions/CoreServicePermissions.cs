namespace StartinhsMart.CoreService.Permissions;

public static class CoreServicePermissions
{
    public const string GroupName = "CoreService";


    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Pets
    {
        public const string Default = GroupName + ".Pets";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}