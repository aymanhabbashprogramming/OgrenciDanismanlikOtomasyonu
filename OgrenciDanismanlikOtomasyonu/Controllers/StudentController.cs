using BusinessLogicLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            clsStudentBusiness? student = clsStudentBusiness.FindByUserId(userId);
            if (student is null) return View((EntityLayer.clsAdvisorInfo?)null);

            if (clsStudentBusiness.IsStudentAktif(student.StudentId))
            {
                List<EntityLayer.clsSurveyInfo> openSurveys =
                    clsSurveyBusiness.GetOpenByDepartment(student.DepartmentId);
                HashSet<int> answeredIds =
                    clsSurveyParticipationData.GetAnsweredSurveyIds(student.StudentId);
                ViewBag.OpenSurveyCount = openSurveys.Count(survey => !answeredIds.Contains(survey.SurveyId));
                ViewBag.HasOpenSurveys  = ViewBag.OpenSurveyCount > 0;
            }
            else
            {
                ViewBag.HasOpenSurveys = false;
            }

            EntityLayer.clsAdvisorInfo? advisor = clsStudentBusiness.GetCurrentAdvisor(student.StudentId);
            return View(advisor);
        }
    }
}
