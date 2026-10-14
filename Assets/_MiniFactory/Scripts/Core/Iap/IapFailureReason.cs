namespace MiniFactory.Core.Iap
{
    public enum IapFailureReason
    {
        Unknown = 0,
        NotInitialized = 1,
        NotAvailable = 2,
        ProductNotFound = 3,
        PurchaseCancelled = 4,
        PaymentDeclined = 5
    }
}
