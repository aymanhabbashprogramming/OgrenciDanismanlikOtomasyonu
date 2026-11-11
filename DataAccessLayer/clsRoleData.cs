using EntityLayer;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public static class clsRoleData
    {
        public static List<clsRole> GetAllRoles()
        {
            List<clsRole> roles = new List<clsRole>();

            string query = "SELECT RoleId, RoleName FROM Roles ORDER BY RoleId";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clsRole role = new clsRole
                        {
                            RoleId = (int)reader["RoleId"],
                            RoleName = (string)reader["RoleName"]
                        };

                        roles.Add(role);
                    }
                }
            }

            return roles;
        }


    }
}