using System;
using System.Collections.Generic;
using AspNetWebhookPublisher.Common;

namespace AspNetWebhookPublisher.Entities
{
    public class WebhookSubscription : BaseEntity
    {
        public Guid WebhookSubscriptionContentTypeId { get; set; }
        public WebhookSubscriptionContentType WebhookSubscriptionContentType { get; set; }
        public Guid WebhookSubscriptionTypeId { get; set; }
        public WebhookSubscriptionType WebhookSubscriptionType { get; set; }       
        public string PayloadUrl { get; set; }
        public string Secret { get; set; }
        public bool IsActive { get; set; }
        public ICollection<WebhookSubscriptionAllowedEvent> WebhookSubscriptionAllowedEvents { get; set; }
    }
}
