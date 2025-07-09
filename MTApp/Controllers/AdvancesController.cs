using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;
using Microsoft.AspNetCore.Authorization; // Authorization for access control

namespace MTApp.Controllers
{
    [Authorize] // Ensures only authorized users can access this controller
    public class AdvancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Advances
        // Lists all advances, including employee information.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Advances.Include(a => a.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Advances/Details/5
        // Displays details for a specific advance.
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

        // GET: Advances/Create
        // Displays the form for creating a new advance request.
        public IActionResult Create()
        {
            // Populate dropdown list for employees.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName"); // Display first name
            return View();
        }

        // POST: Advances/Create
        // Processes data from the new advance request form and saves it.
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
            // If validation fails, repopulate the dropdown list.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        // GET: Advances/Edit/5
        // Displays the form for editing an existing advance request.
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
            // Populate dropdown list for employees.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        // POST: Advances/Edit/5
        // Processes data from the edit form and updates the advance request.
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
            // If validation fails, repopulate the dropdown list.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", advance.EmployeeId);
            return View(advance);
        }

        // GET: Advances/Delete/5
        // Displays the confirmation page for deleting an advance request.
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

        // POST: Advances/Delete/5
        // Performs the deletion of the advance request.
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

        // Checks if an advance request exists in the database.
        private bool AdvanceExists(int id)
        {
            return _context.Advances.Any(e => e.Id == id);
        }
    }
}