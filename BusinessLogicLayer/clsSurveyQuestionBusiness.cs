using DataAccessLayer;
using EntityLayer;

namespace BusinessLogicLayer
{
    public class clsSurveyQuestionBusiness : clsSurveyQuestion
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; private set; }

        public clsSurveyQuestionBusiness()
        {
            SurveyQuestionId = -1;
            QuestionType     = "Puanlama";
            Mode             = enMode.AddNew;
        }

        private clsSurveyQuestionBusiness(clsSurveyQuestion question)
        {
            SurveyQuestionId = question.SurveyQuestionId;
            SurveyId         = question.SurveyId;
            QuestionText     = question.QuestionText;
            QuestionType     = question.QuestionType;
            SortOrder        = question.SortOrder;
            Mode             = enMode.Update;
        }

        public static clsSurveyQuestionBusiness? Find(int questionId)
        {
            clsSurveyQuestion? question = clsSurveyQuestionData.GetSurveyQuestionInfoByID(questionId);
            return question is null ? null : new clsSurveyQuestionBusiness(question);
        }

        public bool Save()
        {
            QuestionText = QuestionText.Trim();

            if (string.IsNullOrEmpty(QuestionText) || SurveyId <= 0) return false;
            if (QuestionType != "Puanlama" && QuestionType != "SerbestMetin") return false;

            if (Mode == enMode.AddNew)
            {
                SortOrder = clsSurveyQuestionData.GetNextSortOrder(SurveyId);
                int newId = clsSurveyQuestionData.AddNewSurveyQuestion(this);
                if (newId == -1) return false;
                SurveyQuestionId = newId;
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        public static bool Delete(int questionId) =>
            clsSurveyQuestionData.DeleteSurveyQuestion(questionId);

        public static bool IsExist(int questionId) =>
            clsSurveyQuestionData.IsSurveyQuestionExist(questionId);

        public static List<clsSurveyQuestion> GetBySurvey(int surveyId) =>
            clsSurveyQuestionData.GetAllQuestionsBySurvey(surveyId);
    }
}
