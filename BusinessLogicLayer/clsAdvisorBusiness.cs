using DataAccessLayer;
using EntityLayer;
using BC = BCrypt.Net.BCrypt;

namespace BusinessLogicLayer
{
    public class clsAdvisorBusiness : clsAdvisor
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public string Username { get; set; } = string.Empty;
        public bool   IsActive { get; set; } = true;
        private string _passwordHash = string.Empty;

        public bool HasPassword => !string.IsNullOrEmpty(_passwordHash);

        public void SetPassword(string plainPassword) =>
            _passwordHash = BC.HashPassword(plainPassword);

        public string GetFullName() => string.IsNullOrWhiteSpace(AcademicTitle)
            ? $"{FirstName} {LastName}".Trim()
            : $"{AcademicTitle} {FirstName} {LastName}".Trim();

        public clsAdvisorBusiness()
        {
            AdvisorId = -1;
            IsActive  = true;
            Mode      = enMode.AddNew;
        }

        private clsAdvisorBusiness(clsAdvisor advisor, string username, bool isActive)
        {
            AdvisorId     = advisor.AdvisorId;
            UserId        = advisor.UserId;
            SicilNo       = advisor.SicilNo;
            AcademicTitle = advisor.AcademicTitle;
            FirstName     = advisor.FirstName;
            LastName      = advisor.LastName;
            DepartmentId  = advisor.DepartmentId;
            Email         = advisor.Email;
            Phone         = advisor.Phone;
            OfficeNo      = advisor.OfficeNo;
            Username      = username;
            IsActive      = isActive;
            Mode          = enMode.Update;
        }

        public static clsAdvisorBusiness? Find(int advisorId)
        {
            clsAdvisor? advisor = clsAdvisorData.GetAdvisorInfoByID(advisorId);
            if (advisor is null) return null;
            clsUser? user = clsUserData.GetUserInfoByID(advisor.UserId);
            return new clsAdvisorBusiness(advisor, user?.Username ?? "", user?.IsActive ?? false);
        }

        private bool _AddNewAdvisor()
        {
            clsUser user = new()
            {
                Username     = Username,
                PasswordHash = _passwordHash,
                RoleId       = (int)clsUserBusiness.enRole.Advisor,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            };
            AdvisorId = clsAdvisorData.AddNewAdvisorWithUser(this, user);
            return AdvisorId != -1;
        }

        private bool _UpdateAdvisor()
        {
            clsUser user = new() { UserId = UserId, Username = Username };
            return clsAdvisorData.UpdateAdvisorWithUser(this, user);
        }

        public bool Save()
        {
            FirstName     = FirstName.Trim();
            LastName      = LastName.Trim();
            SicilNo       = SicilNo.Trim();
            Username      = Username.Trim();
            AcademicTitle = AcademicTitle?.Trim();

            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) ||
                string.IsNullOrEmpty(SicilNo)   || string.IsNullOrEmpty(Username) ||
                DepartmentId <= 0)
                return false;

            if (Mode == enMode.AddNew && !HasPassword) return false;

            if (clsAdvisorData.IsSicilNoExist(SicilNo, AdvisorId)) return false;

            clsUser? existingUser = clsUserData.GetUserInfoByUsername(Username);
            if (existingUser != null && existingUser.UserId != UserId) return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAdvisor())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateAdvisor();
                default:
                    return false;
            }
        }

        public static List<clsAdvisorInfo> GetAllAdvisors() =>
            clsAdvisorData.GetAllAdvisors();

        public static bool IsAdvisorExist(int advisorId) =>
            clsAdvisorData.IsAdvisorExist(advisorId);

        public static bool ResetPassword(int advisorId, string newPassword)
        {
            string hash = BC.HashPassword(newPassword);
            return clsAdvisorData.ResetPassword(advisorId, hash);
        }

        public static bool SetActive(int advisorId, bool isActive) =>
            clsAdvisorData.SetActive(advisorId, isActive);

        public static int GetActiveAdvisorCount() =>
            clsAdvisorData.GetActiveAdvisorCount();

        public static clsAdvisorBusiness? FindByUserId(int userId)
        {
            clsAdvisor? advisor = clsAdvisorData.GetAdvisorByUserId(userId);
            if (advisor is null) return null;
            clsUser? user = clsUserData.GetUserInfoByID(advisor.UserId);
            return new clsAdvisorBusiness(advisor, user?.Username ?? "", user?.IsActive ?? false);
        }

        public static List<clsAdvisorInfo> GetActiveAdvisorsForDepartment(int departmentId) =>
            clsAdvisorData.GetActiveAdvisorsByDepartmentId(departmentId);
    }
}
