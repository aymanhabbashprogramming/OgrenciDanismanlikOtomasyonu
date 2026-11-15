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
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new clsFaculty
                {
                    FacultyId   = (int)reader["FacultyId"],
                    FacultyName = (string)reader["FacultyName"]
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsFacultyData.GetFacultyInfoByID] {ex.Message}");
                return null;
            }
        }

        public static List<clsFaculty> GetAllFaculties()
        {
            List<clsFaculty> facultyList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT FacultyId, FacultyName FROM Faculties ORDER BY FacultyName", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    facultyList.Add(new clsFaculty
                    {
                        FacultyId   = (int)reader["FacultyId"],
                        FacultyName = (string)reader["FacultyName"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsFacultyData.GetAllFaculties] {ex.Message}");
            }
            return facultyList;
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsFacultyData.IsFacultyExist] {ex.Message}");
                return false;
            }
        }
    }
}
