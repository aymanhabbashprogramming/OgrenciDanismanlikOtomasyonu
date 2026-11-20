using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsUserData
    {
        private static clsUser _MapUser(SqlDataReader reader) => new clsUser
        {
            UserId       = (int)reader["UserId"],
            Username     = (string)reader["Username"],
            PasswordHash = (string)reader["PasswordHash"],
            RoleId       = (int)reader["RoleId"],
            IsActive     = (bool)reader["IsActive"],
            CreatedAt    = (DateTime)reader["CreatedAt"],
            LastLoginAt  = reader["LastLoginAt"] == DBNull.Value ? null : (DateTime?)reader["LastLoginAt"]
        };

        private const string _selectAll =
            "SELECT UserId, Username, PasswordHash, RoleId, IsActive, CreatedAt, LastLoginAt FROM Users";

        public static clsUser? GetUserInfoByID(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_selectAll + " WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapUser(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return null;
            }
        }

        public static clsUser? GetUserInfoByUsername(string username)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_selectAll + " WHERE Username = @Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapUser(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return null;
            }
        }

        public static int AddNewUser(clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt, LastLoginAt)
                    VALUES (@Username, @PasswordHash, @RoleId, @IsActive, @CreatedAt, @LastLoginAt);
                    SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@Username",     user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId",       user.RoleId);
                cmd.Parameters.AddWithValue("@IsActive",     user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedAt",    user.CreatedAt);
                cmd.Parameters.AddWithValue("@LastLoginAt",  (object?)user.LastLoginAt ?? DBNull.Value);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? Convert.ToInt32(result) : -1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return -1;
            }
        }

        public static bool UpdateUser(clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Users SET
                        Username     = @Username,
                        PasswordHash = @PasswordHash,
                        RoleId       = @RoleId,
                        IsActive     = @IsActive
                    WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId",       user.UserId);
                cmd.Parameters.AddWithValue("@Username",     user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@RoleId",       user.RoleId);
                cmd.Parameters.AddWithValue("@IsActive",     user.IsActive);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return false;
            }
        }

        public static bool DeleteUser(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new("DELETE FROM Users WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return false;
            }
        }

        public static List<clsUser> GetAllUsers()
        {
            List<clsUser> userList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_selectAll + " ORDER BY UserId", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    userList.Add(_MapUser(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
            }
            return userList;
        }

        public static bool IsUserExist(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new("SELECT COUNT(*) FROM Users WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return false;
            }
        }

        public static bool UpdateLastLogin(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "UPDATE Users SET LastLoginAt = SYSDATETIME() WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsUserData] {ex.Message}");
                return false;
            }
        }
    }
}
