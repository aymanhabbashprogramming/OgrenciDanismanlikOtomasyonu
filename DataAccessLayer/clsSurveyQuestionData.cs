using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsSurveyQuestionData
    {
        private static clsSurveyQuestion _MapQuestion(SqlDataReader reader) => new clsSurveyQuestion
        {
            SurveyQuestionId = (int)reader["SurveyQuestionId"],
            SurveyId         = (int)reader["SurveyId"],
            QuestionText     = (string)reader["QuestionText"],
            QuestionType     = (string)reader["QuestionType"],
            SortOrder        = (int)reader["SortOrder"]
        };

        public static clsSurveyQuestion? GetSurveyQuestionInfoByID(int questionId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SurveyQuestionId, SurveyId, QuestionText, QuestionType, SortOrder
                    FROM   SurveyQuestions WHERE SurveyQuestionId = @QuestionId", conn);
                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapQuestion(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.GetSurveyQuestionInfoByID] {ex}");
                return null;
            }
        }

        public static int AddNewSurveyQuestion(clsSurveyQuestion question)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    INSERT INTO SurveyQuestions (SurveyId, QuestionText, QuestionType, SortOrder)
                    VALUES (@SurveyId, @QuestionText, @QuestionType, @SortOrder);
                    SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@SurveyId",     question.SurveyId);
                cmd.Parameters.AddWithValue("@QuestionText", question.QuestionText);
                cmd.Parameters.AddWithValue("@QuestionType", question.QuestionType);
                cmd.Parameters.AddWithValue("@SortOrder",    question.SortOrder);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is null ? -1 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.AddNewSurveyQuestion] {ex}");
                return -1;
            }
        }

        public static bool DeleteSurveyQuestion(int questionId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "DELETE FROM SurveyQuestions WHERE SurveyQuestionId = @QuestionId", conn);
                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.DeleteSurveyQuestion] {ex}");
                return false;
            }
        }

        public static List<clsSurveyQuestion> GetAllQuestionsBySurvey(int surveyId)
        {
            List<clsSurveyQuestion> questionList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SurveyQuestionId, SurveyId, QuestionText, QuestionType, SortOrder
                    FROM   SurveyQuestions
                    WHERE  SurveyId = @SurveyId
                    ORDER  BY SortOrder, SurveyQuestionId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    questionList.Add(_MapQuestion(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.GetAllQuestionsBySurvey] {ex}");
            }
            return questionList;
        }

        public static bool IsSurveyQuestionExist(int questionId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM SurveyQuestions WHERE SurveyQuestionId = @QuestionId", conn);
                cmd.Parameters.AddWithValue("@QuestionId", questionId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.IsSurveyQuestionExist] {ex}");
                return false;
            }
        }

        public static int GetNextSortOrder(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT ISNULL(MAX(SortOrder), 0) + 1 FROM SurveyQuestions WHERE SurveyId = @SurveyId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyQuestionData.GetNextSortOrder] {ex}");
                return 1;
            }
        }
    }
}
