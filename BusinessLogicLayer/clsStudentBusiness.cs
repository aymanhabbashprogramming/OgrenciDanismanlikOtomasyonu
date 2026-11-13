using DataAccessLayer;
using EntityLayer;
using BC = BCrypt.Net.BCrypt;

namespace BusinessLogicLayer
{
    public class clsStudentBusiness : clsStudent
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public string Username { get; set; } = string.Empty;
        public bool   IsActive { get; set; } = true;
        private string _passwordHash = string.Empty;

        public bool HasPassword => !string.IsNullOrEmpty(_passwordHash);

        public void SetPassword(string plainPassword) =>
            _passwordHash = BC.HashPassword(plainPassword);

        public string GetFullName() => $"{FirstName} {LastName}".Trim();

        public clsStudentBusiness()
        {
            StudentId = -1;
            IsActive  = true;
            Mode      = enMode.AddNew;
        }

        private clsStudentBusiness(clsStudent s, string username, bool isActive)
        {
            StudentId       = s.StudentId;
            UserId          = s.UserId;
            StudentNumber   = s.StudentNumber;
            FirstName       = s.FirstName;
            LastName        = s.LastName;
            NationalId      = s.NationalId;
            DepartmentId    = s.DepartmentId;
            EducationType   = s.EducationType;
            MobilePhone     = s.MobilePhone;
            Email           = s.Email;
            ClassYear       = s.ClassYear;
            IsInternational = s.IsInternational;
            Username        = username;
            IsActive        = isActive;
            Mode            = enMode.Update;
        }

        public static clsStudentBusiness? Find(int studentId)
        {
            clsStudent? s = clsStudentData.GetStudentInfoByID(studentId);
            if (s is null) return null;
            clsUser? u = clsUserData.GetUserInfoByID(s.UserId);
            return new clsStudentBusiness(s, u?.Username ?? "", u?.IsActive ?? false);
        }

        public static clsStudentBusiness? FindByUserId(int userId)
        {
            clsStudent? s = clsStudentData.GetStudentByUserId(userId);
            if (s is null) return null;
            clsUser? u = clsUserData.GetUserInfoByID(s.UserId);
            return new clsStudentBusiness(s, u?.Username ?? "", u?.IsActive ?? false);
        }

        private bool _AddNewStudent()
        {
            clsUser user = new()
            {
                Username     = Username,
                PasswordHash = _passwordHash,
                RoleId       = (int)clsUserBusiness.enRole.Student,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };
            StudentId = clsStudentData.AddNewStudentWithUser(this, user);
            return StudentId != -1;
        }

        private bool _UpdateStudent()
        {
            clsUser user = new() { UserId = UserId, Username = Username };
            return clsStudentData.UpdateStudentWithUser(this, user);
        }

        public bool Save()
        {
            FirstName     = FirstName.Trim();
            LastName      = LastName.Trim();
            StudentNumber = StudentNumber.Trim();
            Username      = Username.Trim();
            NationalId    = NationalId.Trim();
            EducationType = EducationType.Trim();

            if (string.IsNullOrEmpty(FirstName)     || string.IsNullOrEmpty(LastName) ||
                string.IsNullOrEmpty(StudentNumber) || string.IsNullOrEmpty(Username) ||
                string.IsNullOrEmpty(EducationType) || DepartmentId <= 0)
                return false;

            if (Mode == enMode.AddNew && !HasPassword) return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent()) { Mode = enMode.Update; return true; }
                    return false;
                case enMode.Update:
                    return _UpdateStudent();
                default:
                    return false;
            }
        }

        public static List<clsStudentInfo> GetAllStudents() =>
            clsStudentData.GetAllStudents();

        public static List<clsStudentInfo> GetStudentsByAdvisorId(int advisorId) =>
            clsStudentData.GetStudentsByAdvisorId(advisorId);

        public static bool IsStudentExist(int studentId) =>
            clsStudentData.IsStudentExist(studentId);

        public static bool IsStudentNumberTaken(string number, int excludeStudentId) =>
            clsStudentData.IsStudentNumberExist(number, excludeStudentId);

        public static bool IsNationalIdTaken(string nationalId, int excludeStudentId) =>
            clsStudentData.IsNationalIdExist(nationalId, excludeStudentId);

        public static bool IsUsernameTaken(string username, int excludeUserId)
        {
            clsUser? existing = clsUserData.GetUserInfoByUsername(username);
            return existing != null && existing.UserId != excludeUserId;
        }

        public static bool AssignAdvisor(int studentId, int advisorId, string? changeReason = null)
        {
            clsAdvisorBusiness? advisor = clsAdvisorBusiness.Find(advisorId);
            if (advisor is null || !advisor.IsActive) return false;

            clsStudent? student = clsStudentData.GetStudentInfoByID(studentId);
            if (student is null) return false;

            if (advisor.DepartmentId != student.DepartmentId) return false;

            return clsStudentData.AssignAdvisor(studentId, advisorId, changeReason);
        }

        public static List<clsAdvisorAssignmentHistory> GetAdvisorHistory(int studentId) =>
            clsStudentData.GetAdvisorHistory(studentId);

        public static clsAdvisorInfo? GetCurrentAdvisor(int studentId) =>
            clsStudentData.GetCurrentAdvisorByStudentId(studentId);

        public static List<clsAdvisorInfo> GetActiveAdvisorsForDepartment(int departmentId) =>
            clsAdvisorData.GetActiveAdvisorsByDepartmentId(departmentId);

        public static bool ResetPassword(int studentId, string newPassword)
        {
            string hash = BC.HashPassword(newPassword);
            return clsStudentData.ResetPassword(studentId, hash);
        }

        public static bool SetActive(int studentId, bool isActive) =>
            clsStudentData.SetActive(studentId, isActive);

        public static int GetActiveStudentCount() =>
            clsStudentData.GetActiveStudentCount();
    }
}
