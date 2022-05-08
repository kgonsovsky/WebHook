using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TravelAgency.Web.BindingModels;

namespace TravelAgency.Web.Controllers;

public class SubScriberController : ControllerBase
{
    protected const string SignatureHeaderName = "webhook-signature";

    protected const string Secret = "secret";     

    [HttpPost("webhook")]
    public IActionResult WebhookDataArrived([FromBody] WebhookBindingModel model)
    {
        if (!CheckMessageAuthenticationCode("secret", GetHashJson(model)))
        {
            throw new Exception("Unexpected Signature");
        }
        Console.WriteLine(model.Event);
        Console.WriteLine(model.Data);
        return Ok();
    }

    private string GetHashJson(WebhookBindingModel model)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        return JsonSerializer.Serialize(model, options);
    }

    private bool CheckMessageAuthenticationCode(string secret, string hashJson)
    {
        if (!HttpContext.Request.Headers.ContainsKey(SignatureHeaderName))
        {
            return false;
        }

        var receivedSignature = HttpContext.Request.Headers[SignatureHeaderName].ToString().Split("=");

        string computedSignature;
        switch (receivedSignature[0])
        {
            case "sha256":
                var secretBytes = Encoding.UTF8.GetBytes(secret);
                using (var hasher = new HMACSHA256(secretBytes))
                {
                    var data = Encoding.UTF8.GetBytes(hashJson);
                    computedSignature = BitConverter.ToString(hasher.ComputeHash(data));
                }
                break;
            default:
                throw new NotImplementedException();
        }
        return computedSignature == receivedSignature[1];
    }
}