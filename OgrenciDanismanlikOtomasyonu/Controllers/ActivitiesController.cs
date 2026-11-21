using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class ActivitiesController : Controller
    {
        public IActionResult Index(string? statusFilter, string? search)
        {
            int departmentId = _GetManagerDepartmentId();

            List<clsActivityInfo> activities = clsDepartmentManagerBusiness.GetActivitiesByDepartmentId(departmentId);

            string activeFilter = string.IsNullOrWhiteSpace(statusFilter) ? "Tümü" : statusFilter;

            if (activeFilter != "Tümü")
                activities = activities.Where(activity => activity.Status == activeFilter).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim().ToLowerInvariant();
                activities = activities.Where(activity =>
                    activity.Title.ToLowerInvariant().Contains(term) ||
                    activity.SemesterName.ToLowerInvariant().Contains(term)).ToList();
            }

            ViewBag.StatusFilter = activeFilter;
            ViewBag.Search       = search ?? string.Empty;
            return View(activities);
        }

        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }
    }
}
