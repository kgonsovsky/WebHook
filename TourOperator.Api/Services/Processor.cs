using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TourOperator.Db;
using TourOperator.Model;

namespace TourOperator.Api.Services;

public class Processor: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Processor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private static readonly ConcurrentQueue<WebhookPayload> _queue = new ConcurrentQueue<WebhookPayload>();

    private static readonly ConcurrentQueue<WebhookPayload> _queueNow = new ConcurrentQueue<WebhookPayload>();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Factory.StartNew
        (
            () => ReQueue(stoppingToken), stoppingToken);

        _ = Task.Factory.StartNew
        (
            () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    while (_queue.TryDequeue(out var payload))
                    {
                        _queueNow.Enqueue(payload);
                        _ = Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                using var scope = _scopeFactory.CreateScope();
                                var publisher = scope.ServiceProvider.GetRequiredService<Publisher>();
                                await publisher.Publish(payload, stoppingToken);
                                _queueNow.TryDequeue(out payload);
                            }
                            catch (Exception e)
                            {
                                _queueNow.TryDequeue(out payload);
                            }

                        }, stoppingToken);
                    }
                }
            });

        return Task.CompletedTask;
    }

    public static void Enqueue(WebhookPayload payload)
    {
        Logger.Log("::Enqueue WebHook", payload, payload.WebhookSubscription);
        _queue.Enqueue(payload);
    }


    private  async void ReQueue(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000, stoppingToken);
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var subscriptions = db.WebhookSubscriptions.Where(q => q.IsActive).ToList();
                var events = db.WebhookEvents.ToList();

                var payloads = db.WebhookPayloads.Where(a => a.Handled == false)
                    .OrderByDescending(a => a.Created).Take(100).ToList()
                    .Where(a => _queueNow.Any(b=> b.Id==a.Id) ==false && _queue.Any(b => b.Id == a.Id) == false);
                foreach (var payload in payloads)
                {
                    payload.WebhookSubscription = subscriptions.First(a => a.Id == payload.WebhookSubscriptionId);
                    payload.WebhookEvent = events.First(a => a.Id == payload.WebhookEventId);
                    Enqueue(payload);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}