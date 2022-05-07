using TourOperator.Model.Common;

namespace TourOperator.Model.Entities
{
    public class WebhookPayload : BaseEntity
    {
        public Guid WebhookEventId { get; set; }
        public WebhookEvent WebhookEvent { get; set; }
        public int Attempt { get; set; }
        public string Data { get; set; }
    }
}
