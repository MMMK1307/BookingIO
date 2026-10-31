using BookingIO.Common;
using Microsoft.AspNetCore.Mvc;

namespace BookingIO.Controllers;

public class BaseController : Controller
{
    protected void NotifySuccess(string title, string body = "")
    {
        Notify.AddNotification(TempData, NotifyType.Success, title, body);
    }
    protected void NotifyError(string title, string body = "")
    {
        Notify.AddNotification(TempData, NotifyType.Error, title, body);
    }
    protected void NotifyWarning(string title, string body = "")
    {
        Notify.AddNotification(TempData, NotifyType.Warning, title, body);
    }
}
