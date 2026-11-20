using DataAccessLayer;

namespace BusinessLogicLayer
{
    public static class clsDepartmentManagerBusiness
    {
        public static int GetManagerIdByUserId(int userId) =>
            clsDepartmentManagerData.GetManagerIdByUserId(userId);

        public static int GetDepartmentIdByUserId(int userId) =>
            clsDepartmentManagerData.GetDepartmentIdByUserId(userId);

        public static string GetFirstNameByUserId(int userId) =>
            clsDepartmentManagerData.GetFirstNameByUserId(userId);
    }
}
