using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using TourOperator.Db;
using TourOperator.Model.Entities;
using TourOperator.Model.Enums;
using TourOperator.Model.Interfaces;

namespace TourOperator.Api.Services
{    
    public class WebhookPublisher : IWebhookPublisher
    {
        protected const string SignatureHeaderKey = "sha256";

        protected const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";

        protected const string SignatureHeaderName = "webhook-signature";

        private readonly ApplicationDbContext _db;

        private readonly HttpClient _httpClient;

        public WebhookPublisher(HttpClient httpClient, ApplicationDbContext db)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _db = db;
        }

        public static AsyncRetryPolicy GetRetryPolicy()
        {
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                    onRetry: (response, delay, retryCount, context) =>
                    {
                        System.Diagnostics.Debug.WriteLine("RETRYING... - " + retryCount);
                    });
            return retryPolicy;
        }

        public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy()
        {
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
            System.Diagnostics.Debug.WriteLine("STARTING...");
            return circuitBreakerPolicy;
        }

        public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public async Task Publish<T>(WebHookEnum whEnum, T data)
        {
            var retryPolicy = GetRetryPolicy();
            var circuitBreakerPolicy = GetCircuitBreakerPolicy();
            var eventName = whEnum.GetDisplayName();
            var dataJson = JsonSerializer.Serialize(data, JsonOptions);
            var hashJson = JsonSerializer.Serialize(new { Data = dataJson, Event = eventName }, JsonOptions);

            var subscriptions = _db.WebhookSubscriptions
                .Where(q => q.IsActive).ToList();

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var entity = _db.WebhookEvents.SingleOrDefault(q => q.Name == eventName);
                    if (entity == null)
                    {
                        continue;
                    }

                    PublishSubsciption(subscription, entity, circuitBreakerPolicy, retryPolicy, dataJson, hashJson);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private async void PublishSubsciption(WebhookSubscription subscription, WebhookEvent entity,
            AsyncCircuitBreakerPolicy circuitBreakerPolicy, AsyncRetryPolicy retryPolicy, string dataJson, string hashJson)
        {
            var paylodEntity = new WebhookPayload()
            {
                WebhookEventId = (Guid)entity.Id,
                Data = dataJson,
                Attempt = 0,
                Created = DateTime.Now
            };

            _db.WebhookPayloads.Add(paylodEntity);
            _db.SaveChanges();

            HttpContent httpContent = null;
            httpContent = new StringContent(hashJson, Encoding.UTF8, "application/json");

            var secretBytes = Encoding.UTF8.GetBytes(subscription.Secret);

            using (var hasher = new HMACSHA256(secretBytes))
            {
                var hashData = Encoding.UTF8.GetBytes(hashJson);
                var sha256 = hasher.ComputeHash(hashData);
                var headerValue = string.Format(CultureInfo.InvariantCulture, SignatureHeaderValueTemplate,
                    BitConverter.ToString(sha256));
                httpContent.Headers.Add(SignatureHeaderName, headerValue);
            }


            await retryPolicy.WrapAsync(circuitBreakerPolicy).ExecuteAsync(async () =>
            {
                var response = await _httpClient.PostAsync(subscription.PayloadUrl, httpContent);
                _db.WebhookResponses.Add(new WebhookResponse()
                {
                    Data = await response.Content.ReadAsStringAsync(), HttpStatusCode = (int) response.StatusCode,
                    WebhookPayloadId = (Guid)paylodEntity.Id
                });
                _db.SaveChanges();
                if (!response.IsSuccessStatusCode)
                {
                    var payload = _db.WebhookPayloads.Where(q => q.Id == paylodEntity.Id).SingleOrDefault();
                    if (paylodEntity != null)
                    {
                        payload.Attempt += 1;
                        _db.WebhookPayloads.Update(payload);
                        _db.SaveChanges();
                    }
                }

                response.EnsureSuccessStatusCode();
            });
        }
    }
}
