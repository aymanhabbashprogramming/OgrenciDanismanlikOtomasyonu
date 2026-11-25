using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsSurveyAnswerData
    {
        public static bool SubmitSurveyAnswers(int surveyId, int studentId, List<clsSurveyAnswer> answers)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdParticipation = new(@"
                        INSERT INTO SurveyParticipations (SurveyId, StudentId, ParticipatedAt)
                        VALUES (@SurveyId, @StudentId, SYSDATETIME())", conn, transaction);
                    cmdParticipation.Parameters.AddWithValue("@SurveyId",  surveyId);
                    cmdParticipation.Parameters.AddWithValue("@StudentId", studentId);
                    cmdParticipation.ExecuteNonQuery();

                    foreach (clsSurveyAnswer answer in answers)
                    {
                        using SqlCommand cmdAnswer = new(@"
                            INSERT INTO SurveyAnswers (SurveyQuestionId, Score, Comment)
                            VALUES (@SurveyQuestionId, @Score, @Comment)", conn, transaction);
                        cmdAnswer.Parameters.AddWithValue("@SurveyQuestionId", answer.SurveyQuestionId);
                        cmdAnswer.Parameters.AddWithValue("@Score",            (object?)answer.Score ?? DBNull.Value);
                        cmdAnswer.Parameters.AddWithValue("@Comment",          (object?)answer.Comment ?? DBNull.Value);
                        cmdAnswer.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsSurveyAnswerData.SubmitSurveyAnswers inner] {ex}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyAnswerData.SubmitSurveyAnswers outer] {ex}");
                return false;
            }
        }

        public static List<clsQuestionResultInfo> GetResultsBySurvey(int surveyId)
        {
            List<clsQuestionResultInfo> resultList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SurveyQuestion.SurveyQuestionId, SurveyQuestion.QuestionText,
                           SurveyQuestion.QuestionType,     SurveyQuestion.SortOrder,
                           SurveyAnswer.Score,              SurveyAnswer.Comment
                    FROM   SurveyQuestions AS SurveyQuestion
                    LEFT   JOIN SurveyAnswers AS SurveyAnswer
                           ON  SurveyAnswer.SurveyQuestionId = SurveyQuestion.SurveyQuestionId
                    WHERE  SurveyQuestion.SurveyId = @SurveyId
                    ORDER  BY SurveyQuestion.SortOrder, SurveyQuestion.SurveyQuestionId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                Dictionary<int, clsQuestionResultInfo> resultMap = new();

                while (reader.Read())
                {
                    int questionId = (int)reader["SurveyQuestionId"];

                    if (!resultMap.TryGetValue(questionId, out clsQuestionResultInfo? result))
                    {
                        result = new clsQuestionResultInfo
                        {
                            SurveyQuestionId  = questionId,
                            QuestionText      = (string)reader["QuestionText"],
                            QuestionType      = (string)reader["QuestionType"],
                            SortOrder         = (int)reader["SortOrder"],
                            ScoreDistribution = new int[10]
                        };
                        resultMap[questionId] = result;
                        resultList.Add(result);
                    }

                    if (reader["Score"] == DBNull.Value)
                        continue;

                    result.AnswerCount++;

                    if (result.QuestionType == "Puanlama")
                    {
                        int score = (int)reader["Score"];
                        if (score >= 1 && score <= 10)
                            result.ScoreDistribution[score - 1]++;
                    }
                    else
                    {
                        string? comment = reader["Comment"] == DBNull.Value ? null : (string)reader["Comment"];
                        if (!string.IsNullOrWhiteSpace(comment))
                            result.FreeTextAnswers.Add(comment);
                    }
                }

                foreach (clsQuestionResultInfo result in resultList)
                {
                    if (result.QuestionType == "Puanlama" && result.AnswerCount > 0)
                    {
                        int total = 0;
                        for (int index = 0; index < 10; index++)
                            total += result.ScoreDistribution[index] * (index + 1);
                        result.AverageScore = Math.Round((double)total / result.AnswerCount, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyAnswerData.GetResultsBySurvey] {ex}");
            }
            return resultList;
        }
    }
}
