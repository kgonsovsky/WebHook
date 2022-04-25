using AspNetWebhookSubscriber.BindingModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AspNetWebhookSubscriber.Controllers
{ 
    public class TestController : ControllerBase
    {
        protected const string SignatureHeaderName = "webhook-signature";

        protected const string Secret = "secret";     
        [HttpPost("webhook-form-data-test")]

        public IActionResult WebhookFormDataTest([FromForm] WebhookBindingModel webhookBindingModel)
        {           
            if (!CheckMessageAuthenticationCode("secret", GetHashJson(webhookBindingModel)))
            {
                throw new Exception("Unexpected Signature");
            }
            Console.WriteLine(webhookBindingModel.Data);
            Console.WriteLine("");
            Console.WriteLine(webhookBindingModel.Event);
            return Ok();
        }

        [HttpPost("webhook-json-data-test")]
        public IActionResult WebhookJsonDataTest([FromBody] WebhookBindingModel webhookBindingModel)
        {
            if (!CheckMessageAuthenticationCode("secret", GetHashJson(webhookBindingModel)))
            {
                throw new Exception("Unexpected Signature");
            }
            Console.WriteLine(webhookBindingModel.Data);
            Console.WriteLine("");
            Console.WriteLine(webhookBindingModel.Event);
            return Ok();
        }

        private string GetHashJson(WebhookBindingModel webhookBindingModel)
        {

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var data = JsonSerializer.Serialize(webhookBindingModel, options);
            return data;
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
}
