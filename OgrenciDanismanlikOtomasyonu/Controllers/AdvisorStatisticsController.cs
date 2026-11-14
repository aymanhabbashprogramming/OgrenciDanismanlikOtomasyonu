using BusinessLogicLayer;
using EntityLayer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class AdvisorStatisticsController : Controller
    {
        public IActionResult Index()
        {
            List<clsAdvisorStatisticsInfo> statisticsList = clsAdvisorBusiness.GetAdvisorStatisticsList();

            int    activeAdvisorCount = clsAdvisorBusiness.GetActiveAdvisorCount();
            int    activeStudentCount = clsStudentBusiness.GetActiveStudentCount();
            double average            = activeAdvisorCount > 0
                                            ? (double)activeStudentCount / activeAdvisorCount
                                            : 0.0;

            ViewBag.ActiveAdvisorCount = activeAdvisorCount;
            ViewBag.ActiveStudentCount = activeStudentCount;
            ViewBag.AverageDisplay     = average.ToString("F1", new System.Globalization.CultureInfo("tr-TR"));

            return View(statisticsList);
        }
    }
}
