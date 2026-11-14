using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsStudentData
    {
        private static clsStudent _MapStudent(SqlDataReader reader) => new clsStudent
        {
            StudentId       = (int)reader["StudentId"],
            UserId          = (int)reader["UserId"],
            StudentNumber   = (string)reader["StudentNumber"],
            FirstName       = (string)reader["FirstName"],
            LastName        = (string)reader["LastName"],
            NationalId      = (string)reader["NationalId"],
            DepartmentId    = (int)reader["DepartmentId"],
            EducationType   = (string)reader["EducationType"],
            MobilePhone     = reader["MobilePhone"] == DBNull.Value ? null : (string)reader["MobilePhone"],
            Email           = reader["Email"]        == DBNull.Value ? null : (string)reader["Email"],
            ClassYear       = reader["ClassYear"]    == DBNull.Value ? null : (int?)(int)reader["ClassYear"],
            IsInternational = (bool)reader["IsInternational"]
        };

        private static clsStudentInfo _MapStudentInfo(SqlDataReader reader) => new clsStudentInfo
        {
            StudentId        = (int)reader["StudentId"],
            UserId           = (int)reader["UserId"],
            StudentNumber    = (string)reader["StudentNumber"],
            FirstName        = (string)reader["FirstName"],
            LastName         = (string)reader["LastName"],
            NationalId       = (string)reader["NationalId"],
            DepartmentId     = (int)reader["DepartmentId"],
            DepartmentName   = (string)reader["DepartmentName"],
            EducationType    = (string)reader["EducationType"],
            MobilePhone      = reader["MobilePhone"] == DBNull.Value ? null : (string)reader["MobilePhone"],
            Email            = reader["Email"]        == DBNull.Value ? null : (string)reader["Email"],
            ClassYear        = reader["ClassYear"]    == DBNull.Value ? null : (int?)(int)reader["ClassYear"],
            IsInternational  = (bool)reader["IsInternational"],
            Username         = (string)reader["Username"],
            IsActive         = (bool)reader["IsActive"],
            CurrentAdvisorId = reader["CurrentAdvisorId"] == DBNull.Value ? null : (int?)(int)reader["CurrentAdvisorId"],
            AdvisorFullName  = reader["AdvisorFullName"]  == DBNull.Value ? null : (string)reader["AdvisorFullName"]
        };

        private const string _infoSelect = @"
            SELECT s.StudentId, s.UserId, s.StudentNumber, s.FirstName, s.LastName, s.NationalId,
                   s.DepartmentId, d.DepartmentName, s.EducationType, s.MobilePhone, s.Email,
                   s.ClassYear, s.IsInternational, u.Username, u.IsActive,
                   aa.AdvisorId AS CurrentAdvisorId,
                   CASE WHEN a.AdvisorId IS NULL THEN NULL
                        ELSE ISNULL(a.AcademicTitle + ' ', '') + a.FirstName + ' ' + a.LastName
                   END AS AdvisorFullName
            FROM   Students s
            INNER JOIN Users       u  ON s.UserId       = u.UserId
            INNER JOIN Departments d  ON s.DepartmentId = d.DepartmentId
            LEFT  JOIN AdvisorAssignments aa ON aa.StudentId = s.StudentId AND aa.EndDate IS NULL
            LEFT  JOIN Advisors    a  ON aa.AdvisorId   = a.AdvisorId";

        public static clsStudent? GetStudentInfoByID(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT StudentId, UserId, StudentNumber, FirstName, LastName, NationalId,
                           DepartmentId, EducationType, MobilePhone, Email, ClassYear, IsInternational
                    FROM Students WHERE StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapStudent(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetStudentInfoByID] {ex}");
                return null;
            }
        }

        public static clsStudent? GetStudentByUserId(int userId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT StudentId, UserId, StudentNumber, FirstName, LastName, NationalId,
                           DepartmentId, EducationType, MobilePhone, Email, ClassYear, IsInternational
                    FROM Students WHERE UserId = @UserId", conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapStudent(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetStudentByUserId] {ex}");
                return null;
            }
        }

        public static int AddNewStudentWithUser(clsStudent student, clsUser user)
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
                    student.UserId = userId;

                    using SqlCommand cmdStudent = new(@"
                        INSERT INTO Students
                            (UserId, StudentNumber, FirstName, LastName, NationalId,
                             DepartmentId, EducationType, MobilePhone, Email, ClassYear, IsInternational)
                        VALUES
                            (@UserId, @StudentNumber, @FirstName, @LastName, @NationalId,
                             @DepartmentId, @EducationType, @MobilePhone, @Email, @ClassYear, @IsInternational);
                        SELECT SCOPE_IDENTITY();", conn, transaction);
                    cmdStudent.Parameters.AddWithValue("@UserId",          userId);
                    cmdStudent.Parameters.AddWithValue("@StudentNumber",   student.StudentNumber);
                    cmdStudent.Parameters.AddWithValue("@FirstName",       student.FirstName);
                    cmdStudent.Parameters.AddWithValue("@LastName",        student.LastName);
                    cmdStudent.Parameters.AddWithValue("@NationalId",      student.NationalId);
                    cmdStudent.Parameters.AddWithValue("@DepartmentId",    student.DepartmentId);
                    cmdStudent.Parameters.AddWithValue("@EducationType",   student.EducationType);
                    cmdStudent.Parameters.AddWithValue("@MobilePhone",     (object?)student.MobilePhone  ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@Email",           (object?)student.Email        ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@ClassYear",       (object?)student.ClassYear    ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@IsInternational", student.IsInternational);
                    object? studentResult = cmdStudent.ExecuteScalar();
                    if (studentResult is null)
                    {
                        transaction.Rollback();
                        return -1;
                    }
                    int studentId = Convert.ToInt32(studentResult);

                    using SqlCommand cmdEnroll = new(@"
                        INSERT INTO StudentEnrollmentStatuses (StudentId, Status, StartDate, EndDate, Note)
                        VALUES (@StudentId, 'Aktif', CAST(GETDATE() AS DATE), NULL, NULL);", conn, transaction);
                    cmdEnroll.Parameters.AddWithValue("@StudentId", studentId);
                    cmdEnroll.ExecuteNonQuery();

                    transaction.Commit();
                    return studentId;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsStudentData.AddNewStudentWithUser inner] {ex}");
                    transaction.Rollback();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.AddNewStudentWithUser outer] {ex}");
                return -1;
            }
        }

        public static bool UpdateStudentWithUser(clsStudent student, clsUser user)
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
                    cmdUser.ExecuteNonQuery();

                    using SqlCommand cmdStudent = new(@"
                        UPDATE Students SET
                            StudentNumber   = @StudentNumber,
                            FirstName       = @FirstName,
                            LastName        = @LastName,
                            NationalId      = @NationalId,
                            DepartmentId    = @DepartmentId,
                            EducationType   = @EducationType,
                            MobilePhone     = @MobilePhone,
                            Email           = @Email,
                            ClassYear       = @ClassYear,
                            IsInternational = @IsInternational
                        WHERE StudentId = @StudentId", conn, transaction);
                    cmdStudent.Parameters.AddWithValue("@StudentNumber",   student.StudentNumber);
                    cmdStudent.Parameters.AddWithValue("@FirstName",       student.FirstName);
                    cmdStudent.Parameters.AddWithValue("@LastName",        student.LastName);
                    cmdStudent.Parameters.AddWithValue("@NationalId",      student.NationalId);
                    cmdStudent.Parameters.AddWithValue("@DepartmentId",    student.DepartmentId);
                    cmdStudent.Parameters.AddWithValue("@EducationType",   student.EducationType);
                    cmdStudent.Parameters.AddWithValue("@MobilePhone",     (object?)student.MobilePhone  ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@Email",           (object?)student.Email        ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@ClassYear",       (object?)student.ClassYear    ?? DBNull.Value);
                    cmdStudent.Parameters.AddWithValue("@IsInternational", student.IsInternational);
                    cmdStudent.Parameters.AddWithValue("@StudentId",       student.StudentId);
                    int rowsAffected = cmdStudent.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsStudentData.UpdateStudentWithUser inner] {ex}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.UpdateStudentWithUser outer] {ex}");
                return false;
            }
        }

        public static List<clsStudentInfo> GetAllStudents()
        {
            List<clsStudentInfo> studentList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + " ORDER BY s.LastName, s.FirstName", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    studentList.Add(_MapStudentInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetAllStudents] {ex}");
            }
            return studentList;
        }

        public static List<clsStudentInfo> GetStudentsByAdvisorId(int advisorId)
        {
            List<clsStudentInfo> studentList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + @"
                    WHERE aa.AdvisorId = @AdvisorId
                    ORDER BY s.LastName, s.FirstName", conn);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    studentList.Add(_MapStudentInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetStudentsByAdvisorId] {ex}");
            }
            return studentList;
        }

        public static bool IsStudentExist(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Students WHERE StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.IsStudentExist] {ex}");
                return false;
            }
        }

        public static bool IsStudentNumberExist(string studentNumber, int excludeStudentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Students
                    WHERE StudentNumber = @StudentNumber AND StudentId <> @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@StudentNumber", studentNumber);
                cmd.Parameters.AddWithValue("@ExcludeId",     excludeStudentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.IsStudentNumberExist] {ex}");
                return false;
            }
        }

        public static bool IsNationalIdExist(string nationalId, int excludeStudentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Students
                    WHERE NationalId = @NationalId AND NationalId <> '' AND StudentId <> @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@NationalId", nationalId);
                cmd.Parameters.AddWithValue("@ExcludeId",  excludeStudentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.IsNationalIdExist] {ex}");
                return false;
            }
        }

        public static bool AssignAdvisor(int studentId, int advisorId, string? changeReason)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmdClose = new(@"
                        UPDATE AdvisorAssignments
                        SET EndDate = CAST(GETDATE() AS DATE)
                        WHERE StudentId = @StudentId AND EndDate IS NULL", conn, transaction);
                    cmdClose.Parameters.AddWithValue("@StudentId", studentId);
                    cmdClose.ExecuteNonQuery();

                    using SqlCommand cmdInsert = new(@"
                        INSERT INTO AdvisorAssignments (StudentId, AdvisorId, StartDate, EndDate, ChangeReason)
                        VALUES (@StudentId, @AdvisorId, CAST(GETDATE() AS DATE), NULL, @ChangeReason)", conn, transaction);
                    cmdInsert.Parameters.AddWithValue("@StudentId",    studentId);
                    cmdInsert.Parameters.AddWithValue("@AdvisorId",    advisorId);
                    cmdInsert.Parameters.AddWithValue("@ChangeReason", (object?)changeReason ?? DBNull.Value);
                    cmdInsert.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsStudentData.AssignAdvisor inner] {ex}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.AssignAdvisor outer] {ex}");
                return false;
            }
        }

        public static List<clsAdvisorAssignmentHistory> GetAdvisorHistory(int studentId)
        {
            List<clsAdvisorAssignmentHistory> historyList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT aa.AssignmentId, aa.AdvisorId,
                           ISNULL(a.AcademicTitle + ' ', '') + a.FirstName + ' ' + a.LastName AS AdvisorFullName,
                           aa.StartDate, aa.EndDate, aa.ChangeReason
                    FROM   AdvisorAssignments aa
                    INNER JOIN Advisors a ON aa.AdvisorId = a.AdvisorId
                    WHERE  aa.StudentId = @StudentId
                    ORDER BY aa.StartDate DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    historyList.Add(new clsAdvisorAssignmentHistory
                    {
                        AssignmentId    = (int)reader["AssignmentId"],
                        AdvisorId       = (int)reader["AdvisorId"],
                        AdvisorFullName = (string)reader["AdvisorFullName"],
                        StartDate       = DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                        EndDate         = reader["EndDate"] == DBNull.Value ? null
                                          : DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                        ChangeReason    = reader["ChangeReason"] == DBNull.Value ? null : (string)reader["ChangeReason"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetAdvisorHistory] {ex}");
            }
            return historyList;
        }

        public static clsAdvisorInfo? GetCurrentAdvisorByStudentId(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT a.AdvisorId, a.UserId, a.SicilNo,
                           ISNULL(a.AcademicTitle, '') AS AcademicTitle,
                           a.FirstName, a.LastName, a.DepartmentId,
                           d.DepartmentName, u.Username, u.IsActive
                    FROM   AdvisorAssignments aa
                    INNER JOIN Advisors    a ON aa.AdvisorId   = a.AdvisorId
                    INNER JOIN Users       u ON a.UserId       = u.UserId
                    INNER JOIN Departments d ON a.DepartmentId = d.DepartmentId
                    WHERE  aa.StudentId = @StudentId AND aa.EndDate IS NULL", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new clsAdvisorInfo
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetCurrentAdvisorByStudentId] {ex}");
                return null;
            }
        }

        public static bool ResetPassword(int studentId, string passwordHash)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Users SET PasswordHash = @PasswordHash
                    WHERE UserId = (SELECT UserId FROM Students WHERE StudentId = @StudentId)", conn);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@StudentId",    studentId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.ResetPassword] {ex}");
                return false;
            }
        }

        public static bool SetActive(int studentId, bool isActive)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Users SET IsActive = @IsActive
                    WHERE UserId = (SELECT UserId FROM Students WHERE StudentId = @StudentId)", conn);
                cmd.Parameters.AddWithValue("@IsActive",  isActive);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.SetActive] {ex}");
                return false;
            }
        }

        public static int GetActiveStudentCount()
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Students s
                    INNER JOIN Users u ON s.UserId = u.UserId
                    WHERE u.IsActive = 1", conn);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentData.GetActiveStudentCount] {ex}");
                return 0;
            }
        }
    }
}
