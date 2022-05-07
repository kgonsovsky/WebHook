using Microsoft.AspNetCore.Mvc;

namespace TravelAgency.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return Ok("Ok");
        }
    }
}
