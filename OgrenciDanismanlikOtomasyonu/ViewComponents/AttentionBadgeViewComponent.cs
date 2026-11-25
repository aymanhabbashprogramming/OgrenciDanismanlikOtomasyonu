using BusinessLogicLayer;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.ViewComponents
{
    public class AttentionBadgeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            int departmentId = int.TryParse(
                UserClaimsPrincipal.FindFirstValue("DepartmentId"),
                out int parsedId) ? parsedId : 0;

            int count = departmentId > 0
                ? clsStudentBusiness.GetActiveStudentsWithoutActiveAdvisorCountByDepartmentId(departmentId)
                  + clsStudentBusiness.GetInactiveAdvisorsWithStudentsCountByDepartmentId(departmentId)
                : 0;

            return View(count);
        }
    }
}
