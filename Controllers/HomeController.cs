using ElectronicCourses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicCourses.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult TestRoles()
        {
            var roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                                   .Select(c => c.Value);
            return Content("Twoje role: " + string.Join(", ", roles));
        }

    }
}
