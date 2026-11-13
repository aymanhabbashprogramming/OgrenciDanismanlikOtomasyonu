using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class AdvisorsController : Controller
    {
        public IActionResult Index()
        {
            return View(clsAdvisorBusiness.GetAllAdvisors());
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.Departments = clsDepartmentBusiness.GetAllDepartments();
            if (id == null) return View(new clsAdvisorBusiness());
            var advisor = clsAdvisorBusiness.Find(id.Value);
            if (advisor is null) return NotFound();
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int advisorId, string? firstName, string? lastName,
                                  string? academicTitle, string? sicilNo, int departmentId,
                                  string? email, string? phone, string? officeNo,
                                  string? username, string? password)
        {
            bool isNew = advisorId == -1;

            if (string.IsNullOrWhiteSpace(firstName))
                ModelState.AddModelError("", "Ad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(lastName))
                ModelState.AddModelError("", "Soyad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(sicilNo))
                ModelState.AddModelError("", "Sicil No alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(username))
                ModelState.AddModelError("", "Kullanıcı adı zorunludur.");
            if (departmentId <= 0)
                ModelState.AddModelError("", "Lütfen bölüm seçiniz.");
            if (isNew && string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("", "Şifre zorunludur.");
            else if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = clsDepartmentBusiness.GetAllDepartments();
                clsAdvisorBusiness model = isNew ? new clsAdvisorBusiness()
                                                 : (clsAdvisorBusiness.Find(advisorId) ?? new clsAdvisorBusiness());
                model.FirstName     = firstName?.Trim() ?? "";
                model.LastName      = lastName?.Trim() ?? "";
                model.AcademicTitle = string.IsNullOrWhiteSpace(academicTitle) ? null : academicTitle.Trim();
                model.SicilNo       = sicilNo?.Trim() ?? "";
                model.DepartmentId  = departmentId;
                model.Email         = string.IsNullOrWhiteSpace(email)    ? null : email.Trim();
                model.Phone         = string.IsNullOrWhiteSpace(phone)    ? null : phone.Trim();
                model.OfficeNo      = string.IsNullOrWhiteSpace(officeNo) ? null : officeNo.Trim();
                model.Username      = username?.Trim() ?? "";
                return View(model);
            }

            clsAdvisorBusiness advisor;
            if (isNew)
            {
                advisor = new clsAdvisorBusiness();
            }
            else
            {
                var found = clsAdvisorBusiness.Find(advisorId);
                if (found is null) return NotFound();
                advisor = found;
            }

            advisor.FirstName     = firstName!.Trim();
            advisor.LastName      = lastName!.Trim();
            advisor.AcademicTitle = string.IsNullOrWhiteSpace(academicTitle) ? null : academicTitle.Trim();
            advisor.SicilNo       = sicilNo!.Trim();
            advisor.DepartmentId  = departmentId;
            advisor.Email         = string.IsNullOrWhiteSpace(email)    ? null : email.Trim();
            advisor.Phone         = string.IsNullOrWhiteSpace(phone)    ? null : phone.Trim();
            advisor.OfficeNo      = string.IsNullOrWhiteSpace(officeNo) ? null : officeNo.Trim();
            advisor.Username      = username!.Trim();

            if (isNew) advisor.SetPassword(password!);

            if (advisor.Save())
            {
                TempData["Success"] = isNew ? "Danışman başarıyla eklendi." : "Danışman başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kayıt başarısız oldu. Sicil numarası veya kullanıcı adı zaten kullanımda olabilir.");
            ViewBag.Departments = clsDepartmentBusiness.GetAllDepartments();
            return View(advisor);
        }

        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            var advisor = clsAdvisorBusiness.Find(id);
            if (advisor is null) return NotFound();
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int advisorId, string? newPassword, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("", "Yeni şifre alanı zorunludur.");
            else if (newPassword.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");
            else if (newPassword != confirmPassword)
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");

            if (!ModelState.IsValid)
            {
                var a = clsAdvisorBusiness.Find(advisorId);
                if (a is null) return NotFound();
                return View(a);
            }

            if (clsAdvisorBusiness.ResetPassword(advisorId, newPassword!))
            {
                TempData["Success"] = "Şifre başarıyla sıfırlandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Şifre sıfırlama işlemi başarısız oldu.");
            var advisor = clsAdvisorBusiness.Find(advisorId);
            if (advisor is null) return NotFound();
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            var advisor = clsAdvisorBusiness.Find(id);
            if (advisor is null) return NotFound();
            bool newState = !advisor.IsActive;
            if (clsAdvisorBusiness.SetActive(id, newState))
                TempData["Success"] = newState ? "Danışman aktif edildi." : "Danışman pasif edildi.";
            else
                TempData["Error"] = "İşlem başarısız oldu.";
            return RedirectToAction("Index");
        }
    }
}
