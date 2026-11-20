using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsSurveyData
    {
        private static clsSurvey _MapSurvey(SqlDataReader reader) => new clsSurvey
        {
            SurveyId           = (int)reader["SurveyId"],
            DepartmentId       = (int)reader["DepartmentId"],
            SemesterId         = (int)reader["SemesterId"],
            Title              = (string)reader["Title"],
            Description        = reader["Description"]        == DBNull.Value ? null : (string)reader["Description"],
            SurveyType         = (string)reader["SurveyType"],
            Status             = (string)reader["Status"],
            EvaluatedAdvisorId = reader["EvaluatedAdvisorId"] == DBNull.Value ? null : (int?)(int)reader["EvaluatedAdvisorId"],
            OpensAt            = (DateTime)reader["OpensAt"],
            ClosesAt           = (DateTime)reader["ClosesAt"],
            CreatedByManagerId = (int)reader["CreatedByManagerId"]
        };

        private static clsSurveyInfo _MapSurveyInfo(SqlDataReader reader) => new clsSurveyInfo
        {
            SurveyId           = (int)reader["SurveyId"],
            DepartmentId       = (int)reader["DepartmentId"],
            SemesterId         = (int)reader["SemesterId"],
            SemesterName       = (string)reader["SemesterName"],
            Title              = (string)reader["Title"],
            Description        = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
            Status             = (string)reader["Status"],
            OpensAt            = (DateTime)reader["OpensAt"],
            ClosesAt           = (DateTime)reader["ClosesAt"],
            QuestionCount      = (int)reader["QuestionCount"],
            ParticipationCount = (int)reader["ParticipationCount"]
        };

        private const string _infoSelect = @"
            SELECT Survey.SurveyId, Survey.DepartmentId, Survey.SemesterId,
                   Semester.AcademicYear + N' ' + Semester.Term AS SemesterName,
                   Survey.Title, Survey.Description, Survey.Status,
                   Survey.OpensAt, Survey.ClosesAt,
                   (SELECT COUNT(*) FROM SurveyQuestions    AS Question      WHERE Question.SurveyId      = Survey.SurveyId) AS QuestionCount,
                   (SELECT COUNT(*) FROM SurveyParticipations AS Participation WHERE Participation.SurveyId = Survey.SurveyId) AS ParticipationCount
            FROM   Surveys AS Survey
            INNER JOIN Semesters AS Semester ON Survey.SemesterId = Semester.SemesterId";

        public static clsSurvey? GetSurveyInfoByID(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SurveyId, DepartmentId, SemesterId, Title, Description, SurveyType, Status,
                           EvaluatedAdvisorId, OpensAt, ClosesAt, CreatedByManagerId
                    FROM   Surveys WHERE SurveyId = @SurveyId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapSurvey(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.GetSurveyInfoByID] {ex}");
                return null;
            }
        }

        public static clsSurveyInfo? GetSurveyInfoWithStatsByID(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + " WHERE Survey.SurveyId = @SurveyId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapSurveyInfo(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.GetSurveyInfoWithStatsByID] {ex}");
                return null;
            }
        }

        public static int AddNewSurvey(clsSurvey survey)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    INSERT INTO Surveys
                        (DepartmentId, SemesterId, Title, Description, SurveyType, Status,
                         EvaluatedAdvisorId, OpensAt, ClosesAt, CreatedByManagerId)
                    VALUES
                        (@DepartmentId, @SemesterId, @Title, @Description, @SurveyType, @Status,
                         @EvaluatedAdvisorId, @OpensAt, @ClosesAt, @CreatedByManagerId);
                    SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@DepartmentId",       survey.DepartmentId);
                cmd.Parameters.AddWithValue("@SemesterId",         survey.SemesterId);
                cmd.Parameters.AddWithValue("@Title",              survey.Title);
                cmd.Parameters.AddWithValue("@Description",        (object?)survey.Description        ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SurveyType",         survey.SurveyType);
                cmd.Parameters.AddWithValue("@Status",             survey.Status);
                cmd.Parameters.AddWithValue("@EvaluatedAdvisorId", (object?)survey.EvaluatedAdvisorId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OpensAt",            survey.OpensAt);
                cmd.Parameters.AddWithValue("@ClosesAt",           survey.ClosesAt);
                cmd.Parameters.AddWithValue("@CreatedByManagerId", survey.CreatedByManagerId);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is null ? -1 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.AddNewSurvey] {ex}");
                return -1;
            }
        }

        public static bool UpdateSurvey(clsSurvey survey)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Surveys SET
                        SemesterId  = @SemesterId,
                        Title       = @Title,
                        Description = @Description,
                        OpensAt     = @OpensAt,
                        ClosesAt    = @ClosesAt
                    WHERE SurveyId = @SurveyId AND Status = 'Taslak'", conn);
                cmd.Parameters.AddWithValue("@SemesterId",  survey.SemesterId);
                cmd.Parameters.AddWithValue("@Title",       survey.Title);
                cmd.Parameters.AddWithValue("@Description", (object?)survey.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OpensAt",     survey.OpensAt);
                cmd.Parameters.AddWithValue("@ClosesAt",    survey.ClosesAt);
                cmd.Parameters.AddWithValue("@SurveyId",    survey.SurveyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.UpdateSurvey] {ex}");
                return false;
            }
        }

        public static bool DeleteSurvey(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "DELETE FROM Surveys WHERE SurveyId = @SurveyId AND Status = 'Taslak'", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.DeleteSurvey] {ex}");
                return false;
            }
        }

        public static bool PublishSurvey(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Surveys SET Status = 'Yayınlandı'
                    WHERE  SurveyId = @SurveyId AND Status = 'Taslak'", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.PublishSurvey] {ex}");
                return false;
            }
        }

        public static bool IsSurveyExist(int surveyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Surveys WHERE SurveyId = @SurveyId", conn);
                cmd.Parameters.AddWithValue("@SurveyId", surveyId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.IsSurveyExist] {ex}");
                return false;
            }
        }

        public static List<clsSurveyInfo> GetAllSurveysByDepartment(int departmentId)
        {
            List<clsSurveyInfo> surveyList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    _infoSelect + " WHERE Survey.DepartmentId = @DepartmentId ORDER BY Survey.OpensAt DESC", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    surveyList.Add(_MapSurveyInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.GetAllSurveysByDepartment] {ex}");
            }
            return surveyList;
        }

        public static List<clsSurveyInfo> GetOpenSurveysByDepartment(int departmentId)
        {
            List<clsSurveyInfo> surveyList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    _infoSelect + @"
                    WHERE Survey.DepartmentId = @DepartmentId
                      AND Survey.Status       = 'Yayınlandı'
                      AND Survey.OpensAt      <= SYSDATETIME()
                      AND Survey.ClosesAt     >= SYSDATETIME()
                    ORDER BY Survey.ClosesAt ASC", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    surveyList.Add(_MapSurveyInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyData.GetOpenSurveysByDepartment] {ex}");
            }
            return surveyList;
        }
    }
}
