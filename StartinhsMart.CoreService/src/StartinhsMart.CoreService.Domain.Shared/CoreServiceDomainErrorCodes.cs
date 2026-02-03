namespace StartinhsMart.CoreService;

public static class CoreServiceDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */

    // Category errors
    public const string CategoryParentNotFound = "CoreService:Category:001";
    public const string CategoryCircularReference = "CoreService:Category:002";
    public const string CategoryNotFound = "CoreService:Category:003";

    // Order errors
    public const string OrderNotFound = "CoreService:Order:001";
    public const string OrderItemNotFound = "CoreService:Order:002";
    public const string OrderInvalidStatus = "CoreService:Order:003";

    // Cart errors
    public const string CartNotFound = "CoreService:Cart:001";
    public const string CartItemNotFound = "CoreService:Cart:002";
    public const string InsufficientStock = "CoreService:Cart:003";
}
