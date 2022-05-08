using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TourOperator.Api.Services;
using TourOperator.Db;
using TourOperator.Model;

namespace TourOperator.Api.Controllers;

[Route("/")]
[ApiController]
public class GateWayController: ControllerBase
{
    private readonly ApplicationDbContext _db;

    public GateWayController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public Guid Post(Reservation reservation)
    {
        Logger.Log("New reservation",reservation);

        var subscriptions = _db.WebhookSubscriptions.Where(q => q.IsActive).ToList();
        var events = _db.WebhookEvents.ToList();

        reservation.Handled = true;
        _db.Reservations.Add(reservation);
        _db.SaveChanges();

        foreach (var subscription in subscriptions)
        {
            var wEvent = events.FirstOrDefault(q =>
                q.Name == WebHookEnum.ReservationCreated.GetDisplayName());
            if (wEvent == null)
                continue;

            var payload = new WebhookPayload()
            {
                WebhookEvent = wEvent,
                WebhookEventId = wEvent.Id,
                WebhookSubscriptionId = subscription.Id,
                WebhookSubscription = subscription,
                Data = JsonSerializer.Serialize(reservation, JsonOptions),
                Attempt = 0,
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
            };
            _db.WebhookPayloads.Add(payload);
            _db.SaveChanges();
            Processor.Enqueue(payload);
        }
        return reservation.Id;
    }

    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
}