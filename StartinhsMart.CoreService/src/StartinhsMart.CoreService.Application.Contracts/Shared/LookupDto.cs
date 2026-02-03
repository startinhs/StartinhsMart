namespace StartinhsMart.CoreService.Shared
{
    public class LookupDto<TKey>
    {
        public required TKey Id { get; set; }

        public string DisplayName { get; set; } = null!;
    }
}