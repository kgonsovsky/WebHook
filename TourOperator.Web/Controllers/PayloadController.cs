using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;

namespace TourOperator.Web.Controllers;

public class PayloadController : Controller
{
    private readonly ApplicationDbContext _context;

    public PayloadController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Payload
    public async Task<IActionResult> Index()
    {
        return View(await _context.WebhookPayloads.OrderByDescending(a => a.Created).ToListAsync());
    }

    // GET: Payload/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var payload = await _context.WebhookPayloads.FindAsync(id);
        _context.WebhookPayloads.Remove(payload);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}