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
            clsStudentBusiness? student = clsStudentBusiness.FindByUserId(userId);
            if (student is null) return NotFound();

            EntityLayer.clsStudentHomeInfo? homeInfo =
                clsStudentBusiness.GetStudentHomeInfo(student.StudentId);
            if (homeInfo is null) return NotFound();

            return View(homeInfo);
        }
    }
}
