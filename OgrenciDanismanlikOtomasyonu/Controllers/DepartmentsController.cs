using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            var departments = clsDepartmentBusiness.GetAllDepartments();
            ViewBag.FacultyNames = clsFacultyBusiness.GetAllFaculties()
                .ToDictionary(f => f.FacultyId, f => f.FacultyName);
            return View(departments);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewBag.Faculties = clsFacultyBusiness.GetAllFaculties();

            if (id == null)
                return View(new clsDepartmentBusiness());

            clsDepartmentBusiness? dept = clsDepartmentBusiness.Find(id.Value);
            if (dept is null) return NotFound();
            return View(dept);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int departmentId, string? departmentName, int facultyId)
        {
            string name = (departmentName ?? "").Trim();

            if (string.IsNullOrEmpty(name))
                ModelState.AddModelError("DepartmentName", "Bölüm adı zorunludur.");
            else if (facultyId <= 0)
                ModelState.AddModelError("FacultyId", "Fakülte seçimi zorunludur.");
            else if (clsDepartmentBusiness.IsDepartmentNameExistInFaculty(name, facultyId, departmentId))
                ModelState.AddModelError("DepartmentName", "Bu bölüm adı seçili fakültede zaten kayıtlı.");

            if (!ModelState.IsValid)
            {
                ViewBag.Faculties = clsFacultyBusiness.GetAllFaculties();
                clsDepartmentBusiness model;
                if (departmentId == -1)
                {
                    model = new clsDepartmentBusiness();
                }
                else
                {
                    var found = clsDepartmentBusiness.Find(departmentId);
                    if (found is null) return NotFound();
                    model = found;
                }
                model.DepartmentName = departmentName ?? "";
                model.FacultyId      = facultyId;
                return View(model);
            }

            clsDepartmentBusiness dept;
            if (departmentId == -1)
            {
                dept = new clsDepartmentBusiness();
            }
            else
            {
                var found = clsDepartmentBusiness.Find(departmentId);
                if (found is null) return NotFound();
                dept = found;
            }
            dept.DepartmentName = name;
            dept.FacultyId      = facultyId;

            if (dept.Save())
                TempData["Success"] = "Bölüm başarıyla kaydedildi.";
            else
                TempData["Error"] = "Kaydetme sırasında bir hata oluştu.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (clsDepartmentBusiness.DeleteDepartment(id))
                TempData["Success"] = "Bölüm silindi.";
            else
                TempData["Error"] = "Bölüm silinemedi.";

            return RedirectToAction("Index");
        }
    }
}
