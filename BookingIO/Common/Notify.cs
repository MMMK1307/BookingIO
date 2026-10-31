using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace BookingIO.Common;


public enum NotifyType
{
    Success,
    Warning,
    Error
}

public record NotifyMessage(string Title, string Body, NotifyType NType)
{
    public string ToJsonString()
    {
       return JsonSerializer.Serialize(this);
    }
}; 

public static class Notify
{
    public const string MessageKey = "Message";

    public static void AddNotification(ITempDataDictionary data, NotifyType NType, string title, string body = "")
    {
        List<NotifyMessage> notifications;
        data.TryGetValue(MessageKey, out var value);

        if (value is null)
        {
            notifications = new();
        } else
        {
            string jsonValue = (string)value;
            notifications = JsonSerializer.Deserialize<List<NotifyMessage>>(jsonValue) ?? new();
        }

        NotifyMessage notifyMsg =  new(title, body, NType);
        notifications.Add(notifyMsg);

        data[MessageKey] = JsonSerializer.Serialize(notifications);
    }

    public static string SerializeMessages(List<NotifyMessage> messages)
    {
        return JsonSerializer.Serialize(messages);
    }  
}
