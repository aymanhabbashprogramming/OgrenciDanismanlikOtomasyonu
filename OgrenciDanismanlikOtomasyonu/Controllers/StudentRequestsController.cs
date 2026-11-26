using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentRequestsController : Controller
    {
        // ── List ────────────────────────────────────────────────────────────────

        public IActionResult Index()
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            List<clsRequestInfo> requestList = clsRequestBusiness.GetMyRequests(studentId);
            return View(requestList);
        }

        // ── New request type selection ───────────────────────────────────────

        [HttpGet]
        public IActionResult New()
        {
            HashSet<string> allowedFormCodes = new() { "TF-ÖİŞL-02", "TF-ÖİŞL-05", "TF-ÖİŞL-06", "TF-ÖİŞL-14" };
            List<clsRequestType> allTypes = clsRequestTypeBusiness.GetAll()
                .Where(type => allowedFormCodes.Contains(type.FormCode))
                .ToList();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            List<clsRequestTypeAvailability> availabilityList = new();
            foreach (clsRequestType requestType in allTypes)
            {
                clsRequestPeriodInfo? openPeriod =
                    clsRequestPeriodBusiness.GetOpenPeriod(requestType.RequestTypeId, today);

                clsRequestPeriodInfo? nextPeriod = openPeriod is null
                    ? clsRequestPeriodBusiness.GetNextPeriodByRequestType(requestType.RequestTypeId)
                    : null;

                availabilityList.Add(new clsRequestTypeAvailability
                {
                    RequestTypeId = requestType.RequestTypeId,
                    FormCode      = requestType.FormCode,
                    FormName      = requestType.FormName,
                    IsOpen        = openPeriod is not null,
                    NextStartDate = nextPeriod?.StartDate
                });
            }

            return View(availabilityList);
        }

        // ── Form GET ────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Submit(string formCode)
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            clsRequestType? requestType = clsRequestTypeBusiness.GetByFormCode(formCode);
            if (requestType is null) return NotFound();

            clsRequestFormPreFillInfo? preFill = clsRequestBusiness.GetFormPreFillInfo(studentId);
            if (preFill is null) return NotFound();

            if (!preFill.HasAdvisor)
            {
                TempData["Error"] = "Talep oluşturmak için aktif bir danışmanınızın olması gerekir.";
                return RedirectToAction(nameof(New));
            }

            if (!preFill.HasCurrentSemester)
            {
                TempData["Error"] = "Şu anda aktif bir dönem bulunmamaktadır.";
                return RedirectToAction(nameof(New));
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            clsRequestPeriodInfo? openPeriod =
                clsRequestPeriodBusiness.GetOpenPeriod(requestType.RequestTypeId, today);

            if (openPeriod is null)
            {
                TempData["Error"] = $"{formCode} formu için başvuru dönemi şu an kapalıdır.";
                return RedirectToAction(nameof(New));
            }

            ViewData["FormCode"] = formCode;
            ViewData["FormName"] = requestType.FormName;
            return View($"Submit{formCode.Replace("TF-ÖİŞL-", "")}", preFill);
        }

        // ── Form POST ───────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit02(
            string   formCode,
            string?  reason,
            string[] courseCode,
            string[] courseName,
            string[] attendanceStatus,
            string[] operation,
            bool     declarationAccepted)
        {
            if (!declarationAccepted)
            {
                TempData["Error"] = "Formu göndermek için beyanı onaylamanız gereklidir.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            List<clsRequestCourse> courses = _BuildCourses02(courseCode, courseName, attendanceStatus, operation);
            if (courses.Count == 0)
            {
                TempData["Error"] = "En az bir ders satırı eklemelisiniz.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Gerekçe alanı zorunludur.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            clsRequestBusiness? saved = _SaveRequest(formCode, reason: reason.Trim(), courses: courses);
            if (saved is null) return RedirectToAction(nameof(New));

            TempData["Success"] = "Talebiniz başarıyla gönderildi.";
            return RedirectToAction(nameof(Detail), new { id = saved.RequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit05(
            string   formCode,
            string[] courseCode,
            string[] courseName,
            string[] previousGrade,
            bool     declarationAccepted)
        {
            if (!declarationAccepted)
            {
                TempData["Error"] = "Formu göndermek için beyanı onaylamanız gereklidir.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            List<clsRequestCourse> courses = _BuildCourses05(courseCode, courseName, previousGrade);
            if (courses.Count == 0)
            {
                TempData["Error"] = "En az bir ders satırı eklemelisiniz.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            clsRequestBusiness? saved = _SaveRequest(formCode, courses: courses);
            if (saved is null) return RedirectToAction(nameof(New));

            TempData["Success"] = "Talebiniz başarıyla gönderildi.";
            return RedirectToAction(nameof(Detail), new { id = saved.RequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit06(
            string   formCode,
            string?  declaredGano,
            string[] courseCode,
            string[] courseName,
            string[] courseSemester,
            bool     declarationAccepted)
        {
            if (!declarationAccepted)
            {
                TempData["Error"] = "Formu göndermek için beyanı onaylamanız gereklidir.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            if (!decimal.TryParse(declaredGano,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal parsedGano))
            {
                TempData["Error"] = "Geçerli bir GANO değeri giriniz.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            List<clsRequestCourse> courses = _BuildCourses06(courseCode, courseName, courseSemester);
            if (courses.Count == 0)
            {
                TempData["Error"] = "En az bir ders satırı eklemelisiniz.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            clsRequestBusiness? saved = _SaveRequest(formCode, declaredGano: parsedGano, courses: courses);
            if (saved is null) return RedirectToAction(nameof(New));

            TempData["Success"] = "Talebiniz başarıyla gönderildi.";
            return RedirectToAction(nameof(Detail), new { id = saved.RequestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit14(
            string   formCode,
            string[] table1CourseCode,
            string[] table1CourseName,
            string[] table1TheoryHours,
            string[] table1PracticeHours,
            string[] table1Ects,
            string[] table1AttendanceStatus,
            string[] table2CourseCode,
            string[] table2CourseName,
            string[] table2TheoryHours,
            string[] table2PracticeHours,
            string[] table2Ects,
            string[] table2University,
            string[] table2Department,
            bool     declarationAccepted)
        {
            if (!declarationAccepted)
            {
                TempData["Error"] = "Formu göndermek için beyanı onaylamanız gereklidir.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            List<clsRequestCourse> courses = _BuildCourses14(
                table1CourseCode, table1CourseName, table1TheoryHours,
                table1PracticeHours, table1Ects, table1AttendanceStatus,
                table2CourseCode, table2CourseName, table2TheoryHours,
                table2PracticeHours, table2Ects, table2University, table2Department);

            bool hasTable1 = courses.Any(course => course.TableGroup == "CurrentDepartment");
            bool hasTable2 = courses.Any(course => course.TableGroup == "Outside");

            if (!hasTable1 || !hasTable2)
            {
                TempData["Error"] = "Her iki tabloya da en az birer ders satırı eklemelisiniz.";
                return RedirectToAction(nameof(Submit), new { formCode });
            }

            clsRequestBusiness? saved = _SaveRequest(formCode, courses: courses);
            if (saved is null) return RedirectToAction(nameof(New));

            TempData["Success"] = "Talebiniz başarıyla gönderildi.";
            return RedirectToAction(nameof(Detail), new { id = saved.RequestId });
        }

        // ── Detail ──────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Detail(int id)
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            clsRequestDetailInfo? detail = clsRequestBusiness.GetRequestDetail(id, studentId);
            if (detail is null) return NotFound();

            return View(detail);
        }

        // ── Private helpers ──────────────────────────────────────────────────

        private int _GetStudentId()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            clsStudentBusiness? student = clsStudentBusiness.FindByUserId(userId);
            return student?.StudentId ?? 0;
        }

        private int _GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        private clsRequestBusiness? _SaveRequest(
            string                 formCode,
            string?                reason      = null,
            decimal?               declaredGano = null,
            List<clsRequestCourse>? courses     = null)
        {
            int studentId = _GetStudentId();
            int userId    = _GetUserId();
            if (studentId <= 0) return null;

            clsRequestType? requestType = clsRequestTypeBusiness.GetByFormCode(formCode);
            if (requestType is null) return null;

            clsRequestFormPreFillInfo? preFill = clsRequestBusiness.GetFormPreFillInfo(studentId);
            if (preFill is null || !preFill.HasAdvisor || !preFill.HasCurrentSemester) return null;

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            clsRequestPeriodInfo? openPeriod =
                clsRequestPeriodBusiness.GetOpenPeriod(requestType.RequestTypeId, today);

            if (openPeriod is null)
            {
                TempData["Error"] = $"{formCode} formu için başvuru dönemi kapanmıştır.";
                return null;
            }

            clsRequestBusiness request = new()
            {
                StudentId     = studentId,
                RequestTypeId = requestType.RequestTypeId,
                AdvisorId     = preFill.AdvisorId,
                SemesterId    = preFill.CurrentSemesterId,
                Reason        = reason,
                DeclaredGano  = declaredGano,
                Courses       = courses ?? new()
            };

            if (!request.Save(userId))
            {
                TempData["Error"] = "Talep kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
                return null;
            }

            return request;
        }

        private static List<clsRequestCourse> _BuildCourses02(
            string[] code, string[] name, string[] attendance, string[] operation)
        {
            List<clsRequestCourse> courses = new();
            int count = Math.Min(Math.Min(code.Length, name.Length),
                                 Math.Min(attendance.Length, operation.Length));
            for (int index = 0; index < count; index++)
            {
                if (string.IsNullOrWhiteSpace(code[index]) || string.IsNullOrWhiteSpace(name[index]))
                    continue;
                courses.Add(new clsRequestCourse
                {
                    CourseCode       = code[index].Trim(),
                    CourseName       = name[index].Trim(),
                    AttendanceStatus = string.IsNullOrWhiteSpace(attendance[index]) ? null : attendance[index].Trim(),
                    Operation        = string.IsNullOrWhiteSpace(operation[index])  ? null : operation[index].Trim()
                });
            }
            return courses;
        }

        private static List<clsRequestCourse> _BuildCourses05(
            string[] code, string[] name, string[] previousGrade)
        {
            List<clsRequestCourse> courses = new();
            int count = Math.Min(Math.Min(code.Length, name.Length), previousGrade.Length);
            for (int index = 0; index < count; index++)
            {
                if (string.IsNullOrWhiteSpace(code[index]) || string.IsNullOrWhiteSpace(name[index]))
                    continue;
                courses.Add(new clsRequestCourse
                {
                    CourseCode    = code[index].Trim(),
                    CourseName    = name[index].Trim(),
                    PreviousGrade = string.IsNullOrWhiteSpace(previousGrade[index]) ? null : previousGrade[index].Trim()
                });
            }
            return courses;
        }

        private static List<clsRequestCourse> _BuildCourses06(
            string[] code, string[] name, string[] semester)
        {
            List<clsRequestCourse> courses = new();
            int count = Math.Min(Math.Min(code.Length, name.Length), semester.Length);
            for (int index = 0; index < count; index++)
            {
                if (string.IsNullOrWhiteSpace(code[index]) || string.IsNullOrWhiteSpace(name[index]))
                    continue;
                int.TryParse(semester[index], out int semesterNumber);
                courses.Add(new clsRequestCourse
                {
                    CourseCode     = code[index].Trim(),
                    CourseName     = name[index].Trim(),
                    CourseSemester = semesterNumber > 0 ? semesterNumber : null
                });
            }
            return courses;
        }

        private static List<clsRequestCourse> _BuildCourses14(
            string[] t1Code, string[] t1Name, string[] t1Theory, string[] t1Practice,
            string[] t1Ects, string[] t1Attendance,
            string[] t2Code, string[] t2Name, string[] t2Theory, string[] t2Practice,
            string[] t2Ects, string[] t2University, string[] t2Department)
        {
            List<clsRequestCourse> courses = new();

            int count1 = Math.Min(Math.Min(t1Code.Length, t1Name.Length),
                                  Math.Min(t1Theory.Length, Math.Min(t1Practice.Length,
                                  Math.Min(t1Ects.Length, t1Attendance.Length))));
            for (int index = 0; index < count1; index++)
            {
                if (string.IsNullOrWhiteSpace(t1Code[index]) || string.IsNullOrWhiteSpace(t1Name[index]))
                    continue;
                int.TryParse(t1Theory[index],   out int theory);
                int.TryParse(t1Practice[index],  out int practice);
                int.TryParse(t1Ects[index],      out int ects);
                courses.Add(new clsRequestCourse
                {
                    TableGroup       = "CurrentDepartment",
                    CourseCode       = t1Code[index].Trim(),
                    CourseName       = t1Name[index].Trim(),
                    TheoryHours      = theory   > 0 ? theory   : null,
                    PracticeHours    = practice > 0 ? practice : null,
                    Ects             = ects     > 0 ? ects     : null,
                    AttendanceStatus = string.IsNullOrWhiteSpace(t1Attendance[index]) ? null : t1Attendance[index].Trim()
                });
            }

            int count2 = Math.Min(Math.Min(t2Code.Length, t2Name.Length),
                                  Math.Min(t2Theory.Length, Math.Min(t2Practice.Length,
                                  Math.Min(t2Ects.Length, Math.Min(t2University.Length, t2Department.Length)))));
            for (int index = 0; index < count2; index++)
            {
                if (string.IsNullOrWhiteSpace(t2Code[index]) || string.IsNullOrWhiteSpace(t2Name[index]))
                    continue;
                int.TryParse(t2Theory[index],   out int theory);
                int.TryParse(t2Practice[index],  out int practice);
                int.TryParse(t2Ects[index],      out int ects);
                courses.Add(new clsRequestCourse
                {
                    TableGroup    = "Outside",
                    CourseCode    = t2Code[index].Trim(),
                    CourseName    = t2Name[index].Trim(),
                    TheoryHours   = theory   > 0 ? theory   : null,
                    PracticeHours = practice > 0 ? practice : null,
                    Ects          = ects     > 0 ? ects     : null,
                    University    = string.IsNullOrWhiteSpace(t2University[index])  ? null : t2University[index].Trim(),
                    Department    = string.IsNullOrWhiteSpace(t2Department[index])  ? null : t2Department[index].Trim()
                });
            }

            return courses;
        }
    }
}
