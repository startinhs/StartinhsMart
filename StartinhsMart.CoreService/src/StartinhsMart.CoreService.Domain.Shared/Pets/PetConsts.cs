namespace StartinhsMart.CoreService.Pets
{
    public static class PetConsts
    {
        private const string DefaultSorting = "{0}ImageId asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Pet." : string.Empty);
        }

    }
}