using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsStudentAdvisorPageData
    {
        public static clsStudentAdvisorPageInfo GetCurrentAdvisorWithStartDate(int studentId)
        {
            clsStudentAdvisorPageInfo pageInfo = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT Advisor.AdvisorId,
                           Advisor.AcademicTitle,
                           Advisor.FirstName,
                           Advisor.LastName,
                           Advisor.Email,
                           AdvisorAssignment.StartDate
                    FROM   AdvisorAssignments AS AdvisorAssignment
                    INNER JOIN Advisors AS Advisor ON AdvisorAssignment.AdvisorId = Advisor.AdvisorId
                    WHERE  AdvisorAssignment.StudentId = @StudentId
                      AND  AdvisorAssignment.EndDate   IS NULL", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    pageInfo.CurrentAdvisorId     = (int)reader["AdvisorId"];
                    pageInfo.AdvisorAcademicTitle = reader["AcademicTitle"] == DBNull.Value
                                                    ? null : (string)reader["AcademicTitle"];
                    pageInfo.AdvisorFirstName     = (string)reader["FirstName"];
                    pageInfo.AdvisorLastName      = (string)reader["LastName"];
                    pageInfo.AdvisorEmail         = reader["Email"] == DBNull.Value
                                                    ? null : (string)reader["Email"];
                    pageInfo.AssignmentStartDate  = DateOnly.FromDateTime((DateTime)reader["StartDate"]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentAdvisorPageData.GetCurrentAdvisorWithStartDate] {ex.Message}");
            }
            return pageInfo;
        }

        public static List<clsOfficeHour> GetOfficeHoursByAdvisorId(int advisorId, int semesterId)
        {
            List<clsOfficeHour> officeHourList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT OfficeHour.OfficeHourId, OfficeHour.AdvisorId, OfficeHour.DayOfWeek,
                           OfficeHour.StartTime, OfficeHour.EndTime, OfficeHour.Location, OfficeHour.SemesterId
                    FROM   OfficeHours AS OfficeHour
                    WHERE  OfficeHour.AdvisorId  = @AdvisorId
                      AND  OfficeHour.SemesterId = @SemesterId
                    ORDER BY
                        CASE OfficeHour.DayOfWeek
                            WHEN N'Pazartesi' THEN 1
                            WHEN N'Salı'      THEN 2
                            WHEN N'Çarşamba'  THEN 3
                            WHEN N'Perşembe'  THEN 4
                            WHEN N'Cuma'      THEN 5
                            ELSE 6
                        END,
                        OfficeHour.StartTime", conn);
                cmd.Parameters.AddWithValue("@AdvisorId",  advisorId);
                cmd.Parameters.AddWithValue("@SemesterId", semesterId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    officeHourList.Add(new clsOfficeHour
                    {
                        OfficeHourId = (int)reader["OfficeHourId"],
                        AdvisorId    = (int)reader["AdvisorId"],
                        DayOfWeek    = (string)reader["DayOfWeek"],
                        StartTime    = TimeOnly.FromTimeSpan((TimeSpan)reader["StartTime"]),
                        EndTime      = TimeOnly.FromTimeSpan((TimeSpan)reader["EndTime"]),
                        Location     = reader["Location"] == DBNull.Value ? null : (string)reader["Location"],
                        SemesterId   = (int)reader["SemesterId"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentAdvisorPageData.GetOfficeHoursByAdvisorId] {ex}");
            }
            return officeHourList;
        }
    }
}
