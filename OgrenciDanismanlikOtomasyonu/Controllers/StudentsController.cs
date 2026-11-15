using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class StudentsController : Controller
    {
        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }

        public IActionResult Index(string? filter = null, int? advisorId = null)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            List<EntityLayer.clsStudentInfo> studentList;

            if (filter == "noAdvisor")
                studentList = clsStudentBusiness.GetStudentsWithoutActiveAdvisorByDepartmentId(managerDepartmentId);
            else if (filter == "inactiveAdvisor")
                studentList = clsStudentBusiness.GetStudentsWithInactiveAdvisorByDepartmentId(managerDepartmentId);
            else if (advisorId.HasValue)
                studentList = clsStudentBusiness.GetStudentsByAdvisorId(advisorId.Value);
            else
                studentList = clsStudentBusiness.GetAllStudentsByDepartmentId(managerDepartmentId);

            ViewBag.ActiveFilter    = filter;
            ViewBag.FilterAdvisorId = advisorId;

            if (advisorId.HasValue)
            {
                clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(advisorId.Value);
                if (advisor is null || advisor.DepartmentId != managerDepartmentId)
                    return NotFound();
                ViewBag.FilterAdvisorName = advisor.GetFullName();
            }

            return View(studentList);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            _SetEditViewBag(isNew: id == null, managerDepartmentId: managerDepartmentId, studentId: id ?? -1);

            if (id == null)
            {
                var newStudent = new clsStudentBusiness();
                newStudent.DepartmentId = managerDepartmentId;
                return View(newStudent);
            }

            clsStudentBusiness? student = clsStudentBusiness.Find(id.Value);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();

            ViewBag.CurrentAdvisor = clsStudentBusiness.GetCurrentAdvisor(student.StudentId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int studentId, string? studentNumber, string? firstName,
                                  string? lastName, string? nationalId,
                                  string? educationType, string? mobilePhone, string? email,
                                  int? classYear, bool isInternational,
                                  string? username, string? password, int initialAdvisorId = 0)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            bool isNew = studentId == -1;

            if (string.IsNullOrWhiteSpace(firstName))
                ModelState.AddModelError("", "Ad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(lastName))
                ModelState.AddModelError("", "Soyad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(studentNumber))
                ModelState.AddModelError("", "Öğrenci numarası zorunludur.");
            if (string.IsNullOrWhiteSpace(username))
                ModelState.AddModelError("", "Kullanıcı adı zorunludur.");
            if (string.IsNullOrWhiteSpace(educationType))
                ModelState.AddModelError("", "Öğretim türü zorunludur.");
            if (isNew && string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("", "Şifre zorunludur.");
            else if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");

            clsStudentBusiness? existing = isNew ? null : clsStudentBusiness.Find(studentId);
            if (!isNew && existing is null) return NotFound();
            if (!isNew && existing!.DepartmentId != managerDepartmentId) return NotFound();

            int excludeUserId = existing?.UserId ?? -1;

            if (!string.IsNullOrWhiteSpace(studentNumber) &&
                clsStudentBusiness.IsStudentNumberTaken(studentNumber.Trim(), studentId))
                ModelState.AddModelError("", "Bu öğrenci numarası zaten kayıtlı.");

            string? trimmedNationalId = nationalId?.Trim();
            if (!string.IsNullOrEmpty(trimmedNationalId) &&
                clsStudentBusiness.IsNationalIdTaken(trimmedNationalId, studentId))
                ModelState.AddModelError("", "Bu TC kimlik no zaten kayıtlı.");

            if (!string.IsNullOrWhiteSpace(username) &&
                clsStudentBusiness.IsUsernameTaken(username.Trim(), excludeUserId))
                ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor.");

            if (!ModelState.IsValid)
            {
                _SetEditViewBag(isNew, managerDepartmentId, studentId);
                clsStudentBusiness model = existing ?? new clsStudentBusiness();
                model.DepartmentId = managerDepartmentId;
                _Fill(model, studentNumber, firstName, lastName, nationalId, managerDepartmentId,
                      educationType, mobilePhone, email, classYear, isInternational, username);
                return View(model);
            }

            clsStudentBusiness student = existing ?? new clsStudentBusiness();
            _Fill(student, studentNumber, firstName, lastName, nationalId, managerDepartmentId,
                  educationType, mobilePhone, email, classYear, isInternational, username);

            if (isNew) student.SetPassword(password!);

            if (student.Save())
            {
                if (isNew && initialAdvisorId > 0)
                    clsStudentBusiness.AssignAdvisor(student.StudentId, initialAdvisorId);

                TempData["Success"] = isNew ? "Öğrenci başarıyla eklendi." : "Öğrenci başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu. Lütfen tekrar deneyin.");
            _SetEditViewBag(isNew, managerDepartmentId, studentId);
            return View(student);
        }

        private void _SetEditViewBag(bool isNew, int managerDepartmentId, int studentId)
        {
            EntityLayer.clsDepartment? department = clsDepartmentBusiness.Find(managerDepartmentId);
            ViewBag.ManagerDepartmentName = department?.DepartmentName ?? "";

            if (isNew)
                ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(managerDepartmentId);
            else
                ViewBag.CurrentAdvisor = clsStudentBusiness.GetCurrentAdvisor(studentId);
        }

        [HttpGet]
        public IActionResult ChangeAdvisor(int id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsStudentBusiness? student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();

            ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(id);
            ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(managerDepartmentId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAdvisor(int studentId, int newAdvisorId, string? changeReason)
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            if (newAdvisorId <= 0)
                ModelState.AddModelError("", "Lütfen bir danışman seçiniz.");

            clsStudentBusiness? student = clsStudentBusiness.Find(studentId);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(studentId);
                ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(managerDepartmentId);
                return View(student);
            }

            if (clsStudentBusiness.AssignAdvisor(studentId, newAdvisorId, changeReason))
            {
                TempData["Success"] = "Danışman başarıyla atandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Atama başarısız oldu. Danışman aktif olmayabilir veya farklı bölümde olabilir.");
            ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(studentId);
            ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(managerDepartmentId);
            return View(student);
        }

        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsStudentBusiness? student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int studentId, string? newPassword, string? confirmPassword)
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("", "Yeni şifre alanı zorunludur.");
            else if (newPassword.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");
            else if (newPassword != confirmPassword)
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");

            clsStudentBusiness? student = clsStudentBusiness.Find(studentId);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            if (clsStudentBusiness.ResetPassword(studentId, newPassword!))
            {
                TempData["Success"] = "Şifre başarıyla sıfırlandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Şifre sıfırlama işlemi başarısız oldu.");
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsStudentBusiness? student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            if (student.DepartmentId != managerDepartmentId) return NotFound();

            bool newState = !student.IsActive;
            if (clsStudentBusiness.SetActive(id, newState))
                TempData["Success"] = newState ? "Öğrenci aktif edildi." : "Öğrenci pasif edildi.";
            else
                TempData["Error"] = "İşlem başarısız oldu.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult ActiveAdvisors()
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            List<EntityLayer.clsAdvisorInfo> advisorList =
                clsAdvisorBusiness.GetActiveAdvisorsForDepartment(managerDepartmentId);
            return Json(advisorList.Select(advisor => new
            {
                advisor.AdvisorId,
                FullName = advisor.FullName
            }));
        }

        private static void _Fill(clsStudentBusiness student, string? studentNumber, string? firstName,
            string? lastName, string? nationalId, int departmentId, string? educationType,
            string? mobilePhone, string? email, int? classYear, bool isInternational, string? username)
        {
            student.StudentNumber   = studentNumber?.Trim()  ?? "";
            student.FirstName       = firstName?.Trim()      ?? "";
            student.LastName        = lastName?.Trim()       ?? "";
            student.NationalId      = nationalId?.Trim()     ?? "";
            student.DepartmentId    = departmentId;
            student.EducationType   = educationType?.Trim()  ?? "";
            student.MobilePhone     = string.IsNullOrWhiteSpace(mobilePhone) ? null : mobilePhone.Trim();
            student.Email           = string.IsNullOrWhiteSpace(email)       ? null : email.Trim();
            student.ClassYear       = classYear;
            student.IsInternational = isInternational;
            student.Username        = username?.Trim()       ?? "";
        }
    }
}
