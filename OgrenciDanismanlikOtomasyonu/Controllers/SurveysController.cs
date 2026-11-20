using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class SurveysController : Controller
    {
        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }

        private int _GetManagerId()
        {
            string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.TryParse(value, out int id) ? id : 0;
            return clsDepartmentManagerBusiness.GetManagerIdByUserId(userId);
        }

        public IActionResult Index()
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            List<clsSurveyInfo> surveyList = clsSurveyBusiness.GetAllByDepartment(managerDepartmentId);
            return View(surveyList);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            _LoadEditViewBag();

            if (id == null)
            {
                clsSurveyBusiness newSurvey = new()
                {
                    DepartmentId = managerDepartmentId,
                    OpensAt      = DateTime.Today,
                    ClosesAt     = DateTime.Today.AddDays(14)
                };
                return View(newSurvey);
            }

            clsSurveyBusiness? survey = clsSurveyBusiness.Find(id.Value);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            if (survey.Status != "Taslak")
            {
                TempData["Error"] = "Yayınlanmış anketler düzenlenemez.";
                return RedirectToAction(nameof(Index));
            }

            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int surveyId, string? title, string? description,
                                  int semesterId, DateTime opensAt, DateTime closesAt)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness survey;

            if (surveyId == -1)
            {
                survey = new clsSurveyBusiness
                {
                    DepartmentId       = managerDepartmentId,
                    CreatedByManagerId = _GetManagerId()
                };
            }
            else
            {
                clsSurveyBusiness? existing = clsSurveyBusiness.Find(surveyId);
                if (existing is null || existing.DepartmentId != managerDepartmentId) return NotFound();
                survey = existing;
            }

            survey.Title       = title?.Trim() ?? string.Empty;
            survey.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            survey.SemesterId  = semesterId;
            survey.OpensAt     = opensAt;
            survey.ClosesAt    = closesAt;

            if (!survey.Save())
            {
                _LoadEditViewBag();
                TempData["Error"] = "Anket kaydedilemedi. Başlık zorunludur ve bitiş tarihi başlangıçtan sonra olmalıdır.";
                return View(survey);
            }

            TempData["Success"] = surveyId == -1 ? "Anket oluşturuldu." : "Anket güncellendi.";
            return RedirectToAction(nameof(Questions), new { surveyId = survey.SurveyId });
        }

        [HttpGet]
        public IActionResult Questions(int surveyId)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            ViewBag.Survey       = survey;
            ViewBag.QuestionList = clsSurveyQuestionBusiness.GetBySurvey(surveyId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion(int surveyId, string? questionText, string? questionType)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            if (survey.Status != "Taslak")
            {
                TempData["Error"] = "Yayınlanmış ankete soru eklenemez.";
                return RedirectToAction(nameof(Questions), new { surveyId });
            }

            clsSurveyQuestionBusiness question = new()
            {
                SurveyId     = surveyId,
                QuestionText = questionText?.Trim() ?? string.Empty,
                QuestionType = questionType ?? "Puanlama"
            };

            if (!question.Save())
            {
                TempData["Error"] = "Soru eklenemedi. Soru metni boş olamaz.";
                return RedirectToAction(nameof(Questions), new { surveyId });
            }

            TempData["Success"] = "Soru eklendi.";
            return RedirectToAction(nameof(Questions), new { surveyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestion(int questionId, int surveyId)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            if (survey.Status != "Taslak")
            {
                TempData["Error"] = "Yayınlanmış anketin soruları silinemez.";
                return RedirectToAction(nameof(Questions), new { surveyId });
            }

            clsSurveyQuestionBusiness? question = clsSurveyQuestionBusiness.Find(questionId);
            if (question is null || question.SurveyId != surveyId) return NotFound();

            clsSurveyQuestionBusiness.Delete(questionId);
            TempData["Success"] = "Soru silindi.";
            return RedirectToAction(nameof(Questions), new { surveyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Publish(int surveyId)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            List<clsSurveyQuestion> questionList = clsSurveyQuestionBusiness.GetBySurvey(surveyId);
            if (questionList.Count == 0)
            {
                TempData["Error"] = "Anketi yayınlamak için en az bir soru ekleyin.";
                return RedirectToAction(nameof(Questions), new { surveyId });
            }

            if (!clsSurveyBusiness.Publish(surveyId))
            {
                TempData["Error"] = "Anket yayınlanamadı.";
                return RedirectToAction(nameof(Questions), new { surveyId });
            }

            TempData["Success"] = "Anket yayınlandı. Artık sorular değiştirilemez.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int surveyId)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            if (!clsSurveyBusiness.Delete(surveyId))
                TempData["Error"] = "Anket silinemedi. Yalnızca taslak anketler silinebilir.";
            else
                TempData["Success"] = "Anket silindi.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Results(int surveyId)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != managerDepartmentId) return NotFound();

            clsSurveyResultInfo? results = clsSurveyAnswerBusiness.GetResults(surveyId);
            if (results is null) return NotFound();

            return View(results);
        }

        private void _LoadEditViewBag()
        {
            ViewBag.Semesters = clsSemesterBusiness.GetAllSemesters();
        }
    }
}
