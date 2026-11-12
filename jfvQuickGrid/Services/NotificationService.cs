namespace jfvQuickGrid.Services;

public sealed class NotificationService
{
    public event Action<string>? OnProductUpdated;

    public void NotifyProductUpdated(string message)
    {
        OnProductUpdated?.Invoke(message);
    }
}
