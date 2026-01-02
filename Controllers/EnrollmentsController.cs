using ElectronicCourses.Models;
using ElectronicCourses.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

[Authorize]
public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext _db;
    public EnrollmentsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var query = _db.Enrollments.Include(e => e.Course).Include(e => e.User).AsQueryable();
        if (!isAdmin)
        {
            query = query.Where(e => e.UserId == userId);
        }

        return View(await query.AsNoTracking().ToListAsync());
    }

    public async Task<IActionResult> Create(int courseId)
    {
        // admin nie może się zapisywać
        if (User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var course = await _db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        ViewBag.Course = course;
        return View(new Enrollment { CourseId = courseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePost(int courseId)
    {
        // admin nie może się zapisywać
        if (User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var course = await _db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var exists = await _db.Enrollments.AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        if (exists)
        {
            ModelState.AddModelError("", "Jesteś już zapisany na ten kurs.");
            ViewBag.Course = course;
            return View("Create", new Enrollment { CourseId = courseId });
        }

        var enrollment = new Enrollment { CourseId = courseId, UserId = userId, ProgressPercent = 0 };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProgress(int id)
    {
        var e = await _db.Enrollments.Include(x => x.Course).FirstOrDefaultAsync(x => x.Id == id);
        if (e == null) return NotFound();
        return View(e);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProgress(int id, int progressPercent)
    {
        var e = await _db.Enrollments.FindAsync(id);
        if (e == null) return NotFound();

        if (progressPercent < 0 || progressPercent > 100)
        {
            ModelState.AddModelError("", "Procent postępu musi mieścić się w zakresie 0–100.");
            return View(e);
        }

        e.ProgressPercent = progressPercent;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);

        if (enrollment == null)
        {
            return NotFound();
        }

        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
