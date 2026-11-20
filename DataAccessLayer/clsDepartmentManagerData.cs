using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsDepartmentManagerData
    {
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
    }
}
