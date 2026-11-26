using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsRequestTypeBusiness : clsRequestType
    {
        public static List<clsRequestType> GetAll() =>
            clsRequestTypeData.GetAll();

        public static clsRequestType? GetByFormCode(string formCode) =>
            clsRequestTypeData.GetByFormCode(formCode);
    }
}
