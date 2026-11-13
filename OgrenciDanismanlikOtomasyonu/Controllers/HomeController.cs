using Microsoft.AspNetCore.Mvc;
using OgrenciDanismanlikOtomasyonu.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectByRole(User.FindFirstValue(ClaimTypes.Role) ?? "");
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult RedirectByRole(string role) => role switch
        {
            "Student"           => RedirectToAction("Index", "Student"),
            "Advisor"           => RedirectToAction("Index", "Advisor"),
            "DepartmentManager" => RedirectToAction("Index", "Department"),
            _                   => RedirectToAction("Login", "Account")
        };
    }
}
