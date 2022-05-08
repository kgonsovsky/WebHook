using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using TourOperator.Api.Controllers;
using TourOperator.Db;
using TourOperator.Model;

namespace TourOperator.Api.Services;

public class Publisher
{
    protected const string SignatureHeaderKey = "sha256";

    protected const string SignatureHeaderValueTemplate = SignatureHeaderKey + "={0}";

    protected const string SignatureHeaderName = "webhook-signature";
        
    private readonly HttpClient _httpClient;

    private readonly ApplicationDbContext _db;

    public Publisher(HttpClient httpClient, ApplicationDbContext db)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _db = db;
    }

    public static AsyncRetryPolicy GetRetryPolicy()
    {
        var maxRetryAttempts = 3;
        var pauseBetweenFailures = TimeSpan.FromSeconds(5);
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                onRetry: (response, delay, retryCount, context) =>
                {
                    Console.WriteLine("RETRYING... - " + retryCount);
                });
        return retryPolicy;
    }

    public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy()
    {
        var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        Console.WriteLine("STARTING...");
        return circuitBreakerPolicy;
    }

    public async Task Publish(WebhookPayload payload, CancellationToken stopToken)
    {
        var retryPolicy = GetRetryPolicy();
        var circuitBreakerPolicy = GetCircuitBreakerPolicy();

        var hashJson = JsonSerializer.Serialize(new
        {
            Data = payload.Data,
            Event = payload.WebhookEvent.Name,
        }, GateWayController.JsonOptions);
        var secretBytes = Encoding.UTF8.GetBytes(payload.WebhookSubscription.Secret);

        var httpContent = new StringContent(hashJson, Encoding.UTF8, "application/json");

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
            HttpResponseMessage response=null;
            try
            {
                response = await _httpClient.PostAsync(payload.WebhookSubscription.PayloadUrl, httpContent, stopToken);
                _db.WebhookResponses.Add(new WebhookResponse()
                {
                    Data = await response.Content.ReadAsStringAsync(stopToken),
                    HttpStatusCode = (int)response.StatusCode,
                    WebhookPayloadId = payload.Id,
                    Created = DateTime.Now
                });
                payload.Handled = response.IsSuccessStatusCode;
                Logger.Log("WebHook delivered", payload, payload.WebhookSubscription);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Logger.Log($"Error publishing WebHook, Attempt N: {payload.Attempt+1}, {e.Message}",payload, payload.WebhookSubscription); ;
                throw;
            }
            payload.Attempt += 1;
            _db.WebhookPayloads.Update(payload);
            await _db.SaveChangesAsync(stopToken);
               

        });

    }
}