using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public static class clsSurveyAnswerBusiness
    {
        public static bool SubmitAnswers(int surveyId, int studentId, List<clsSurveyAnswer> answers)
        {
            if (clsSurveyParticipationData.HasStudentAnswered(surveyId, studentId))
                return false;

            return clsSurveyAnswerData.SubmitSurveyAnswers(surveyId, studentId, answers);
        }

        public static clsSurveyResultInfo? GetResults(int surveyId)
        {
            clsSurveyInfo? surveyInfo = clsSurveyData.GetSurveyInfoWithStatsByID(surveyId);
            if (surveyInfo is null) return null;

            return new clsSurveyResultInfo
            {
                Survey            = surveyInfo,
                AktifStudentCount = clsSurveyParticipationData.GetAktifStudentCountByDepartment(surveyInfo.DepartmentId),
                QuestionResults   = clsSurveyAnswerData.GetResultsBySurvey(surveyId)
            };
        }
    }
}
