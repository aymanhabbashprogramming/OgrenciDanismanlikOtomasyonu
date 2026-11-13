using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    public class RolesController : Controller
    {
        public IActionResult Index()
        {
            List<clsRole> roles = clsRoleBusiness.GetAllRoles();
            return View(roles);
        }
    }
}
