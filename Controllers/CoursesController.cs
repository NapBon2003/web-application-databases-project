using ElectronicCourses.Models;
using ElectronicCourses.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class CoursesController : Controller
{
    private readonly ApplicationDbContext _db;
    public CoursesController(ApplicationDbContext db) => _db = db;

    // lista kursów (dostępna dla wszystkich)
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var courses = await _db.Courses
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .AsNoTracking()
            .ToListAsync();

        return View(courses);
    }

    // szczegóły kursu (dostępne dla wszystkich)
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Modules)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        return View(course);
    }
    [Authorize] // tylko zalogowani
    public async Task<IActionResult> Player(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        return View(course); // nowy widok CoursePlayer.cshtml
    }

    // tworzenie kursus (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course, IFormFile? CoverImage)
    {
        if (!ModelState.IsValid) { 
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage); 
            Console.WriteLine("Błędy walidacji: " + string.Join(", ", errors)); 
            return View(course); 
        }
        try
        {
            if (CoverImage is { Length: > 0 })
            {
                var fileName = Path.GetFileName(CoverImage.FileName);
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "covers");
                Directory.CreateDirectory(dir); // zapewnij istnienie
                var savePath = Path.Combine(dir, fileName);
                using var stream = new FileStream(savePath, FileMode.Create);
                await CoverImage.CopyToAsync(stream);

                course.CoverImagePath = "/images/covers/" + fileName;
            }

            _db.Courses.Add(course); 
            Console.WriteLine("Dodaję kurs: " + course.Title); 
            await _db.SaveChangesAsync(); 
            Console.WriteLine("Zapisano kurs do bazy!");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Błąd zapisu: {ex.Message}");
            return View(course);
        }
    }



    // edycja kursu (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course, IFormFile CoverImage)
    {
        if (id != course.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (CoverImage != null && CoverImage.Length > 0)
            {
                var fileName = Path.GetFileName(CoverImage.FileName);
                var savePath = Path.Combine("wwwroot/images/covers", fileName);

                using var stream = new FileStream(savePath, FileMode.Create);
                await CoverImage.CopyToAsync(stream);

                course.CoverImagePath = "/images/covers/" + fileName;
            }

            _db.Update(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(course);
    }


    // usuwanie kursu (only Admin)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await _db.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        if (course.Enrollments?.Any() == true)
        {
            _db.Enrollments.RemoveRange(course.Enrollments);
        }

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
