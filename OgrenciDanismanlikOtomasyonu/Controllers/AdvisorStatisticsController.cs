using BusinessLogicLayer;
using EntityLayer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class AdvisorStatisticsController : Controller
    {
        public IActionResult Index()
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            List<clsAdvisorStatisticsInfo> statisticsList =
                clsAdvisorBusiness.GetAdvisorStatisticsListByDepartmentId(managerDepartmentId);

            int    activeAdvisorCount = clsAdvisorBusiness.GetActiveAdvisorCountByDepartmentId(managerDepartmentId);
            int    activeStudentCount = clsStudentBusiness.GetActiveStudentCountByDepartmentId(managerDepartmentId);
            double average            = activeAdvisorCount > 0
                                            ? (double)activeStudentCount / activeAdvisorCount
                                            : 0.0;

            ViewBag.ActiveAdvisorCount = activeAdvisorCount;
            ViewBag.ActiveStudentCount = activeStudentCount;
            ViewBag.AverageDisplay     = average.ToString("F1", new System.Globalization.CultureInfo("tr-TR"));

            return View(statisticsList);
        }

        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }
    }
}
