using System;

namespace TourOperator.Model;

public class WebhookResponse : BaseEntity
{       
    public Guid WebhookPayloadId { get; set; }
    public WebhookPayload WebhookPayload { get; set; }
    public string Data { get; set; }
    public int? HttpStatusCode { get; set; }       
}