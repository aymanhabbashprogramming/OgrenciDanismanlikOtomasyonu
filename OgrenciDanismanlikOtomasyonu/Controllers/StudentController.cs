using BusinessLogicLayer;
using EntityLayer;
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
            if (student is null) return NotFound();

            clsStudentHomeInfo? homeInfo =
                clsStudentBusiness.GetStudentHomeInfo(student.StudentId);
            if (homeInfo is null) return NotFound();

            return View(homeInfo);
        }

        [HttpGet]
        public IActionResult MyAdvisor()
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            clsStudentAdvisorPageInfo pageInfo =
                clsStudentAdvisorPageBusiness.GetAdvisorPageInfo(studentId);

            return View(pageInfo);
        }

        [HttpGet]
        public IActionResult MyInformation()
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            clsStudentFormInfo? formInfo = clsStudentFileBusiness.GetStudentFormInfo(studentId);
            if (formInfo is null) return NotFound();

            return View(formInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyInformation(
            int     studentFileId,
            string? mobilePhone,
            string? email,
            string? bloodType,
            string? birthPlace,
            string? birthDate,
            string? previousSchool,
            string? gano,
            string? housingType,
            string? housingTypeDescription,
            string? address,
            string? familyAddress,
            string? familyPhone,
            bool?   hasDisability,
            int?    disabilityRate,
            string? disabilityDescription,
            bool?   hasChronicIllness,
            string? chronicIllnessDetails,
            bool?   isMartyrOrVeteranChild,
            bool?   stayedInCareHome,
            string? motherFirstName,
            string? motherLastName,
            string? motherPhone,
            string? motherOccupation,
            bool?   motherHasSocialSecurity,
            string? motherWorkplace,
            string? motherMonthlyIncome,
            string? motherEducationLevel,
            bool?   motherIsAlive,
            int?    motherDeathYear,
            int?    motherParentId,
            string? fatherFirstName,
            string? fatherLastName,
            string? fatherPhone,
            string? fatherOccupation,
            bool?   fatherHasSocialSecurity,
            string? fatherWorkplace,
            string? fatherMonthlyIncome,
            string? fatherEducationLevel,
            bool?   fatherIsAlive,
            int?    fatherDeathYear,
            int?    fatherParentId,
            string? parentsStatus,
            bool    isMotherDisabled,
            bool    isFatherDisabled,
            bool    isSiblingDisabled,
            string? familyMonthlyIncome,
            string? familyHomeType,
            bool    ownsSummerHouse,
            bool    ownsShop,
            bool    ownsRentalHouse,
            bool    ownsCar,
            int?    siblingsPrimary,
            int?    siblingsHighSchool,
            int?    siblingsUniInCity,
            int?    siblingsUniOutCity,
            bool?   receivesScholarship,
            string? scholarshipAmount,
            bool?   receivesLoan,
            string? loanAmount,
            bool?   isWorking,
            string? workIncome,
            bool    declarationAccepted)
        {
            int studentId = _GetStudentId();
            if (studentId <= 0) return NotFound();

            if (!declarationAccepted)
            {
                TempData["Error"] = "Formu kaydetmek için beyanı onaylamanız gereklidir.";
                return RedirectToAction(nameof(MyInformation));
            }

            clsStudentFileBusiness form = new()
            {
                StudentFileId          = studentFileId,
                StudentId              = studentId,
                MobilePhone            = mobilePhone?.Trim(),
                Email                  = email?.Trim(),
                BloodType              = string.IsNullOrWhiteSpace(bloodType) ? null : bloodType.Trim(),
                BirthPlace             = string.IsNullOrWhiteSpace(birthPlace) ? null : birthPlace.Trim(),
                BirthDate              = DateOnly.TryParse(birthDate, out DateOnly parsedDate) ? parsedDate : null,
                PreviousSchool         = string.IsNullOrWhiteSpace(previousSchool) ? null : previousSchool.Trim(),
                Gano                   = decimal.TryParse(gano, System.Globalization.NumberStyles.Any,
                                                          System.Globalization.CultureInfo.InvariantCulture,
                                                          out decimal parsedGano) ? parsedGano : null,
                HousingType            = string.IsNullOrWhiteSpace(housingType) ? null : housingType,
                HousingTypeDescription = string.IsNullOrWhiteSpace(housingTypeDescription) ? null : housingTypeDescription.Trim(),
                Address                = string.IsNullOrWhiteSpace(address) ? null : address.Trim(),
                FamilyAddress          = string.IsNullOrWhiteSpace(familyAddress) ? null : familyAddress.Trim(),
                FamilyPhone            = string.IsNullOrWhiteSpace(familyPhone) ? null : familyPhone.Trim(),
                HasDisability          = hasDisability,
                DisabilityRate         = hasDisability == true ? disabilityRate : null,
                DisabilityDescription  = hasDisability == true && !string.IsNullOrWhiteSpace(disabilityDescription)
                                             ? disabilityDescription.Trim() : null,
                HasChronicIllness      = hasChronicIllness,
                ChronicIllnessDetails  = hasChronicIllness == true && !string.IsNullOrWhiteSpace(chronicIllnessDetails)
                                             ? chronicIllnessDetails.Trim() : null,
                IsMartyrOrVeteranChild = isMartyrOrVeteranChild,
                StayedInCareHome       = stayedInCareHome,
                Mother = new clsParent
                {
                    ParentId          = motherParentId ?? 0,
                    Relation          = "Anne",
                    FirstName         = string.IsNullOrWhiteSpace(motherFirstName) ? null : motherFirstName.Trim(),
                    LastName          = string.IsNullOrWhiteSpace(motherLastName)  ? null : motherLastName.Trim(),
                    Phone             = string.IsNullOrWhiteSpace(motherPhone) ? null : motherPhone.Trim(),
                    Occupation        = string.IsNullOrWhiteSpace(motherOccupation) ? null : motherOccupation.Trim(),
                    HasSocialSecurity = motherHasSocialSecurity,
                    Workplace         = string.IsNullOrWhiteSpace(motherWorkplace) ? null : motherWorkplace.Trim(),
                    MonthlyIncome     = decimal.TryParse(motherMonthlyIncome, System.Globalization.NumberStyles.Any,
                                                         System.Globalization.CultureInfo.InvariantCulture,
                                                         out decimal parsedMotherIncome) ? parsedMotherIncome : null,
                    EducationLevel    = string.IsNullOrWhiteSpace(motherEducationLevel) ? null : motherEducationLevel,
                    IsAlive           = motherIsAlive,
                    DeathYear         = motherIsAlive == false ? motherDeathYear : null
                },
                Father = new clsParent
                {
                    ParentId          = fatherParentId ?? 0,
                    Relation          = "Baba",
                    FirstName         = string.IsNullOrWhiteSpace(fatherFirstName) ? null : fatherFirstName.Trim(),
                    LastName          = string.IsNullOrWhiteSpace(fatherLastName)  ? null : fatherLastName.Trim(),
                    Phone             = string.IsNullOrWhiteSpace(fatherPhone) ? null : fatherPhone.Trim(),
                    Occupation        = string.IsNullOrWhiteSpace(fatherOccupation) ? null : fatherOccupation.Trim(),
                    HasSocialSecurity = fatherHasSocialSecurity,
                    Workplace         = string.IsNullOrWhiteSpace(fatherWorkplace) ? null : fatherWorkplace.Trim(),
                    MonthlyIncome     = decimal.TryParse(fatherMonthlyIncome, System.Globalization.NumberStyles.Any,
                                                         System.Globalization.CultureInfo.InvariantCulture,
                                                         out decimal parsedFatherIncome) ? parsedFatherIncome : null,
                    EducationLevel    = string.IsNullOrWhiteSpace(fatherEducationLevel) ? null : fatherEducationLevel,
                    IsAlive           = fatherIsAlive,
                    DeathYear         = fatherIsAlive == false ? fatherDeathYear : null
                },
                ParentsStatus       = string.IsNullOrWhiteSpace(parentsStatus) ? null : parentsStatus,
                IsMotherDisabled    = isMotherDisabled,
                IsFatherDisabled    = isFatherDisabled,
                IsSiblingDisabled   = isSiblingDisabled,
                FamilyMonthlyIncome = decimal.TryParse(familyMonthlyIncome, System.Globalization.NumberStyles.Any,
                                                        System.Globalization.CultureInfo.InvariantCulture,
                                                        out decimal parsedFamilyIncome) ? parsedFamilyIncome : null,
                FamilyHomeType      = string.IsNullOrWhiteSpace(familyHomeType) ? null : familyHomeType,
                OwnsSummerHouse     = ownsSummerHouse,
                OwnsShop            = ownsShop,
                OwnsRentalHouse     = ownsRentalHouse,
                OwnsCar             = ownsCar,
                SiblingsPrimary     = siblingsPrimary,
                SiblingsHighSchool  = siblingsHighSchool,
                SiblingsUniInCity   = siblingsUniInCity,
                SiblingsUniOutCity  = siblingsUniOutCity,
                ReceivesScholarship = receivesScholarship,
                ScholarshipAmount   = receivesScholarship == true
                                        ? (decimal.TryParse(scholarshipAmount, System.Globalization.NumberStyles.Any,
                                                            System.Globalization.CultureInfo.InvariantCulture,
                                                            out decimal parsedScholarship) ? parsedScholarship : null)
                                        : null,
                ReceivesLoan        = receivesLoan,
                LoanAmount          = receivesLoan == true
                                        ? (decimal.TryParse(loanAmount, System.Globalization.NumberStyles.Any,
                                                            System.Globalization.CultureInfo.InvariantCulture,
                                                            out decimal parsedLoan) ? parsedLoan : null)
                                        : null,
                IsWorking           = isWorking,
                WorkIncome          = isWorking == true
                                        ? (decimal.TryParse(workIncome, System.Globalization.NumberStyles.Any,
                                                            System.Globalization.CultureInfo.InvariantCulture,
                                                            out decimal parsedWorkIncome) ? parsedWorkIncome : null)
                                        : null
            };

            if (form.Save())
            {
                TempData["Success"] = "Bilgileriniz başarıyla kaydedildi.";
            }
            else
            {
                TempData["Error"] = "Bilgiler kaydedilirken bir hata oluştu. Lütfen tekrar deneyin.";
            }

            return RedirectToAction(nameof(MyInformation));
        }

        private int _GetStudentId()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            clsStudentBusiness? student = clsStudentBusiness.FindByUserId(userId);
            return student?.StudentId ?? 0;
        }
    }
}
