using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DepartmentSummaryController : Controller
    {
        public IActionResult Index()
        {
            int departmentId = _GetManagerDepartmentId();
            ViewBag.Summary = clsDepartmentManagerBusiness.GetDepartmentSummaryByDepartmentId(departmentId);
            return View();
        }

        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }
    }
}
