using System;
using TourOperator.Model;

namespace TourOperator.Api.Services;

public class Logger
{
    public static void Log(string message,WebhookPayload payload, WebhookSubscription subscription)
    {
        Console.WriteLine($"{message} {payload.Id} for subscriber {subscription.PayloadUrl}.");
    }

    public static void Log(string message, Reservation reservation)
    {
        Console.WriteLine($"{message} {reservation.Guest}, checkIn: {reservation.CheckIn}");
    }
}