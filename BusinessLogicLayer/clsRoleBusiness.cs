using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsRoleBusiness
    {
        public static List<clsRole> GetAllRoles()
        {
            return clsRoleData.GetAllRoles();
        }
    }
}