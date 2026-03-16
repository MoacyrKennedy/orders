namespace Sales.Orders.Domain.Common;

public sealed class Notification
{
    public string Key { get; }
    public string Message { get; }
    public bool IsError { get; }

    public Notification(string key, string message, bool isError = true)
    {
        Key = key;
        Message = message;
        IsError = isError;
    }
}