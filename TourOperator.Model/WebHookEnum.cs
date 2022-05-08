using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TourOperator.Model;

public enum WebHookEnum
{
    [Display(Name = "reservation.created")]
    ReservationCreated,
    [Display(Name = "reservation.updated")]
    ReservationUpdated,
    [Display(Name = "reservation.canceled")]
    ReservationCanceled
}

public static class WebHookEventHelper
{
    public static string GetDisplayName(this WebHookEnum webHookEnum)
    {
        var displayAttribute = typeof(WebHookEnum).GetMember(webHookEnum.ToString())[0]
            .GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Name))
        {
            return displayAttribute.Name;
        }

        return null;

    }
}