using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsSurveyBusiness : clsSurvey
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public clsSurveyBusiness()
        {
            SurveyId   = -1;
            SurveyType = "Genel";
            Status     = "Taslak";
            Mode       = enMode.AddNew;
        }

        private clsSurveyBusiness(clsSurvey survey)
        {
            SurveyId           = survey.SurveyId;
            DepartmentId       = survey.DepartmentId;
            SemesterId         = survey.SemesterId;
            Title              = survey.Title;
            Description        = survey.Description;
            SurveyType         = survey.SurveyType;
            Status             = survey.Status;
            EvaluatedAdvisorId = survey.EvaluatedAdvisorId;
            OpensAt            = survey.OpensAt;
            ClosesAt           = survey.ClosesAt;
            CreatedByManagerId = survey.CreatedByManagerId;
            Mode               = enMode.Update;
        }

        public static clsSurveyBusiness? Find(int surveyId)
        {
            clsSurvey? survey = clsSurveyData.GetSurveyInfoByID(surveyId);
            return survey is null ? null : new clsSurveyBusiness(survey);
        }

        private bool _AddNewSurvey()
        {
            SurveyId = clsSurveyData.AddNewSurvey(this);
            return SurveyId != -1;
        }

        private bool _UpdateSurvey() =>
            clsSurveyData.UpdateSurvey(this);

        public bool Save()
        {
            Title    = Title.Trim();
            OpensAt  = OpensAt.Date;
            ClosesAt = new DateTime(ClosesAt.Year, ClosesAt.Month, ClosesAt.Day, 23, 59, 59);

            if (string.IsNullOrEmpty(Title) || SemesterId <= 0 ||
                DepartmentId <= 0 || CreatedByManagerId <= 0 || ClosesAt <= OpensAt)
                return false;

            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewSurvey())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    if (Status != "Taslak") return false;
                    return _UpdateSurvey();
                default:
                    return false;
            }
        }

        public static bool Publish(int surveyId) =>
            clsSurveyData.PublishSurvey(surveyId);

        public static bool Delete(int surveyId) =>
            clsSurveyData.DeleteSurvey(surveyId);

        public static bool IsExist(int surveyId) =>
            clsSurveyData.IsSurveyExist(surveyId);

        public static List<clsSurveyInfo> GetAllByDepartment(int departmentId) =>
            clsSurveyData.GetAllSurveysByDepartment(departmentId);

        public static List<clsSurveyInfo> GetOpenByDepartment(int departmentId) =>
            clsSurveyData.GetOpenSurveysByDepartment(departmentId);
    }
}
