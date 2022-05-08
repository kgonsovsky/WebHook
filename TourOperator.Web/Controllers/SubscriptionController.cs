using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;
using TourOperator.Model;

namespace TourOperator.Web.Controllers;

public class SubscriptionController : Controller
{
    private readonly ApplicationDbContext _context;

    public SubscriptionController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Subscription
    public async Task<IActionResult> Index()
    {
        return View(await _context.WebhookSubscriptions.OrderByDescending(a=> a.Created).ToListAsync());
    }


    // GET: Subscription/Create
    public IActionResult AddOrEdit(Guid? id)
    {
        if (id == null)
            return View(new WebhookSubscription());
        else
            return View(_context.WebhookSubscriptions.Find(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrEdit([Bind("Id,DisplayName,PayloadUrl,Secret,IsActive")] WebhookSubscription subscription)
    {
        if (ModelState.IsValid)
        {
            if (subscription.Id == null)
                _context.Add(subscription);
            else
                _context.Update(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(subscription);
    }


    // GET: Subscription/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var subscription =await _context.WebhookSubscriptions.FindAsync(id);
        _context.WebhookSubscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


}