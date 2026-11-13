using BusinessLogicLayer;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.FacultyCount        = clsFacultyBusiness.GetAllFaculties().Count;
            ViewBag.DepartmentCount     = clsDepartmentBusiness.GetAllDepartments().Count;
            ViewBag.ActiveAdvisorCount  = clsAdvisorBusiness.GetActiveAdvisorCount();
            ViewBag.ActiveStudentCount  = clsStudentBusiness.GetActiveStudentCount();
            return View();
        }
    }
}
