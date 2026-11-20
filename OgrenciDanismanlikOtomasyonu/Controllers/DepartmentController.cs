using BusinessLogicLayer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            ViewBag.ActiveAdvisorCount                = clsAdvisorBusiness.GetActiveAdvisorCountByDepartmentId(managerDepartmentId);
            ViewBag.ActiveStudentCount                = clsStudentBusiness.GetActiveStudentCountByDepartmentId(managerDepartmentId);
            ViewBag.CurrentSemester                   = clsSemesterBusiness.GetCurrentSemester();
            ViewBag.StudentsWithoutAdvisorCount       = clsStudentBusiness.GetActiveStudentsWithoutActiveAdvisorCountByDepartmentId(managerDepartmentId);
            ViewBag.InactiveAdvisorsWithStudentsCount = clsStudentBusiness.GetInactiveAdvisorsWithStudentsCountByDepartmentId(managerDepartmentId);
            ViewBag.AdvisorStatisticsList              = clsAdvisorBusiness.GetAdvisorStatisticsListByDepartmentId(managerDepartmentId)
                                                            .Where(stat => stat.IsActive)
                                                            .Take(5)
                                                            .ToList();
            return View();
        }

        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }
    }
}
