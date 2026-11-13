using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class StudentsController : Controller
    {
        public IActionResult Index()
        {
            return View(clsStudentBusiness.GetAllStudents());
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.Departments = clsDepartmentBusiness.GetAllDepartments();
            if (id == null)
            {
                ViewBag.ActiveAdvisors = new List<EntityLayer.clsAdvisorInfo>();
                return View(new clsStudentBusiness());
            }
            var student = clsStudentBusiness.Find(id.Value);
            if (student is null) return NotFound();
            ViewBag.CurrentAdvisor = clsStudentBusiness.GetCurrentAdvisor(student.StudentId);
            ViewBag.ActiveAdvisors = new List<EntityLayer.clsAdvisorInfo>();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int studentId, string? studentNumber, string? firstName,
                                  string? lastName, string? nationalId, int departmentId,
                                  string? educationType, string? mobilePhone, string? email,
                                  int? classYear, bool isInternational,
                                  string? username, string? password, int initialAdvisorId = 0)
        {
            bool isNew = studentId == -1;

            // Required-field checks
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
            if (departmentId <= 0)
                ModelState.AddModelError("", "Lütfen bölüm seçiniz.");
            if (isNew && string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("", "Şifre zorunludur.");
            else if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");

            // Load existing record once (needed for UserId in duplicate check)
            clsStudentBusiness? existing = isNew ? null : clsStudentBusiness.Find(studentId);
            if (!isNew && existing is null) return NotFound();
            int excludeUserId = existing?.UserId ?? -1;

            // Individual duplicate checks — each with its own Turkish message
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
                _SetEditViewBag(isNew, departmentId, studentId);
                clsStudentBusiness model = existing ?? new clsStudentBusiness();
                _Fill(model, studentNumber, firstName, lastName, nationalId, departmentId,
                      educationType, mobilePhone, email, classYear, isInternational, username);
                return View(model);
            }

            clsStudentBusiness student = existing ?? new clsStudentBusiness();
            _Fill(student, studentNumber, firstName, lastName, nationalId, departmentId,
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
            _SetEditViewBag(isNew, departmentId, studentId);
            return View(student);
        }

        private void _SetEditViewBag(bool isNew, int departmentId, int studentId)
        {
            ViewBag.Departments = clsDepartmentBusiness.GetAllDepartments();
            if (isNew)
                ViewBag.ActiveAdvisors = departmentId > 0
                    ? clsAdvisorBusiness.GetActiveAdvisorsForDepartment(departmentId)
                    : new List<EntityLayer.clsAdvisorInfo>();
            else
                ViewBag.CurrentAdvisor = clsStudentBusiness.GetCurrentAdvisor(studentId);
        }

        [HttpGet]
        public IActionResult ChangeAdvisor(int id)
        {
            var student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(id);
            ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(student.DepartmentId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeAdvisor(int studentId, int newAdvisorId, string? changeReason)
        {
            if (newAdvisorId <= 0)
                ModelState.AddModelError("", "Lütfen bir danışman seçiniz.");

            var student = clsStudentBusiness.Find(studentId);
            if (student is null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(studentId);
                ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(student.DepartmentId);
                return View(student);
            }

            if (clsStudentBusiness.AssignAdvisor(studentId, newAdvisorId, changeReason))
            {
                TempData["Success"] = "Danışman başarıyla atandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Atama başarısız oldu. Danışman aktif olmayabilir veya farklı bölümde olabilir.");
            ViewBag.AdvisorHistory = clsStudentBusiness.GetAdvisorHistory(studentId);
            ViewBag.ActiveAdvisors = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(student.DepartmentId);
            return View(student);
        }

        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            var student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int studentId, string? newPassword, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("", "Yeni şifre alanı zorunludur.");
            else if (newPassword.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");
            else if (newPassword != confirmPassword)
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");

            if (!ModelState.IsValid)
            {
                var s = clsStudentBusiness.Find(studentId);
                if (s is null) return NotFound();
                return View(s);
            }

            if (clsStudentBusiness.ResetPassword(studentId, newPassword!))
            {
                TempData["Success"] = "Şifre başarıyla sıfırlandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Şifre sıfırlama işlemi başarısız oldu.");
            var student = clsStudentBusiness.Find(studentId);
            if (student is null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            var student = clsStudentBusiness.Find(id);
            if (student is null) return NotFound();
            bool newState = !student.IsActive;
            if (clsStudentBusiness.SetActive(id, newState))
                TempData["Success"] = newState ? "Öğrenci aktif edildi." : "Öğrenci pasif edildi.";
            else
                TempData["Error"] = "İşlem başarısız oldu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult ActiveAdvisors(int departmentId)
        {
            var list = clsAdvisorBusiness.GetActiveAdvisorsForDepartment(departmentId);
            return Json(list.Select(a => new { a.AdvisorId, FullName = a.FullName }));
        }

        private static void _Fill(clsStudentBusiness s, string? studentNumber, string? firstName,
            string? lastName, string? nationalId, int departmentId, string? educationType,
            string? mobilePhone, string? email, int? classYear, bool isInternational, string? username)
        {
            s.StudentNumber   = studentNumber?.Trim()  ?? "";
            s.FirstName       = firstName?.Trim()      ?? "";
            s.LastName        = lastName?.Trim()       ?? "";
            s.NationalId      = nationalId?.Trim()     ?? "";
            s.DepartmentId    = departmentId;
            s.EducationType   = educationType?.Trim()  ?? "";
            s.MobilePhone     = string.IsNullOrWhiteSpace(mobilePhone) ? null : mobilePhone.Trim();
            s.Email           = string.IsNullOrWhiteSpace(email)       ? null : email.Trim();
            s.ClassYear       = classYear;
            s.IsInternational = isInternational;
            s.Username        = username?.Trim()       ?? "";
        }
    }
}
