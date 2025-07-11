using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MTApp.Data;
using MTApp.Models;
using OfficeOpenXml;

namespace MTApp.Controllers
{
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

        public async Task<IActionResult> Details(int? id)
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

        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["TitleId"] = new SelectList(_context.Titles, "Id", "Name");
            return View();
        }

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

        [HttpGet]
        public IActionResult BulkUpload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpload(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütfen yüklenecek bir Excel dosyası seçin.";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Lütfen geçerli bir .xlsx Excel dosyası yükleyin.";
                return View();
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var importedEmployees = new List<Employee>();
            var errors = new List<string>();
            int rowCount = 0;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            TempData["ErrorMessage"] = "Excel dosyasında geçerli bir çalışma sayfası bulunamadı.";
                            return View();
                        }

                        rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                string departmentName = worksheet.Cells[row, 9].Text.Trim();
                                string titleName = worksheet.Cells[row, 10].Text.Trim();

                                var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == departmentName);
                                var title = await _context.Titles.FirstOrDefaultAsync(t => t.Name == titleName);

                                if (department == null)
                                {
                                    errors.Add($"Satır {row}: '{departmentName}' adında bir departman bulunamadı. Lütfen önce departmanı ekleyin.");
                                    continue;
                                }
                                if (title == null)
                                {
                                    errors.Add($"Satır {row}: '{titleName}' adında bir unvan bulunamadı. Lütfen önce unvanı ekleyin.");
                                    continue;
                                }

                                var employee = new Employee
                                {
                                    EmployeeNumber = worksheet.Cells[row, 1].Text.Trim(),
                                    FirstName = worksheet.Cells[row, 2].Text.Trim(),
                                    LastName = worksheet.Cells[row, 3].Text.Trim(),
                                    NationalId = worksheet.Cells[row, 4].Text.Trim(),
                                    DateOfBirth = DateTime.Parse(worksheet.Cells[row, 5].Text.Trim()),
                                    Gender = worksheet.Cells[row, 6].Text.Trim(),
                                    MaritalStatus = worksheet.Cells[row, 7].Text.Trim(),
                                    Nationality = worksheet.Cells[row, 8].Text.Trim(),
                                    DepartmentId = department.Id,
                                    TitleId = title.Id,
                                    Salary = decimal.Parse(worksheet.Cells[row, 11].Text.Trim()),
                                    Email = worksheet.Cells[row, 12].Text.Trim(),
                                    PhoneNumber = worksheet.Cells[row, 13].Text.Trim(),
                                    Address = worksheet.Cells[row, 14].Text.Trim(),
                                    HireDate = DateTime.Parse(worksheet.Cells[row, 15].Text.Trim()),
                                    TerminationDate = string.IsNullOrEmpty(worksheet.Cells[row, 16].Text.Trim()) ? (DateTime?)null : DateTime.Parse(worksheet.Cells[row, 16].Text.Trim()),
                                    IsActive = bool.Parse(worksheet.Cells[row, 17].Text.Trim())
                                };

                                if (await _context.Employees.AnyAsync(e => e.EmployeeNumber == employee.EmployeeNumber || e.NationalId == employee.NationalId))
                                {
                                    errors.Add($"Satır {row}: Sicil Numarası '{employee.EmployeeNumber}' veya TC Kimlik Numarası '{employee.NationalId}' zaten mevcut. Bu personel atlandı.");
                                    continue;
                                }

                                importedEmployees.Add(employee);
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Satır {row} okunurken hata oluştu: {ex.Message}");
                            }
                        }
                    }
                }

                if (importedEmployees.Any())
                {
                    _context.Employees.AddRange(importedEmployees); // Excel ile toplu ekleme - güncellenecek
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"{importedEmployees.Count} personel başarıyla eklendi.";
                }
                else
                {
                    TempData["WarningMessage"] = "Excel dosyasında eklenecek geçerli personel bulunamadı.";
                }

                if (errors.Any())
                {
                    TempData["ErrorList"] = errors;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Dosya işlenirken beklenmeyen bir hata oluştu: {ex.Message}";
                if (errors.Any())
                {
                    TempData["ErrorList"] = errors;
                }
                return View();
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}