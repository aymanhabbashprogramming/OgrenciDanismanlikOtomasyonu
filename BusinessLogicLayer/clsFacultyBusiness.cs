using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsFacultyBusiness : clsFaculty
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public clsFacultyBusiness()
        {
            FacultyId = -1;
            Mode      = enMode.AddNew;
        }

        private clsFacultyBusiness(clsFaculty faculty)
        {
            FacultyId   = faculty.FacultyId;
            FacultyName = faculty.FacultyName;
            Mode        = enMode.Update;
        }

        public static clsFacultyBusiness? Find(int facultyId)
        {
            clsFaculty? faculty = clsFacultyData.GetFacultyInfoByID(facultyId);
            return faculty is null ? null : new clsFacultyBusiness(faculty);
        }

        private bool _AddNewFaculty()
        {
            FacultyId = clsFacultyData.AddNewFaculty(this);
            return FacultyId != -1;
        }

        private bool _UpdateFaculty() => clsFacultyData.UpdateFaculty(this);

        public bool Save()
        {
            FacultyName = FacultyName.Trim();
            if (string.IsNullOrEmpty(FacultyName)) return false;
            if (clsFacultyData.IsFacultyNameExist(FacultyName, FacultyId)) return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewFaculty())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateFaculty();
                default:
                    return false;
            }
        }

        public static List<clsFaculty> GetAllFaculties() => clsFacultyData.GetAllFaculties();

        public static bool DeleteFaculty(int facultyId)
        {
            if (clsDepartmentData.GetDepartmentsByFacultyId(facultyId).Count > 0)
                return false;
            return clsFacultyData.DeleteFaculty(facultyId);
        }

        public static bool IsFacultyExist(int facultyId) => clsFacultyData.IsFacultyExist(facultyId);

        public static bool IsFacultyNameExist(string name, int excludeId)
            => clsFacultyData.IsFacultyNameExist(name, excludeId);
    }
}
