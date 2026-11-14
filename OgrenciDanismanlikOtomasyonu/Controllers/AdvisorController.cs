using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Advisor")]
    public class AdvisorController : Controller
    {
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var advisor = clsAdvisorBusiness.FindByUserId(userId);

            ViewBag.CurrentSemester = clsSemesterBusiness.GetCurrentSemester();

            if (advisor is null)
            {
                ViewBag.StudentCount = 0;
                return View(new List<EntityLayer.clsStudentInfo>());
            }

            var students = clsStudentBusiness.GetStudentsByAdvisorId(advisor.AdvisorId);
            ViewBag.StudentCount = students.Count;
            return View(students);
        }
    }
}
