using System.ComponentModel.DataAnnotations;

namespace AspNetWebhookPublisher.Enums
{
    public enum WebHookEvents
    {
        [Display(Name = "person.created")]
        PersonCreated,
        [Display(Name = "person.updated")]
        PersonUpdated,
        [Display(Name = "person.deleted")]
        PersonDeleted
    }
}
