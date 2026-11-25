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
            int managerUserId = _GetManagerUserId();

            ViewBag.ManagerProfile  = clsDepartmentManagerBusiness.GetManagerProfileInfoByUserId(managerUserId);
            ViewBag.CurrentSemester = clsSemesterBusiness.GetCurrentSemester();

            return View();
        }

        private int _GetManagerUserId()
        {
            string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out int userId) ? userId : 0;
        }
    }
}
