using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsRoleData
    {
        public static clsRole? GetRoleInfoByID(int roleId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand command = new SqlCommand("SELECT RoleId, RoleName FROM Roles WHERE RoleId = @RoleId", connection);
                command.Parameters.AddWithValue("@RoleId", roleId);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new clsRole
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = (string)reader["RoleName"]
                    };
                }
                return null;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsRoleData.GetRoleInfoByID] {ex.Message}"); return null; }
        }

        public static List<clsRole> GetAllRoles()
        {
            List<clsRole> roles = new List<clsRole>();
            try
            {
                using SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand command = new SqlCommand("SELECT RoleId, RoleName FROM Roles ORDER BY RoleId", connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(new clsRole
                    {
                        RoleId = (int)reader["RoleId"],
                        RoleName = (string)reader["RoleName"]
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"[clsRoleData.GetAllRoles] {ex.Message}"); }
            return roles;
        }

        public static bool IsRoleExist(int roleId)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
                using SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Roles WHERE RoleId = @RoleId", connection);
                command.Parameters.AddWithValue("@RoleId", roleId);
                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
            catch (Exception ex) { Debug.WriteLine($"[clsRoleData.IsRoleExist] {ex.Message}"); return false; }
        }
    }
}
