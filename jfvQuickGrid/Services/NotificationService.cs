namespace jfvQuickGrid.Services;

public sealed class NotificationService
{
    public event Action<string>? OnProductChanged;

    public void NotifyProductChanged(string message)
    {
        OnProductChanged?.Invoke(message);
    }
}
