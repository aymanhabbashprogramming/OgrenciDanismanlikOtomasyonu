using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentSurveysController : Controller
    {
        private (int studentId, int departmentId, bool isAktif) _GetStudentContext()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            clsStudentBusiness? student = clsStudentBusiness.FindByUserId(userId);
            if (student is null) return (0, 0, false);

            bool isAktif = clsStudentBusiness.IsStudentAktif(student.StudentId);
            return (student.StudentId, student.DepartmentId, isAktif);
        }

        public IActionResult Index()
        {
            var (studentId, departmentId, isAktif) = _GetStudentContext();
            if (studentId == 0) return RedirectToAction("Index", "Student");

            if (!isAktif)
            {
                ViewBag.NotAktif = true;
                return View(new List<clsSurveyInfo>());
            }

            List<clsSurveyInfo> openSurveys = clsSurveyBusiness.GetOpenByDepartment(departmentId);
            HashSet<int> answeredIds = DataAccessLayer.clsSurveyParticipationData.GetAnsweredSurveyIds(studentId);
            ViewBag.AnsweredIds = answeredIds;
            return View(openSurveys);
        }

        [HttpGet]
        public IActionResult Answer(int surveyId)
        {
            var (studentId, departmentId, isAktif) = _GetStudentContext();
            if (studentId == 0) return RedirectToAction("Index", "Student");
            if (!isAktif)
            {
                TempData["Error"] = "Kayıt durumunuz anket doldurmaya uygun değil.";
                return RedirectToAction(nameof(Index));
            }

            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != departmentId) return NotFound();
            if (!survey.IsOpen) return RedirectToAction(nameof(Index));

            if (DataAccessLayer.clsSurveyParticipationData.HasStudentAnswered(surveyId, studentId))
            {
                TempData["Error"] = "Bu anketi zaten doldurdunuz.";
                return RedirectToAction(nameof(Index));
            }

            List<clsSurveyQuestion> questions = clsSurveyQuestionBusiness.GetBySurvey(surveyId);
            ViewBag.Survey    = survey;
            ViewBag.Questions = questions;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Answer(int surveyId, IFormCollection form)
        {
            var (studentId, departmentId, isAktif) = _GetStudentContext();
            if (studentId == 0) return RedirectToAction("Index", "Student");
            if (!isAktif)
            {
                TempData["Error"] = "Kayıt durumunuz anket doldurmaya uygun değil.";
                return RedirectToAction(nameof(Index));
            }

            clsSurveyBusiness? survey = clsSurveyBusiness.Find(surveyId);
            if (survey is null || survey.DepartmentId != departmentId) return NotFound();
            if (!survey.IsOpen) return RedirectToAction(nameof(Index));

            if (DataAccessLayer.clsSurveyParticipationData.HasStudentAnswered(surveyId, studentId))
            {
                TempData["Error"] = "Bu anketi zaten doldurdunuz.";
                return RedirectToAction(nameof(Index));
            }

            List<clsSurveyQuestion> questions = clsSurveyQuestionBusiness.GetBySurvey(surveyId);
            List<clsSurveyAnswer> answers = new();

            foreach (clsSurveyQuestion question in questions)
            {
                string fieldKey = $"q_{question.SurveyQuestionId}";
                string? rawValue = form[fieldKey];

                if (question.QuestionType == "Puanlama")
                {
                    if (!int.TryParse(rawValue, out int score) || score < 1 || score > 10)
                    {
                        TempData["Error"] = "Lütfen tüm puanlama sorularını yanıtlayın (1–10).";
                        ViewBag.Survey      = survey;
                        ViewBag.Questions   = questions;
                        ViewBag.SavedValues = form;
                        return View();
                    }
                    answers.Add(new clsSurveyAnswer { SurveyQuestionId = question.SurveyQuestionId, Score = score });
                }
                else
                {
                    string? comment = string.IsNullOrWhiteSpace(rawValue) ? null : rawValue.Trim();
                    answers.Add(new clsSurveyAnswer
                    {
                        SurveyQuestionId = question.SurveyQuestionId,
                        Score            = null,
                        Comment          = comment
                    });
                }
            }

            if (!clsSurveyAnswerBusiness.SubmitAnswers(surveyId, studentId, answers))
            {
                TempData["Error"] = "Yanıtlar kaydedilemedi. Lütfen tekrar deneyin.";
                ViewBag.Survey      = survey;
                ViewBag.Questions   = questions;
                ViewBag.SavedValues = form;
                return View();
            }

            TempData["Success"] = "Anket yanıtlarınız kaydedildi. Teşekkürler!";
            return RedirectToAction(nameof(Index));
        }
    }
}
