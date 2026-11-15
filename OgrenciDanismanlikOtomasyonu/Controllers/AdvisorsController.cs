using BusinessLogicLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class AdvisorsController : Controller
    {
        private int _GetManagerDepartmentId()
        {
            string? value = User.FindFirstValue("DepartmentId");
            return int.TryParse(value, out int departmentId) ? departmentId : 0;
        }

        public IActionResult Index(string? filter = null)
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            List<EntityLayer.clsAdvisorInfo> advisorList;

            if (filter == "inactiveWithStudents")
                advisorList = clsAdvisorBusiness.GetInactiveAdvisorsWithStudentsByDepartmentId(managerDepartmentId);
            else
                advisorList = clsAdvisorBusiness.GetAdvisorsByDepartmentId(managerDepartmentId);

            ViewBag.ActiveFilter = filter;
            return View(advisorList);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            if (id == null)
            {
                var newAdvisor = new clsAdvisorBusiness();
                newAdvisor.DepartmentId = managerDepartmentId;
                _SetEditViewBag(managerDepartmentId);
                return View(newAdvisor);
            }

            clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(id.Value);
            if (advisor is null) return NotFound();
            if (advisor.DepartmentId != managerDepartmentId) return NotFound();

            _SetEditViewBag(managerDepartmentId);
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int advisorId, string? firstName, string? lastName,
                                  string? academicTitle, string? sicilNo,
                                  string? email, string? phone, string? officeNo,
                                  string? username, string? password)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            bool isNew = advisorId == -1;

            if (string.IsNullOrWhiteSpace(firstName))
                ModelState.AddModelError("", "Ad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(lastName))
                ModelState.AddModelError("", "Soyad alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(sicilNo))
                ModelState.AddModelError("", "Sicil No alanı zorunludur.");
            if (string.IsNullOrWhiteSpace(username))
                ModelState.AddModelError("", "Kullanıcı adı zorunludur.");
            if (isNew && string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("", "Şifre zorunludur.");
            else if (!string.IsNullOrWhiteSpace(password) && password.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");

            if (!ModelState.IsValid)
            {
                clsAdvisorBusiness model = isNew ? new clsAdvisorBusiness()
                                                 : (clsAdvisorBusiness.Find(advisorId) ?? new clsAdvisorBusiness());
                model.FirstName     = firstName?.Trim() ?? "";
                model.LastName      = lastName?.Trim() ?? "";
                model.AcademicTitle = string.IsNullOrWhiteSpace(academicTitle) ? null : academicTitle.Trim();
                model.SicilNo       = sicilNo?.Trim() ?? "";
                model.DepartmentId  = managerDepartmentId;
                model.Email         = string.IsNullOrWhiteSpace(email)    ? null : email.Trim();
                model.Phone         = string.IsNullOrWhiteSpace(phone)    ? null : phone.Trim();
                model.OfficeNo      = string.IsNullOrWhiteSpace(officeNo) ? null : officeNo.Trim();
                model.Username      = username?.Trim() ?? "";
                _SetEditViewBag(managerDepartmentId);
                return View(model);
            }

            clsAdvisorBusiness advisor;
            if (isNew)
            {
                advisor = new clsAdvisorBusiness();
            }
            else
            {
                clsAdvisorBusiness? found = clsAdvisorBusiness.Find(advisorId);
                if (found is null) return NotFound();
                if (found.DepartmentId != managerDepartmentId) return NotFound();
                advisor = found;
            }

            advisor.FirstName     = firstName!.Trim();
            advisor.LastName      = lastName!.Trim();
            advisor.AcademicTitle = string.IsNullOrWhiteSpace(academicTitle) ? null : academicTitle.Trim();
            advisor.SicilNo       = sicilNo!.Trim();
            advisor.DepartmentId  = managerDepartmentId;
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
            _SetEditViewBag(managerDepartmentId);
            return View(advisor);
        }

        [HttpGet]
        public IActionResult ResetPassword(int id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(id);
            if (advisor is null) return NotFound();
            if (advisor.DepartmentId != managerDepartmentId) return NotFound();
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(int advisorId, string? newPassword, string? confirmPassword)
        {
            int managerDepartmentId = _GetManagerDepartmentId();

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError("", "Yeni şifre alanı zorunludur.");
            else if (newPassword.Length < 6)
                ModelState.AddModelError("", "Şifre en az 6 karakter olmalıdır.");
            else if (newPassword != confirmPassword)
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");

            clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(advisorId);
            if (advisor is null) return NotFound();
            if (advisor.DepartmentId != managerDepartmentId) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(advisor);
            }

            if (clsAdvisorBusiness.ResetPassword(advisorId, newPassword!))
            {
                TempData["Success"] = "Şifre başarıyla sıfırlandı.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Şifre sıfırlama işlemi başarısız oldu.");
            return View(advisor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleActive(int id)
        {
            int managerDepartmentId = _GetManagerDepartmentId();
            clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(id);
            if (advisor is null) return NotFound();
            if (advisor.DepartmentId != managerDepartmentId) return NotFound();

            bool newState = !advisor.IsActive;
            if (clsAdvisorBusiness.SetActive(id, newState))
                TempData["Success"] = newState ? "Danışman aktif edildi." : "Danışman pasif edildi.";
            else
                TempData["Error"] = "İşlem başarısız oldu.";

            return RedirectToAction("Index");
        }

        private void _SetEditViewBag(int managerDepartmentId)
        {
            EntityLayer.clsDepartment? department = clsDepartmentBusiness.Find(managerDepartmentId);
            ViewBag.ManagerDepartmentName = department?.DepartmentName ?? "";
        }
    }
}
