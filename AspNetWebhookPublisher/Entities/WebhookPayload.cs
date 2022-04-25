using System;
using AspNetWebhookPublisher.Common;

namespace AspNetWebhookPublisher.Entities
{
    public class WebhookPayload : BaseEntity
    {
        public Guid WebhookEventId { get; set; }
        public WebhookEvent WebhookEvent { get; set; }
        public int Attempt { get; set; }
        public string Data { get; set; }
    }
}
