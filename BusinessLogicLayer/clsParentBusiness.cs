using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsParentBusiness : clsParent
    {
        public static List<clsParent> GetByStudentFileId(int studentFileId) =>
            clsParentData.GetParentsByStudentFileId(studentFileId);

        public static clsParent? Find(int parentId) =>
            clsParentData.GetParentInfoByID(parentId);

        public static bool IsParentExist(int parentId) =>
            clsParentData.IsParentExist(parentId);
    }
}
