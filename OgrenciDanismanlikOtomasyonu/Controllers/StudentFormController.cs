using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Advisor,DepartmentManager")]
    public class StudentFormController : Controller
    {
        [HttpGet]
        public IActionResult Details(int studentId)
        {
            clsStudentBusiness? student = clsStudentBusiness.Find(studentId);
            if (student is null) return NotFound();

            if (!_CanViewStudent(student))
                return NotFound();

            clsStudentFormInfo? formInfo = clsStudentFileBusiness.GetStudentFormInfo(studentId);
            if (formInfo is null) return NotFound();

            return View(formInfo);
        }

        private bool _CanViewStudent(clsStudentBusiness student)
        {
            if (User.IsInRole("DepartmentManager"))
            {
                string? departmentIdClaim = User.FindFirstValue("DepartmentId");
                if (!int.TryParse(departmentIdClaim, out int managerDepartmentId))
                    return false;
                return student.DepartmentId == managerDepartmentId;
            }

            if (User.IsInRole("Advisor"))
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                clsAdvisorBusiness? advisor = clsAdvisorBusiness.FindByUserId(userId);
                if (advisor is null) return false;

                clsAdvisorInfo? currentAdvisor = clsStudentBusiness.GetCurrentAdvisor(student.StudentId);
                return currentAdvisor?.AdvisorId == advisor.AdvisorId;
            }

            return false;
        }
    }
}
