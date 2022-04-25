using AspNetWebhookPublisher.Common;

namespace AspNetWebhookPublisher.Entities
{
    public class Person : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
