using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;

namespace MTApp.Controllers
{
    [Authorize]
    public class AdvancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Advances.Include(a => a.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advance = await _context.Advances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advance == null)
            {
                return NotFound();
            }

            return View(advance);
        }

        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,Amount,RequestDate,Status,Description")] Advance advance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(advance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advance = await _context.Advances.FindAsync(id);
            if (advance == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,Amount,RequestDate,Status,Description")] Advance advance)
        {
            if (id != advance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvanceExists(advance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advance = await _context.Advances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (advance == null)
            {
                return NotFound();
            }

            return View(advance);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advance = await _context.Advances.FindAsync(id);
            if (advance != null)
            {
                _context.Advances.Remove(advance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvanceExists(int id)
        {
            return _context.Advances.Any(e => e.Id == id);
        }
    }
}