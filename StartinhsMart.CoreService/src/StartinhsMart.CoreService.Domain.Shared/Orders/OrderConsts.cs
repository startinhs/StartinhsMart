namespace StartinhsMart.CoreService.Orders
{
    public static class OrderConsts
    {
        public const int MaxOrderNumberLength = 50;
        public const int MaxShippingAddressLength = 500;
        public const int MaxShippingPhoneLength = 20;
        public const int MaxCustomerNameLength = 256;
        public const int MaxCustomerEmailLength = 256;
        public const int MaxNoteLength = 1000;
        public const int MaxCancellationReasonLength = 500;

        public static string GetDefaultSorting(bool withEntityName = false)
        {
            return withEntityName
                ? "Order.CreationTime DESC"
                : "CreationTime DESC";
        }
    }
}
