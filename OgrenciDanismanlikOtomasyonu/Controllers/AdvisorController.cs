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
            if (advisor is null) return View(new List<EntityLayer.clsStudentInfo>());
            return View(clsStudentBusiness.GetStudentsByAdvisorId(advisor.AdvisorId));
        }
    }
}
