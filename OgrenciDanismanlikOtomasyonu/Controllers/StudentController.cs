using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var student = clsStudentBusiness.FindByUserId(userId);
            if (student is null) return View((EntityLayer.clsAdvisorInfo?)null);
            var advisor = clsStudentBusiness.GetCurrentAdvisor(student.StudentId);
            return View(advisor);
        }
    }
}
