using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsStudentFileBusiness : clsStudentFile
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public string? MobilePhone { get; set; }
        public string? Email       { get; set; }

        public clsParent Mother { get; set; } = new() { Relation = "Anne" };
        public clsParent Father { get; set; } = new() { Relation = "Baba" };

        public clsStudentFileBusiness()
        {
            StudentFileId = -1;
            Mode          = enMode.AddNew;
        }

        private clsStudentFileBusiness(
            clsStudentFile file,
            string?        mobilePhone,
            string?        email,
            clsParent      mother,
            clsParent      father)
        {
            StudentFileId          = file.StudentFileId;
            StudentId              = file.StudentId;
            FilledAt               = file.FilledAt;
            UpdatedAt              = file.UpdatedAt;
            BloodType              = file.BloodType;
            BirthPlace             = file.BirthPlace;
            BirthDate              = file.BirthDate;
            Nationality            = file.Nationality;
            PhotoPath              = file.PhotoPath;
            HousingType            = file.HousingType;
            HousingTypeDescription = file.HousingTypeDescription;
            Address                = file.Address;
            FamilyAddress          = file.FamilyAddress;
            FamilyPhone            = file.FamilyPhone;
            PreviousSchool         = file.PreviousSchool;
            HasDisability          = file.HasDisability;
            DisabilityRate         = file.DisabilityRate;
            DisabilityDescription  = file.DisabilityDescription;
            HasChronicIllness      = file.HasChronicIllness;
            ChronicIllnessDetails  = file.ChronicIllnessDetails;
            IsMartyrOrVeteranChild = file.IsMartyrOrVeteranChild;
            StayedInCareHome       = file.StayedInCareHome;
            Gano                   = file.Gano;
            ParentsStatus          = file.ParentsStatus;
            IsMotherDisabled       = file.IsMotherDisabled;
            IsFatherDisabled       = file.IsFatherDisabled;
            IsSiblingDisabled      = file.IsSiblingDisabled;
            FamilyMonthlyIncome    = file.FamilyMonthlyIncome;
            FamilyHomeType         = file.FamilyHomeType;
            OwnsSummerHouse        = file.OwnsSummerHouse;
            OwnsShop               = file.OwnsShop;
            OwnsRentalHouse        = file.OwnsRentalHouse;
            OwnsCar                = file.OwnsCar;
            SiblingsPrimary        = file.SiblingsPrimary;
            SiblingsHighSchool     = file.SiblingsHighSchool;
            SiblingsUniInCity      = file.SiblingsUniInCity;
            SiblingsUniOutCity     = file.SiblingsUniOutCity;
            ReceivesScholarship    = file.ReceivesScholarship;
            ScholarshipAmount      = file.ScholarshipAmount;
            ReceivesLoan           = file.ReceivesLoan;
            LoanAmount             = file.LoanAmount;
            IsWorking              = file.IsWorking;
            WorkIncome             = file.WorkIncome;

            MobilePhone = mobilePhone;
            Email       = email;
            Mother      = mother;
            Father      = father;
            Mode        = enMode.Update;
        }

        public static clsStudentFileBusiness? Find(int studentId)
        {
            clsStudent? student = clsStudentData.GetStudentInfoByID(studentId);
            if (student is null) return null;

            clsStudentFile? file = clsStudentFileData.GetStudentFileByStudentId(studentId);
            if (file is null)
            {
                return new clsStudentFileBusiness
                {
                    StudentId   = studentId,
                    MobilePhone = student.MobilePhone,
                    Email       = student.Email,
                    Mode        = enMode.AddNew
                };
            }

            List<clsParent> parents = clsParentData.GetParentsByStudentFileId(file.StudentFileId);
            clsParent mother = parents.FirstOrDefault(parent => parent.Relation == "Anne")
                               ?? new clsParent { Relation = "Anne" };
            clsParent father = parents.FirstOrDefault(parent => parent.Relation == "Baba")
                               ?? new clsParent { Relation = "Baba" };

            return new clsStudentFileBusiness(file, student.MobilePhone, student.Email, mother, father);
        }

        public static clsStudentFormInfo? GetStudentFormInfo(int studentId)
        {
            clsStudentFormInfo? info = clsStudentFileData.GetStudentFormInfo(studentId);
            if (info is null) return null;

            if (info.StudentFileId > 0)
            {
                List<clsParent> parents = clsParentData.GetParentsByStudentFileId(info.StudentFileId);
                info.Mother = parents.FirstOrDefault(parent => parent.Relation == "Anne")
                              ?? new clsParent { Relation = "Anne" };
                info.Father = parents.FirstOrDefault(parent => parent.Relation == "Baba")
                              ?? new clsParent { Relation = "Baba" };
            }

            return info;
        }

        public bool Save()
        {
            return clsStudentFileData.SaveStudentForm(StudentId, MobilePhone, Email, this, Mother, Father);
        }

        public static bool IsStudentFileExist(int studentId)
        {
            return clsStudentFileData.GetStudentFileByStudentId(studentId) is not null;
        }
    }
}
