using System;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TourOperator.Interfaces;

namespace TourOperator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebhookPublisher _webhookPublisher;
        private readonly ApplicationDbContext _applicationDbContext;
        public IndexModel(ILogger<IndexModel> logger, IWebhookPublisher webhookPublisher, ApplicationDbContext applicationDbContext)
        {
            _webhookPublisher = webhookPublisher;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task<IActionResult>  OnGetAsync()
        {
            var relation = new Faker<Entities.Reservation>()
                .RuleFor(u => u.Guest, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Agency, (f, u) => f.Company.CompanyName())
                .RuleFor(u => u.CheckIn, (f, u) => f.Date.Future(1))
                .RuleFor(u => u.CheckOut, (f, u) => f.Date.Future(2))
                .RuleFor(u => u.MealPlan, (f, u) => f.Commerce.ProductMaterial())
                .RuleFor(u => u.Id, (f, u) => Guid.NewGuid())
                 .RuleFor(u => u.Created, (f, u) => DateTime.Now)
                .Generate();
            _applicationDbContext.Persons.Add(relation);
            _applicationDbContext.SaveChanges();
            await _webhookPublisher.Publish(Enums.WebHookEvents.PersonCreated, relation);
            return Page();
        }
    }
}
