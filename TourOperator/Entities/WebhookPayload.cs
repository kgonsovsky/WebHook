using System;
using TourOperator.Common;

namespace TourOperator.Entities
{
    public class WebhookPayload : BaseEntity
    {
        public Guid WebhookEventId { get; set; }
        public WebhookEvent WebhookEvent { get; set; }
        public int Attempt { get; set; }
        public string Data { get; set; }
    }
}
