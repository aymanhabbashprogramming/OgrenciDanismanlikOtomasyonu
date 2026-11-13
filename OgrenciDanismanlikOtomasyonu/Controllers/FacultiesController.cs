using BusinessLogicLayer;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OgrenciDanismanlikOtomasyonu.Controllers
{
    [Authorize(Roles = "DepartmentManager")]
    public class FacultiesController : Controller
    {
        public IActionResult Index()
        {
            return View(clsFacultyBusiness.GetAllFaculties());
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View(new clsFacultyBusiness());

            clsFacultyBusiness? faculty = clsFacultyBusiness.Find(id.Value);
            if (faculty is null) return NotFound();
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int facultyId, string? facultyName)
        {
            string name = (facultyName ?? "").Trim();

            if (string.IsNullOrEmpty(name))
                ModelState.AddModelError("FacultyName", "Fakülte adı zorunludur.");
            else if (clsFacultyBusiness.IsFacultyNameExist(name, facultyId))
                ModelState.AddModelError("FacultyName", "Bu fakülte adı zaten kayıtlı.");

            if (!ModelState.IsValid)
            {
                clsFacultyBusiness model;
                if (facultyId == -1)
                {
                    model = new clsFacultyBusiness();
                }
                else
                {
                    var found = clsFacultyBusiness.Find(facultyId);
                    if (found is null) return NotFound();
                    model = found;
                }
                model.FacultyName = facultyName ?? "";
                return View(model);
            }

            clsFacultyBusiness faculty;
            if (facultyId == -1)
            {
                faculty = new clsFacultyBusiness();
            }
            else
            {
                var found = clsFacultyBusiness.Find(facultyId);
                if (found is null) return NotFound();
                faculty = found;
            }
            faculty.FacultyName = name;

            if (faculty.Save())
                TempData["Success"] = "Fakülte başarıyla kaydedildi.";
            else
                TempData["Error"] = "Kaydetme sırasında bir hata oluştu.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (clsFacultyBusiness.DeleteFaculty(id))
                TempData["Success"] = "Fakülte silindi.";
            else
                TempData["Error"] = "Bu fakülteye bağlı bölümler bulunduğu için silinemez.";

            return RedirectToAction("Index");
        }
    }
}
