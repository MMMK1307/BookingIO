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

    public static void AddNotification(ViewDataDictionary data, NotifyType NType, string title, string body = "")
    {
        if (data[MessageKey] is null)
            data[MessageKey] = new List<NotifyMessage>();
        NotifyMessage notifyMsg =  new(title, body, NType);
        ((List<NotifyMessage>)data[MessageKey]!).Add(notifyMsg);
    }

    public static string SerializeMessages(List<NotifyMessage> messages)
    {
        return JsonSerializer.Serialize(messages);
    }  
}
