namespace BookKeeping.Domain.Models
{
    public enum PaymentState
    {
        Initialized = 0,
        Authorized = 1,
        Captured = 2,
        Cancelled = 3,
        Refunded = 4,
        PendingExternalSystem = 5,
        Error = 200,
    }
}