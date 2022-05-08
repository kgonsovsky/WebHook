namespace TourOperator.Model;

public class WebhookEvent : BaseEntity
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public WebhookEvent()
    {

    }       
}