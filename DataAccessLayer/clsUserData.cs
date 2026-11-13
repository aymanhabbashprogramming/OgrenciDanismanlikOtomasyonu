using EntityLayer;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public static class clsUserData
    {
        private static clsUser _Map(SqlDataReader r) => new clsUser
        {
            UserId       = (int)r["UserId"],
            Username     = (string)r["Username"],
            PasswordHash = (string)r["PasswordHash"],
            RoleId       = (int)r["RoleId"],
            IsActive     = (bool)r["IsActive"],
            CreatedAt    = (DateTime)r["CreatedAt"],
            LastLoginAt  = r["LastLoginAt"] == DBNull.Value ? null : (DateTime?)r["LastLoginAt"]
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
                using SqlDataReader r = cmd.ExecuteReader();
                return r.Read() ? _Map(r) : null;
            }
            catch { return null; }
        }

        public static clsUser? GetUserInfoByUsername(string username)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_selectAll + " WHERE Username = @Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                return r.Read() ? _Map(r) : null;
            }
            catch { return null; }
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
            catch { return -1; }
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
            catch { return false; }
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
            catch { return false; }
        }

        public static List<clsUser> GetAllUsers()
        {
            List<clsUser> list = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_selectAll + " ORDER BY UserId", conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read()) list.Add(_Map(r));
            }
            catch { }
            return list;
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
            catch { return false; }
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
            catch { return false; }
        }
    }
}
