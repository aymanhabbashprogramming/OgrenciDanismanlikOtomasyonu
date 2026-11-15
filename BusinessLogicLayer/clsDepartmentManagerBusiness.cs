using DataAccessLayer;

namespace BusinessLogicLayer
{
    public static class clsDepartmentManagerBusiness
    {
        public static int GetDepartmentIdByUserId(int userId)
            => clsDepartmentManagerData.GetDepartmentIdByUserId(userId);
    }
}
