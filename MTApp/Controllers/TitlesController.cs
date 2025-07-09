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
    public class TitlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TitlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Titles
        // Tüm unvanları listeler
        public async Task<IActionResult> Index()
        {
            return View(await _context.Titles.ToListAsync());
        }

        // GET: Titles/Details/5
        // Belirli bir unvanın detaylarını gösterir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // GET: Titles/Create
        // Yeni unvan oluşturma formunu gösterir
        public IActionResult Create()
        {
            return View();
        }

        // POST: Titles/Create
        // Yeni unvan oluşturma formundan gelen verileri işler ve kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Title title)
        {
            if (ModelState.IsValid)
            {
                _context.Add(title);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(title);
        }

        // GET: Titles/Edit/5
        // Mevcut bir unvanı düzenleme formunu gösterir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles.FindAsync(id);
            if (title == null)
            {
                return NotFound();
            }
            return View(title);
        }

        // POST: Titles/Edit/5
        // Düzenleme formundan gelen verileri işler ve günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Title title)
        {
            if (id != title.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(title);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TitleExists(title.Id))
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
            return View(title);
        }

        // GET: Titles/Delete/5
        // Bir unvanı silme onay sayfasını gösterir
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var title = await _context.Titles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (title == null)
            {
                return NotFound();
            }

            return View(title);
        }

        // POST: Titles/Delete/5
        // Unvanı silme işlemini gerçekleştirir
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var title = await _context.Titles.FindAsync(id);
            if (title != null)
            {
                _context.Titles.Remove(title);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Bir unvanın veritabanında var olup olmadığını kontrol eder
        private bool TitleExists(int id)
        {
            return _context.Titles.Any(e => e.Id == id);
        }
    }
}