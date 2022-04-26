namespace Rano.PlatformServices.Billing
{
    public enum PurchaseServiceState
    {
        NotInitialized,
        Initializing,
        InitializeFailed,
        Updating,
        UpdateFailed,
        Available,
    }
}