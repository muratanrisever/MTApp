using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;
using Microsoft.AspNetCore.Authorization; // Yetkilendirme için

namespace MTApp.Controllers
{
    [Authorize] // Sadece yetkili kullanıcıların erişmesini sağlar
    public class LeavesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeavesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Leaves
        // Tüm izinleri listeler. Personel bilgilerini de dahil eder.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Leaves.Include(l => l.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Leaves/Details/5
        // Belirli bir iznin detaylarını gösterir.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // GET: Leaves/Create
        // Yeni izin oluşturma formunu gösterir.
        public IActionResult Create()
        {
            // Personel dropdown listesi için verileri ViewBag'e atarız.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName"); // Sadece Adı gösterelim
            return View();
        }

        // POST: Leaves/Create
        // Yeni izin oluşturma formundan gelen verileri işler ve kaydeder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,LeaveType,StartDate,EndDate,NumberOfDays,Status,Description,RequestDate")] Leave leave)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Doğrulama başarısız ise, formdaki verileri tekrar doldurmak için ViewBag'i tekrar atarız.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", leave.EmployeeId);
            return View(leave);
        }

        // GET: Leaves/Edit/5
        // Mevcut bir izni düzenleme formunu gösterir.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }
            // Düzenleme formundaki dropdown listesi için verileri ViewBag'e atarız.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", leave.EmployeeId);
            return View(leave);
        }

        // POST: Leaves/Edit/5
        // Düzenleme formundan gelen verileri işler ve günceller.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,LeaveType,StartDate,EndDate,NumberOfDays,Status,Description,RequestDate")] Leave leave)
        {
            if (id != leave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leave);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveExists(leave.Id))
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
            // Doğrulama başarısız ise, formdaki verileri tekrar doldurmak için ViewBag'i tekrar atarız.
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "FirstName", leave.EmployeeId);
            return View(leave);
        }

        // GET: Leaves/Delete/5
        // Bir izni silme onay sayfasını gösterir.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // POST: Leaves/Delete/5
        // İzni silme işlemini gerçekleştirir.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Bir iznin veritabanında var olup olmadığını kontrol eder.
        private bool LeaveExists(int id)
        {
            return _context.Leaves.Any(e => e.Id == id);
        }
    }
}