using EntityLayer;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public static class clsAdvisorData
    {
        private static clsAdvisor _MapAdvisor(SqlDataReader r) => new clsAdvisor
        {
            AdvisorId     = (int)r["AdvisorId"],
            UserId        = (int)r["UserId"],
            SicilNo       = (string)r["SicilNo"],
            AcademicTitle = r["AcademicTitle"] == DBNull.Value ? null : (string)r["AcademicTitle"],
            FirstName     = (string)r["FirstName"],
            LastName      = (string)r["LastName"],
            DepartmentId  = (int)r["DepartmentId"],
            Email         = r["Email"]    == DBNull.Value ? null : (string)r["Email"],
            Phone         = r["Phone"]    == DBNull.Value ? null : (string)r["Phone"],
            OfficeNo      = r["OfficeNo"] == DBNull.Value ? null : (string)r["OfficeNo"]
        };

        private static clsAdvisorInfo _MapInfo(SqlDataReader r) => new clsAdvisorInfo
        {
            AdvisorId     = (int)r["AdvisorId"],
            UserId        = (int)r["UserId"],
            SicilNo       = (string)r["SicilNo"],
            AcademicTitle = (string)r["AcademicTitle"],
            FirstName     = (string)r["FirstName"],
            LastName      = (string)r["LastName"],
            DepartmentId  = (int)r["DepartmentId"],
            DepartmentName= (string)r["DepartmentName"],
            Username      = (string)r["Username"],
            IsActive      = (bool)r["IsActive"]
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
                using SqlDataReader r = cmd.ExecuteReader();
                return r.Read() ? _MapAdvisor(r) : null;
            }
            catch { return null; }
        }

        public static int AddNewAdvisorWithUser(clsAdvisor advisor, clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdUser = new(@"
                        INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt, LastLoginAt)
                        VALUES (@Username, @PasswordHash, @RoleId, @IsActive, SYSDATETIME(), NULL);
                        SELECT SCOPE_IDENTITY();", conn, tx);
                    cmdUser.Parameters.AddWithValue("@Username",     user.Username);
                    cmdUser.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    cmdUser.Parameters.AddWithValue("@RoleId",       user.RoleId);
                    cmdUser.Parameters.AddWithValue("@IsActive",     user.IsActive);
                    object? userResult = cmdUser.ExecuteScalar();
                    if (userResult is null) { tx.Rollback(); return -1; }
                    int userId = Convert.ToInt32(userResult);
                    advisor.UserId = userId;

                    using SqlCommand cmdAdv = new(@"
                        INSERT INTO Advisors
                            (UserId, SicilNo, AcademicTitle, FirstName, LastName, DepartmentId, Email, Phone, OfficeNo)
                        VALUES
                            (@UserId, @SicilNo, @AcademicTitle, @FirstName, @LastName, @DepartmentId, @Email, @Phone, @OfficeNo);
                        SELECT SCOPE_IDENTITY();", conn, tx);
                    cmdAdv.Parameters.AddWithValue("@UserId",        userId);
                    cmdAdv.Parameters.AddWithValue("@SicilNo",       advisor.SicilNo);
                    cmdAdv.Parameters.AddWithValue("@AcademicTitle", (object?)advisor.AcademicTitle ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@FirstName",     advisor.FirstName);
                    cmdAdv.Parameters.AddWithValue("@LastName",      advisor.LastName);
                    cmdAdv.Parameters.AddWithValue("@DepartmentId",  advisor.DepartmentId);
                    cmdAdv.Parameters.AddWithValue("@Email",         (object?)advisor.Email    ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@Phone",         (object?)advisor.Phone    ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@OfficeNo",      (object?)advisor.OfficeNo ?? DBNull.Value);
                    object? advResult = cmdAdv.ExecuteScalar();
                    if (advResult is null) { tx.Rollback(); return -1; }

                    tx.Commit();
                    return Convert.ToInt32(advResult);
                }
                catch { tx.Rollback(); return -1; }
            }
            catch { return -1; }
        }

        public static bool UpdateAdvisorWithUser(clsAdvisor advisor, clsUser user)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdUser = new(
                        "UPDATE Users SET Username = @Username WHERE UserId = @UserId", conn, tx);
                    cmdUser.Parameters.AddWithValue("@Username", user.Username);
                    cmdUser.Parameters.AddWithValue("@UserId",   user.UserId);
                    int uRows = cmdUser.ExecuteNonQuery();

                    using SqlCommand cmdAdv = new(@"
                        UPDATE Advisors SET
                            SicilNo       = @SicilNo,
                            AcademicTitle = @AcademicTitle,
                            FirstName     = @FirstName,
                            LastName      = @LastName,
                            DepartmentId  = @DepartmentId,
                            Email         = @Email,
                            Phone         = @Phone,
                            OfficeNo      = @OfficeNo
                        WHERE AdvisorId = @AdvisorId", conn, tx);
                    cmdAdv.Parameters.AddWithValue("@SicilNo",       advisor.SicilNo);
                    cmdAdv.Parameters.AddWithValue("@AcademicTitle", (object?)advisor.AcademicTitle ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@FirstName",     advisor.FirstName);
                    cmdAdv.Parameters.AddWithValue("@LastName",      advisor.LastName);
                    cmdAdv.Parameters.AddWithValue("@DepartmentId",  advisor.DepartmentId);
                    cmdAdv.Parameters.AddWithValue("@Email",         (object?)advisor.Email    ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@Phone",         (object?)advisor.Phone    ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@OfficeNo",      (object?)advisor.OfficeNo ?? DBNull.Value);
                    cmdAdv.Parameters.AddWithValue("@AdvisorId",     advisor.AdvisorId);
                    int aRows = cmdAdv.ExecuteNonQuery();

                    if (uRows == 0 || aRows == 0) { tx.Rollback(); return false; }
                    tx.Commit();
                    return true;
                }
                catch { tx.Rollback(); return false; }
            }
            catch { return false; }
        }

        public static List<clsAdvisorInfo> GetAllAdvisors()
        {
            List<clsAdvisorInfo> list = new();
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
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read()) list.Add(_MapInfo(r));
            }
            catch { }
            return list;
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
            catch { return false; }
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
            catch { return false; }
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
            catch { return 0; }
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
            catch { return false; }
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
            catch { return false; }
        }
    }
}
