using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;

namespace TourOperator.Web.Controllers;

public class ResponseController : Controller
{
    private readonly ApplicationDbContext _context;

    public ResponseController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Response
    public async Task<IActionResult> Index()
    {
        return View(await _context.WebhookResponses.OrderByDescending(a => a.Created).ToListAsync());
    }

    // GET: Response/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _context.WebhookResponses.FindAsync(id);
        _context.WebhookResponses.Remove(response);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}