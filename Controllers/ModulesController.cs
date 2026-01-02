using ElectronicCourses.Data;
using ElectronicCourses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize] // domyślnie tylko zalogowani
public class ModulesController : Controller
{
    private readonly ApplicationDbContext _db;
    public ModulesController(ApplicationDbContext db) => _db = db;

    // lista modułów (only Admin)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var modules = await _db.Modules.Include(m => m.Course).ToListAsync();
        return View(modules);
    }

    // szczegóły modułu (dostępne wyłącznie dla zalogowanych użytkowników)
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var module = await _db.Modules
            .Include(m => m.Course)
            .ThenInclude(c => c.Enrollments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module == null) return NotFound();

        // check if user is enrolled (sprawdź czy użytkownik zapisany na kurs (chyba że Admin))
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var isEnrolled = module.Course.Enrollments.Any(e => e.UserId == userId);

        if (!isEnrolled && !User.IsInRole("Admin"))
        {
            return Forbid(); // blokada jeśli nie zapisany
        }

        return View(module);
    }

    // tworzenie modułu (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        ViewData["CourseId"] = new SelectList(_db.Courses, "Id", "Title");
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Module module, IFormFile? VideoFile)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("Błędy walidacji: " + string.Join(", ", errors));
            ViewData["CourseId"] = new SelectList(_db.Courses, "Id", "Title", module.CourseId); 
            return View(module);
        }
        if (ModelState.IsValid)
        {
            bool exists = await _db.Modules
                .AnyAsync(m => m.CourseId == module.CourseId && m.Order == module.Order);

            if (exists)
            {
                ModelState.AddModelError("Order", "Moduł o tej kolejności już istnieje w wybranym kursie.");
            }
            else
            {
                try
                {
                    if (VideoFile is { Length: > 0 })
                    {
                        var fileName = Path.GetFileName(VideoFile.FileName);
                        var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "modules");
                        Directory.CreateDirectory(dir);
                        var savePath = Path.Combine(dir, fileName);
                        using var stream = new FileStream(savePath, FileMode.Create);
                        await VideoFile.CopyToAsync(stream);

                        module.VideoPath = "/videos/modules/" + fileName;
                    }

                    _db.Modules.Add(module);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Błąd zapisu: {ex.Message}");
                }
            }
        }

        ViewData["CourseId"] = new SelectList(_db.Courses, "Id", "Title", module.CourseId);
        return View(module);
    }



    // edycja modułu (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var module = await _db.Modules.FindAsync(id);
        if (module == null) return NotFound();

        ViewData["CourseId"] = new SelectList(_db.Courses, "Id", "Title", module.CourseId);
        return View(module);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Module module, IFormFile VideoFile)
    {
        if (id != module.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (VideoFile != null && VideoFile.Length > 0)
            {
                var fileName = Path.GetFileName(VideoFile.FileName);
                var savePath = Path.Combine("wwwroot/videos/modules", fileName);
                using var stream = new FileStream(savePath, FileMode.Create);
                await VideoFile.CopyToAsync(stream);
                module.VideoPath = "/videos/modules/" + fileName;
            }

            _db.Update(module);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CourseId"] = new SelectList(_db.Courses, "Id", "Title", module.CourseId);
        return View(module);
    }


    // usuwanie modułu (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var module = await _db.Modules.FindAsync(id);
        if (module == null) return NotFound();
        return View(module);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var module = await _db.Modules.FindAsync(id);
        if (module == null) return NotFound();
        var courseId = module.CourseId;

        _db.Modules.Remove(module);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Courses", new { id = courseId });
    }
}
