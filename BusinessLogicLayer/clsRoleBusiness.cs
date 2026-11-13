using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsRoleBusiness : clsRole
    {
        private clsRoleBusiness(clsRole role)
        {
            RoleId = role.RoleId;
            RoleName = role.RoleName;
        }

        public static clsRoleBusiness? Find(int roleId)
        {
            clsRole? role = clsRoleData.GetRoleInfoByID(roleId);
            return role is null ? null : new clsRoleBusiness(role);
        }

        public static List<clsRole> GetAllRoles() => clsRoleData.GetAllRoles();

        public static bool IsRoleExist(int roleId) => clsRoleData.IsRoleExist(roleId);
    }
}
