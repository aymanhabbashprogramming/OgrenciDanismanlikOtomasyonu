using DataAccessLayer;
using EntityLayer;
using BC = BCrypt.Net.BCrypt;

namespace BusinessLogicLayer
{
    public class clsUserBusiness : clsUser
    {
        public enum enMode { AddNew, Update }
        public enum enRole { Student = 1, Advisor = 2, DepartmentManager = 3 }

        public enMode Mode { get; private set; }

        public clsUserBusiness()
        {
            UserId    = -1;
            IsActive  = true;
            CreatedAt = DateTime.Now;
            Mode      = enMode.AddNew;
        }

        private clsUserBusiness(clsUser u)
        {
            UserId       = u.UserId;
            Username     = u.Username;
            PasswordHash = u.PasswordHash;
            RoleId       = u.RoleId;
            IsActive     = u.IsActive;
            CreatedAt    = u.CreatedAt;
            LastLoginAt  = u.LastLoginAt;
            Mode         = enMode.Update;
        }

        public void SetPassword(string plainPassword)
        {
            PasswordHash = BC.HashPassword(plainPassword);
        }

        public static clsUserBusiness? Find(int userId)
        {
            clsUser? u = clsUserData.GetUserInfoByID(userId);
            return u is null ? null : new clsUserBusiness(u);
        }

        private bool _AddNewUser()
        {
            UserId = clsUserData.AddNewUser(this);
            return UserId != -1;
        }

        private bool _UpdateUser() => clsUserData.UpdateUser(this);

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser()) { Mode = enMode.Update; return true; }
                    return false;
                case enMode.Update:
                    return _UpdateUser();
                default:
                    return false;
            }
        }

        public static clsUserBusiness? Login(string username, string password)
        {
            clsUser? u = clsUserData.GetUserInfoByUsername(username);
            if (u is null || !u.IsActive) return null;
            if (!BC.Verify(password, u.PasswordHash)) return null;
            clsUserData.UpdateLastLogin(u.UserId);
            u.LastLoginAt = DateTime.Now;
            return new clsUserBusiness(u);
        }

        public static List<clsUser> GetAllUsers()  => clsUserData.GetAllUsers();
        public static bool DeleteUser(int userId)   => clsUserData.DeleteUser(userId);
        public static bool IsUserExist(int userId)  => clsUserData.IsUserExist(userId);
    }
}
