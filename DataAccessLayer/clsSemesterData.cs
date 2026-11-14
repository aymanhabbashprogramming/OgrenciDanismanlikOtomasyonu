using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsSemesterData
    {
        private static clsSemester _MapSemester(SqlDataReader reader) => new clsSemester
        {
            SemesterId   = (int)reader["SemesterId"],
            AcademicYear = (string)reader["AcademicYear"],
            Term         = (string)reader["Term"],
            StartDate    = DateOnly.FromDateTime((DateTime)reader["StartDate"]),
            EndDate      = DateOnly.FromDateTime((DateTime)reader["EndDate"]),
            IsCurrent    = (bool)reader["IsCurrent"]
        };

        public static clsSemester? GetSemesterInfoByID(int semesterId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SemesterId, AcademicYear, Term, StartDate, EndDate, IsCurrent
                    FROM Semesters WHERE SemesterId = @SemesterId", conn);
                cmd.Parameters.AddWithValue("@SemesterId", semesterId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapSemester(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSemesterData.GetSemesterInfoByID] {ex}");
                return null;
            }
        }

        public static List<clsSemester> GetAllSemesters()
        {
            List<clsSemester> semesterList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT SemesterId, AcademicYear, Term, StartDate, EndDate, IsCurrent
                    FROM Semesters
                    ORDER BY StartDate DESC", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    semesterList.Add(_MapSemester(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSemesterData.GetAllSemesters] {ex}");
            }
            return semesterList;
        }

        public static bool IsSemesterExist(int semesterId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Semesters WHERE SemesterId = @SemesterId", conn);
                cmd.Parameters.AddWithValue("@SemesterId", semesterId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSemesterData.IsSemesterExist] {ex}");
                return false;
            }
        }

        public static clsSemester? GetCurrentSemester()
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT TOP 1 SemesterId, AcademicYear, Term, StartDate, EndDate, IsCurrent
                    FROM Semesters
                    WHERE StartDate <= CAST(GETDATE() AS DATE)
                      AND EndDate   >= CAST(GETDATE() AS DATE)
                    ORDER BY StartDate DESC", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapSemester(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsSemesterData.GetCurrentSemester] {ex}");
                return null;
            }
        }
    }
}
