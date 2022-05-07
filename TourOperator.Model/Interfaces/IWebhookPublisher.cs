using TourOperator.Model.Enums;

namespace TourOperator.Model.Interfaces
{
    public interface IWebhookPublisher
    {
        Task Publish<T>(WebHookEnum whEnum, T data);
    }
}