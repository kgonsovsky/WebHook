using System;
using AspNetWebhookPublisher.Common;

namespace AspNetWebhookPublisher.Entities
{
    public class WebhookSubscriptionAllowedEvent : BaseEntity
    {
        public Guid WebhookSubscriptionId { get; set; }
        public WebhookSubscription WebhookSubscription { get; set; }
        public Guid WebhookEventId { get; set; }
        public WebhookEvent WebhookEvent { get; set; }
    }
}
