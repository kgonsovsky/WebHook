namespace TourOperator.Model;

public class WebhookSubscription : BaseEntity
{
    public string DisplayName { get; set; }
    public string PayloadUrl { get; set; }
    public string Secret { get; set; }
    public bool IsActive { get; set; }
}