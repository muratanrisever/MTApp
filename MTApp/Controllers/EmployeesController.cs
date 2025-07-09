using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;
using Microsoft.AspNetCore.Hosting; // Dosya yükleme için gerekli
using System.IO; // Dosya işlemleri için gerekli

namespace MTApp.Controllers
{
    // Sadece yetkili kullanıcıların bu kontrolcüye erişmesini sağlar.
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = from e in _context.Employees select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.FirstName.Contains(searchString) ||
                                                 s.LastName.Contains(searchString) ||
                                                 s.EmployeeNumber.Contains(searchString) ||
                                                 (s.Department != null && s.Department.Name.Contains(searchString)) ||
                                                 (s.Title != null && s.Title.Name.Contains(searchString)));
            }

            var applicationDbContext = employees
                                               .Include(e => e.Department)
                                               .Include(e => e.Title);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employees/Details/5
        // Belirli bir personelin detaylarını gösterir.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department) // Departman bilgisini yükle
                .Include(e => e.Title)      // Unvan bilgisini yükle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["TitleId"] = new SelectList(_context.Titles, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeNumber,FirstName,LastName,NationalId,DateOfBirth,Gender,MaritalStatus,Nationality,Email,PhoneNumber,Address,HireDate,TerminationDate,DepartmentId,TitleId,Salary,IsActive")] Employee employee, IFormFile? photoFile, IFormFile? resumeFile)
        {
            if (ModelState.IsValid)
            {
                if (photoFile != null && photoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "employees");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(fileStream);
                    }
                    employee.PhotoUrl = "/images/employees/" + uniqueFileName;
                }

                if (resumeFile != null && resumeFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "resumes", "employees");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + resumeFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await resumeFile.CopyToAsync(fileStream);
                    }
                    employee.ResumePath = "/resumes/employees/" + uniqueFileName;
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["TitleId"] = new SelectList(_context.Titles, "Id", "Name", employee.TitleId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["TitleId"] = new SelectList(_context.Titles, "Id", "Name", employee.TitleId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeNumber,FirstName,LastName,NationalId,DateOfBirth,Gender,MaritalStatus,Nationality,Email,PhoneNumber,Address,HireDate,TerminationDate,DepartmentId,TitleId,Salary,PhotoUrl,ResumePath,IsActive")] Employee employee, IFormFile? photoFile, IFormFile? resumeFile)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existingEmployee == null) return NotFound();

                    if (photoFile != null && photoFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingEmployee.PhotoUrl))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, existingEmployee.PhotoUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "employees");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + photoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(fileStream);
                        }
                        employee.PhotoUrl = "/images/employees/" + uniqueFileName;
                    }
                    else
                    {
                        employee.PhotoUrl = existingEmployee.PhotoUrl;
                    }

                    if (resumeFile != null && resumeFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingEmployee.ResumePath))
                        {
                            string oldFilePath = Path.Combine(_hostEnvironment.WebRootPath, existingEmployee.ResumePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                        }
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "resumes", "employees");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + resumeFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await resumeFile.CopyToAsync(fileStream);
                        }
                        employee.ResumePath = "/resumes/employees/" + uniqueFileName;
                    }
                    else
                    {
                        employee.ResumePath = existingEmployee.ResumePath;
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            ViewData["TitleId"] = new SelectList(_context.Titles, "Id", "Name", employee.TitleId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Title)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                if (!string.IsNullOrEmpty(employee.PhotoUrl))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, employee.PhotoUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                if (!string.IsNullOrEmpty(employee.ResumePath))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, employee.ResumePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}