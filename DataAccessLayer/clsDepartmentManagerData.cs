using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsDepartmentManagerData
    {
        public static clsProfileCardInfo? GetManagerProfileInfoByUserId(int userId)
        {
            const string query = @"
                SELECT
                    DepartmentManager.FirstName,
                    DepartmentManager.LastName,
                    DepartmentManager.Position,
                    DepartmentManager.Email,
                    DepartmentManager.Phone,
                    Department.DepartmentName,
                    Faculty.FacultyName,
                    ManagerUser.Username
                FROM DepartmentManagers AS DepartmentManager
                INNER JOIN Users       AS ManagerUser  ON ManagerUser.UserId        = DepartmentManager.UserId
                INNER JOIN Departments AS Department   ON Department.DepartmentId   = DepartmentManager.DepartmentId
                INNER JOIN Faculties   AS Faculty      ON Faculty.FacultyId         = Department.FacultyId
                WHERE DepartmentManager.UserId = @UserId";

            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null;

                return new clsProfileCardInfo
                {
                    FirstName      = reader["FirstName"]      == DBNull.Value ? string.Empty : (string)reader["FirstName"],
                    LastName       = reader["LastName"]       == DBNull.Value ? string.Empty : (string)reader["LastName"],
                    RoleName       = reader["Position"]       == DBNull.Value ? string.Empty : (string)reader["Position"],
                    Email          = reader["Email"]          == DBNull.Value ? null          : (string)reader["Email"],
                    Phone          = reader["Phone"]          == DBNull.Value ? null          : (string)reader["Phone"],
                    DepartmentName = reader["DepartmentName"] == DBNull.Value ? string.Empty : (string)reader["DepartmentName"],
                    FacultyName    = reader["FacultyName"]    == DBNull.Value ? string.Empty : (string)reader["FacultyName"],
                    Username       = reader["Username"]       == DBNull.Value ? string.Empty : (string)reader["Username"]
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetManagerProfileInfoByUserId] {ex.Message}");
                return null;
            }
        }

        public static int GetManagerIdByUserId(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT ManagerId FROM DepartmentManagers WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetManagerIdByUserId] {ex.Message}");
                return 0;
            }
        }

        public static int GetDepartmentIdByUserId(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT DepartmentId FROM DepartmentManagers WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetDepartmentIdByUserId] {ex.Message}");
                return 0;
            }
        }

        public static string GetFirstNameByUserId(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT FirstName FROM DepartmentManagers WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? (string)result : string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetFirstNameByUserId] {ex.Message}");
                return string.Empty;
            }
        }

        public static List<clsLabelCountRow> GetStudentCountsByEnrollmentStatusByDepartmentId(int departmentId)
        {
            const string query = @"
                SELECT ISNULL(CurrentEnrollment.Status, N'Belirtilmemiş') AS Label, COUNT(*) AS [Count]
                FROM Students AS Student
                INNER JOIN Users AS StudentUser
                    ON StudentUser.UserId = Student.UserId
                LEFT JOIN StudentEnrollmentStatuses AS CurrentEnrollment
                    ON CurrentEnrollment.StudentId = Student.StudentId AND CurrentEnrollment.EndDate IS NULL
                WHERE Student.DepartmentId = @DepartmentId AND StudentUser.IsActive = 1
                GROUP BY CurrentEnrollment.Status
                ORDER BY COUNT(*) DESC";

            List<clsLabelCountRow> result = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new clsLabelCountRow
                    {
                        Label = (string)reader["Label"],
                        Count = (int)reader["Count"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetStudentCountsByEnrollmentStatusByDepartmentId] {ex.Message}");
            }
            return result;
        }

        public static clsNationalitySummaryInfo GetNationalitySummaryByDepartmentId(int departmentId)
        {
            const string query = @"
                SELECT
                    Student.IsInternational,
                    CASE WHEN Student.IsInternational = 0 THEN N'Türkiye'
                         ELSE ISNULL(NationalityCountry.CountryName, N'Bilinmiyor') END AS Label,
                    COUNT(*) AS [Count]
                FROM Students AS Student
                INNER JOIN Users AS StudentUser
                    ON StudentUser.UserId = Student.UserId
                LEFT JOIN Countries AS NationalityCountry
                    ON Student.NationalityCountryId = NationalityCountry.CountryId
                WHERE Student.DepartmentId = @DepartmentId AND StudentUser.IsActive = 1
                GROUP BY Student.IsInternational, NationalityCountry.CountryName
                ORDER BY Student.IsInternational ASC, COUNT(*) DESC";

            clsNationalitySummaryInfo summary = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bool isInternational = (bool)reader["IsInternational"];
                    string label         = (string)reader["Label"];
                    int count            = (int)reader["Count"];

                    if (!isInternational)
                    {
                        summary.DomesticCount += count;
                    }
                    else
                    {
                        summary.InternationalCount += count;
                        summary.TopCountries.Add(new clsLabelCountRow { Label = label, Count = count });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetNationalitySummaryByDepartmentId] {ex.Message}");
            }
            return summary;
        }

        public static List<clsLabelCountRow> GetStudentCountsByClassYearByDepartmentId(int departmentId)
        {
            const string query = @"
                SELECT
                    CASE WHEN Student.ClassYear IS NULL THEN N'Belirtilmemiş'
                         ELSE CAST(Student.ClassYear AS NVARCHAR(10)) + N'. Sınıf' END AS Label,
                    COUNT(*) AS [Count]
                FROM Students AS Student
                INNER JOIN Users AS StudentUser
                    ON StudentUser.UserId = Student.UserId
                WHERE Student.DepartmentId = @DepartmentId AND StudentUser.IsActive = 1
                GROUP BY Student.ClassYear
                ORDER BY Student.ClassYear ASC";

            List<clsLabelCountRow> result = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new clsLabelCountRow
                    {
                        Label = (string)reader["Label"],
                        Count = (int)reader["Count"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetStudentCountsByClassYearByDepartmentId] {ex.Message}");
            }
            return result;
        }

        public static (int ActiveCount, int InactiveCount) GetAdvisorCountsByDepartmentId(int departmentId)
        {
            const string query = @"
                SELECT
                    SUM(CASE WHEN AdvisorUser.IsActive = 1 THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN AdvisorUser.IsActive = 0 THEN 1 ELSE 0 END) AS InactiveCount
                FROM Advisors AS Advisor
                INNER JOIN Users AS AdvisorUser ON AdvisorUser.UserId = Advisor.UserId
                WHERE Advisor.DepartmentId = @DepartmentId";

            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int active   = reader["ActiveCount"]   == DBNull.Value ? 0 : (int)reader["ActiveCount"];
                    int inactive = reader["InactiveCount"] == DBNull.Value ? 0 : (int)reader["InactiveCount"];
                    return (active, inactive);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentManagerData.GetAdvisorCountsByDepartmentId] {ex.Message}");
            }
            return (0, 0);
        }
    }
}
