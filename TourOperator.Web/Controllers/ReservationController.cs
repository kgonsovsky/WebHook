using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourOperator.Db;
using TourOperator.Model;

namespace TourOperator.Web.Controllers;

public class ReservationController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReservationController(ApplicationDbContext context)
    {
        _context = context;
    }


    // GET: Reservation
    public async Task<IActionResult> Index()
    {
        return View(await _context.Reservations.OrderByDescending(a => a.Created).ToListAsync());
    }


    // GET: Reservation/Create
    public IActionResult AddOrEdit(Guid? id)
    {
        if (id == null)
        {
            var reservation = new Faker<Reservation>()
                .RuleFor(u => u.Guest, (f, u) => f.Name.FullName())
                .RuleFor(u => u.Agency, (f, u) => f.Company.CompanyName())
                .RuleFor(u => u.Created, (f, u) => DateTime.Now)
                .RuleFor(u => u.CheckIn, (f, u) => f.Date.Future(1))
                .RuleFor(u => u.CheckOut, (f, u) =>  f.Date.Future(1))
                .Generate();
            return View(reservation);
        }
              
        else
            return View(_context.Reservations.Find(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrEdit([Bind("Id,CheckIn,CheckOut,Guest,Agency")] Reservation reservation)
    {
        if (ModelState.IsValid)
        {
            if (reservation.Id == null)
                _context.Add(reservation);
            else
                _context.Update(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(reservation);
    }


    // GET: Reservation/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}