using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsRequestBusiness : clsRequest
    {
        public enum enMode { AddNew, Update }

        public enMode Mode { get; private set; }

        public List<clsRequestCourse> Courses { get; set; } = new();

        public clsRequestBusiness()
        {
            Mode      = enMode.AddNew;
            RequestId = -1;
        }

        private clsRequestBusiness(clsRequest request)
        {
            Mode              = enMode.Update;
            RequestId         = request.RequestId;
            StudentId         = request.StudentId;
            RequestTypeId     = request.RequestTypeId;
            AdvisorId         = request.AdvisorId;
            SemesterId        = request.SemesterId;
            SubmittedAt       = request.SubmittedAt;
            Reason            = request.Reason;
            DeclaredGano      = request.DeclaredGano;
            Excuse            = request.Excuse;
            ContactAddress    = request.ContactAddress;
            RejectionReason   = request.RejectionReason;
            DecidedAt         = request.DecidedAt;
        }

        public static clsRequestFormPreFillInfo? GetFormPreFillInfo(int studentId) =>
            clsRequestData.GetFormPreFillInfo(studentId);

        public static List<clsRequestInfo> GetMyRequests(int studentId) =>
            clsRequestData.GetRequestsByStudentId(studentId);

        public static clsRequestDetailInfo? GetRequestDetail(int requestId, int studentId) =>
            clsRequestData.GetRequestDetail(requestId, studentId);

        private int _AddNewRequest(int byUserId) =>
            clsRequestData.AddNewRequestAtomic(this, Courses, byUserId);

        public bool Save(int byUserId)
        {
            if (Mode == enMode.AddNew)
            {
                int newId = _AddNewRequest(byUserId);
                if (newId > 0)
                {
                    RequestId = newId;
                    Mode      = enMode.Update;
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
