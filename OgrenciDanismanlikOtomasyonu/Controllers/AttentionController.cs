using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class AttentionController : Controller
    {
        public IActionResult Index()
        {
            int departmentId = _GetManagerDepartmentId();

            ViewBag.StudentsWithoutAdvisorCount       = clsStudentBusiness.GetActiveStudentsWithoutActiveAdvisorCountByDepartmentId(departmentId);
            ViewBag.InactiveAdvisorsWithStudentsCount = clsStudentBusiness.GetInactiveAdvisorsWithStudentsCountByDepartmentId(departmentId);
            ViewBag.NoAdvisorUrl       = Url.Action("Index", "Students", new { filter = "noAdvisor" });
            ViewBag.InactiveAdvisorUrl = Url.Action("Index", "Advisors", new { filter = "inactiveWithStudents" });

            return View();
        }

        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }
    }
}
