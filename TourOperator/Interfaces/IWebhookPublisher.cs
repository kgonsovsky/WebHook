using System.Threading.Tasks;
using TourOperator.Enums;

namespace TourOperator.Interfaces
{
    public interface IWebhookPublisher
    {
        Task Publish<T>(WebHookEvents webhookEvent, T data);
    }
}