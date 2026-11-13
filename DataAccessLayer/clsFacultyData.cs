using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsFacultyData
    {
        public static clsFaculty? GetFacultyInfoByID(int facultyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT FacultyId, FacultyName FROM Faculties WHERE FacultyId = @FacultyId", conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                if (!r.Read()) return null;
                return new clsFaculty { FacultyId = (int)r["FacultyId"], FacultyName = (string)r["FacultyName"] };
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.GetFacultyInfoByID] {ex.Message}"); return null; }
        }

        public static int AddNewFaculty(clsFaculty faculty)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "INSERT INTO Faculties (FacultyName) VALUES (@FacultyName); SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@FacultyName", faculty.FacultyName);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.AddNewFaculty] {ex.Message}"); return -1; }
        }

        public static bool UpdateFaculty(clsFaculty faculty)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "UPDATE Faculties SET FacultyName = @FacultyName WHERE FacultyId = @FacultyId", conn);
                cmd.Parameters.AddWithValue("@FacultyId",   faculty.FacultyId);
                cmd.Parameters.AddWithValue("@FacultyName", faculty.FacultyName);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.UpdateFaculty] {ex.Message}"); return false; }
        }

        public static bool DeleteFaculty(int facultyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "DELETE FROM Faculties WHERE FacultyId = @FacultyId", conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.DeleteFaculty] {ex.Message}"); return false; }
        }

        public static List<clsFaculty> GetAllFaculties()
        {
            List<clsFaculty> list = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT FacultyId, FacultyName FROM Faculties ORDER BY FacultyName", conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new clsFaculty { FacultyId = (int)r["FacultyId"], FacultyName = (string)r["FacultyName"] });
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.GetAllFaculties] {ex.Message}"); }
            return list;
        }

        public static bool IsFacultyExist(int facultyId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Faculties WHERE FacultyId = @FacultyId", conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.IsFacultyExist] {ex.Message}"); return false; }
        }

        public static bool IsFacultyNameExist(string facultyName, int excludeId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Faculties WHERE FacultyName = @FacultyName AND FacultyId <> @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@FacultyName", facultyName);
                cmd.Parameters.AddWithValue("@ExcludeId",   excludeId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsFacultyData.IsFacultyNameExist] {ex.Message}"); return false; }
        }
    }
}
