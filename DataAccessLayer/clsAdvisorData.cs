using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsAdvisorData
    {
        private static clsAdvisor _MapAdvisor(SqlDataReader reader) => new clsAdvisor
        {
            AdvisorId     = (int)reader["AdvisorId"],
            UserId        = (int)reader["UserId"],
            SicilNo       = (string)reader["SicilNo"],
            AcademicTitle = reader["AcademicTitle"] == DBNull.Value ? null : (string)reader["AcademicTitle"],
            FirstName     = (string)reader["FirstName"],
            LastName      = (string)reader["LastName"],
            DepartmentId  = (int)reader["DepartmentId"],
            Email         = reader["Email"]    == DBNull.Value ? null : (string)reader["Email"],
            Phone         = reader["Phone"]    == DBNull.Value ? null : (string)reader["Phone"],
            OfficeNo      = reader["OfficeNo"] == DBNull.Value ? null : (string)reader["OfficeNo"]
        };

        private static clsAdvisorInfo _MapInfo(SqlDataReader reader) => new clsAdvisorInfo
        {
            AdvisorId     = (int)reader["AdvisorId"],
            UserId        = (int)reader["UserId"],
            SicilNo       = (string)reader["SicilNo"],
            AcademicTitle = (string)reader["AcademicTitle"],
            FirstName     = (string)reader["FirstName"],
            LastName      = (string)reader["LastName"],
            DepartmentId  = (int)reader["DepartmentId"],
            DepartmentName= (string)reader["DepartmentName"],
            Username      = (string)reader["Username"],
            IsActive      = (bool)reader["IsActive"]
        };

        public static clsAdvisor? GetAdvisorInfoByID(int advisorId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT AdvisorId, UserId, SicilNo, AcademicTitle,
                           FirstName, LastName, DepartmentId, Email, Phone, OfficeNo
                    FROM Advisors WHERE AdvisorId = @AdvisorId", conn);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapAdvisor(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return null;
            }
        }

        public static int AddNewAdvisorWithUser(clsAdvisor advisor, clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdUser = new(@"
                        INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt, LastLoginAt)
                        VALUES (@Username, @PasswordHash, @RoleId, @IsActive, SYSDATETIME(), NULL);
                        SELECT SCOPE_IDENTITY();", conn, transaction);
                    cmdUser.Parameters.AddWithValue("@Username",     user.Username);
                    cmdUser.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmdUser.Parameters.AddWithValue("@RoleId",       user.RoleId);
                    cmdUser.Parameters.AddWithValue("@IsActive",     user.IsActive);
                    object? userResult = cmdUser.ExecuteScalar();
                    if (userResult is null)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                    int userId = Convert.ToInt32(userResult);
                    advisor.UserId = userId;

                    using SqlCommand cmdAdvisor = new(@"
                        INSERT INTO Advisors
                            (UserId, SicilNo, AcademicTitle, FirstName, LastName, DepartmentId, Email, Phone, OfficeNo)
                        VALUES
                            (@UserId, @SicilNo, @AcademicTitle, @FirstName, @LastName, @DepartmentId, @Email, @Phone, @OfficeNo);
                        SELECT SCOPE_IDENTITY();", conn, transaction);
                    cmdAdvisor.Parameters.AddWithValue("@UserId",        userId);
                    cmdAdvisor.Parameters.AddWithValue("@SicilNo",       advisor.SicilNo);
                    cmdAdvisor.Parameters.AddWithValue("@AcademicTitle", (object?)advisor.AcademicTitle ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@FirstName",     advisor.FirstName);
                    cmdAdvisor.Parameters.AddWithValue("@LastName",      advisor.LastName);
                    cmdAdvisor.Parameters.AddWithValue("@DepartmentId",  advisor.DepartmentId);
                    cmdAdvisor.Parameters.AddWithValue("@Email",         (object?)advisor.Email    ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@Phone",         (object?)advisor.Phone    ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@OfficeNo",      (object?)advisor.OfficeNo ?? DBNull.Value);
                    object? advisorResult = cmdAdvisor.ExecuteScalar();
                    if (advisorResult is null)
                    {
                        transaction.Rollback();
                        return -1;
                    }

                    transaction.Commit();
                    return Convert.ToInt32(advisorResult);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                    transaction.Rollback();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return -1;
            }
        }

        public static bool UpdateAdvisorWithUser(clsAdvisor advisor, clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdUser = new(
                        "UPDATE Users SET Username = @Username WHERE UserId = @UserId", conn, transaction);
                    cmdUser.Parameters.AddWithValue("@Username", user.Username);
                    cmdUser.Parameters.AddWithValue("@UserId",   user.UserId);
                    int userRows = cmdUser.ExecuteNonQuery();

                    using SqlCommand cmdAdvisor = new(@"
                        UPDATE Advisors SET
                            SicilNo       = @SicilNo,
                            AcademicTitle = @AcademicTitle,
                            FirstName     = @FirstName,
                            LastName      = @LastName,
                            DepartmentId  = @DepartmentId,
                            Email         = @Email,
                            Phone         = @Phone,
                            OfficeNo      = @OfficeNo
                        WHERE AdvisorId = @AdvisorId", conn, transaction);
                    cmdAdvisor.Parameters.AddWithValue("@SicilNo",       advisor.SicilNo);
                    cmdAdvisor.Parameters.AddWithValue("@AcademicTitle", (object?)advisor.AcademicTitle ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@FirstName",     advisor.FirstName);
                    cmdAdvisor.Parameters.AddWithValue("@LastName",      advisor.LastName);
                    cmdAdvisor.Parameters.AddWithValue("@DepartmentId",  advisor.DepartmentId);
                    cmdAdvisor.Parameters.AddWithValue("@Email",         (object?)advisor.Email    ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@Phone",         (object?)advisor.Phone    ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@OfficeNo",      (object?)advisor.OfficeNo ?? DBNull.Value);
                    cmdAdvisor.Parameters.AddWithValue("@AdvisorId",     advisor.AdvisorId);
                    int advisorRows = cmdAdvisor.ExecuteNonQuery();

                    if (userRows == 0 || advisorRows == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return false;
            }
        }

        public static List<clsAdvisorInfo> GetAllAdvisors()
        {
            List<clsAdvisorInfo> advisorList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT a.AdvisorId, a.UserId, a.SicilNo,
                           ISNULL(a.AcademicTitle, '') AS AcademicTitle,
                           a.FirstName, a.LastName, a.DepartmentId,
                           d.DepartmentName, u.Username, u.IsActive
                    FROM   Advisors a
                    INNER JOIN Users       u ON a.UserId       = u.UserId
                    INNER JOIN Departments d ON a.DepartmentId = d.DepartmentId
                    ORDER BY a.LastName, a.FirstName", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    advisorList.Add(_MapInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
            }
            return advisorList;
        }

        public static bool IsAdvisorExist(int advisorId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Advisors WHERE AdvisorId = @AdvisorId", conn);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return false;
            }
        }

        public static bool IsSicilNoExist(string sicilNo, int excludeAdvisorId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Advisors
                    WHERE SicilNo = @SicilNo AND AdvisorId <> @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@SicilNo",   sicilNo);
                cmd.Parameters.AddWithValue("@ExcludeId", excludeAdvisorId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return false;
            }
        }

        public static int GetActiveAdvisorCount()
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Advisors a
                    INNER JOIN Users u ON a.UserId = u.UserId
                    WHERE u.IsActive = 1", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return 0;
            }
        }

        public static bool ResetPassword(int advisorId, string passwordHash)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Users SET PasswordHash = @PasswordHash
                    WHERE UserId = (SELECT UserId FROM Advisors WHERE AdvisorId = @AdvisorId)", conn);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@AdvisorId",    advisorId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return false;
            }
        }

        public static bool SetActive(int advisorId, bool isActive)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Users SET IsActive = @IsActive
                    WHERE UserId = (SELECT UserId FROM Advisors WHERE AdvisorId = @AdvisorId)", conn);
                cmd.Parameters.AddWithValue("@IsActive",  isActive);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return false;
            }
        }

        public static clsAdvisor? GetAdvisorByUserId(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT AdvisorId, UserId, SicilNo, AcademicTitle,
                           FirstName, LastName, DepartmentId, Email, Phone, OfficeNo
                    FROM Advisors WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapAdvisor(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
                return null;
            }
        }

        public static List<clsAdvisorInfo> GetActiveAdvisorsByDepartmentId(int departmentId)
        {
            List<clsAdvisorInfo> advisorList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT a.AdvisorId, a.UserId, a.SicilNo,
                           ISNULL(a.AcademicTitle, '') AS AcademicTitle,
                           a.FirstName, a.LastName, a.DepartmentId,
                           d.DepartmentName, u.Username, u.IsActive
                    FROM   Advisors a
                    INNER JOIN Users       u ON a.UserId       = u.UserId
                    INNER JOIN Departments d ON a.DepartmentId = d.DepartmentId
                    WHERE  a.DepartmentId = @DepartmentId AND u.IsActive = 1
                    ORDER BY a.LastName, a.FirstName", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    advisorList.Add(_MapInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsAdvisorData] {ex.Message}");
            }
            return advisorList;
        }
    }
}
