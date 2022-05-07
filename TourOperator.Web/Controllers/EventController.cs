using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;

namespace TourOperator.Web.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Event
        public async Task<IActionResult> Index()
        {
            return View(await _context.WebhookEvents.OrderByDescending(a => a.Created).ToListAsync());
        }
    }
}
