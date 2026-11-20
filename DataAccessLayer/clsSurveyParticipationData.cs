using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsSurveyParticipationData
    {
        public static bool HasStudentAnswered(int surveyId, int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM SurveyParticipations
                    WHERE SurveyId = @SurveyId AND StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@SurveyId",  surveyId);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyParticipationData.HasStudentAnswered] {ex}");
                return false;
            }
        }

        public static HashSet<int> GetAnsweredSurveyIds(int studentId)
        {
            HashSet<int> answeredIds = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT SurveyId FROM SurveyParticipations WHERE StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    answeredIds.Add((int)reader["SurveyId"]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyParticipationData.GetAnsweredSurveyIds] {ex}");
            }
            return answeredIds;
        }

        public static int GetAktifStudentCountByDepartment(int departmentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Students AS Student
                    INNER JOIN StudentEnrollmentStatuses AS EnrollmentStatus
                           ON  EnrollmentStatus.StudentId = Student.StudentId
                    WHERE Student.DepartmentId       = @DepartmentId
                      AND EnrollmentStatus.Status    = 'Aktif'
                      AND EnrollmentStatus.EndDate   IS NULL", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSurveyParticipationData.GetAktifStudentCountByDepartment] {ex}");
                return 0;
            }
        }
    }
}
