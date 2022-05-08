using System;

namespace TourOperator.Model;

public class WebhookPayload : BaseEntity
{
    public Guid WebhookEventId { get; set; }
    public WebhookEvent WebhookEvent { get; set; }

    public Guid WebhookSubscriptionId { get; set; }
    public WebhookSubscription WebhookSubscription { get; set; }

    public int Attempt { get; set; }
    public string Data { get; set; }
    public bool Handled { get; set; }
}