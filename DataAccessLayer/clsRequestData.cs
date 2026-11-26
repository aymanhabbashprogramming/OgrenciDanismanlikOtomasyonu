using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsRequestData
    {
        public static clsRequestFormPreFillInfo? GetFormPreFillInfo(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT
                        Student.StudentId,
                        Student.StudentNumber,
                        Student.FirstName + N' ' + Student.LastName          AS FullName,
                        Student.NationalId,
                        Department.DepartmentName,
                        Student.EducationType,
                        Student.MobilePhone,
                        ISNULL(Advisor.AdvisorId, 0)                         AS AdvisorId,
                        CASE
                            WHEN Advisor.AdvisorId IS NULL THEN NULL
                            ELSE ISNULL(Advisor.AcademicTitle + N' ', N'')
                                 + Advisor.FirstName + N' ' + Advisor.LastName
                        END                                                  AS AdvisorFullName,
                        ISNULL(CurrentSemester.SemesterId, 0)                AS CurrentSemesterId,
                        CurrentSemester.SemesterName                         AS CurrentSemesterName
                    FROM Students AS Student
                    INNER JOIN Departments AS Department
                        ON Department.DepartmentId = Student.DepartmentId
                    LEFT JOIN AdvisorAssignments AS AdvisorAssignment
                        ON AdvisorAssignment.StudentId = Student.StudentId
                        AND AdvisorAssignment.EndDate IS NULL
                    LEFT JOIN Advisors AS Advisor
                        ON Advisor.AdvisorId = AdvisorAssignment.AdvisorId
                    OUTER APPLY (
                        SELECT TOP 1 SemesterId,
                                     AcademicYear + N' ' + Term AS SemesterName
                        FROM   Semesters
                        WHERE  StartDate <= CAST(GETDATE() AS DATE)
                          AND  EndDate   >= CAST(GETDATE() AS DATE)
                        ORDER  BY StartDate DESC
                    ) AS CurrentSemester
                    WHERE Student.StudentId = @StudentId", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;

                return new clsRequestFormPreFillInfo
                {
                    StudentId         = (int)reader["StudentId"],
                    StudentNumber     = (string)reader["StudentNumber"],
                    FullName          = (string)reader["FullName"],
                    NationalId        = (string)reader["NationalId"],
                    DepartmentName    = (string)reader["DepartmentName"],
                    EducationType     = (string)reader["EducationType"],
                    MobilePhone       = reader["MobilePhone"]    == DBNull.Value ? null : (string)reader["MobilePhone"],
                    AdvisorId         = (int)reader["AdvisorId"],
                    AdvisorFullName   = reader["AdvisorFullName"]      == DBNull.Value ? null : (string)reader["AdvisorFullName"],
                    CurrentSemesterId = (int)reader["CurrentSemesterId"],
                    SemesterName      = reader["CurrentSemesterName"]  == DBNull.Value ? null : (string)reader["CurrentSemesterName"]
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestData.GetFormPreFillInfo] {ex}");
                return null;
            }
        }

        public static List<clsRequestInfo> GetRequestsByStudentId(int studentId)
        {
            List<clsRequestInfo> requestList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT
                        Request.RequestId,
                        Request.RequestTypeId,
                        RequestType.FormCode,
                        RequestType.FormName,
                        Semester.AcademicYear + N' ' + Semester.Term AS SemesterName,
                        Request.SubmittedAt,
                        ISNULL(LatestStage.Stage, N'Gönderildi')     AS CurrentStage
                    FROM Requests AS Request
                    INNER JOIN RequestTypes AS RequestType
                        ON RequestType.RequestTypeId = Request.RequestTypeId
                    INNER JOIN Semesters AS Semester
                        ON Semester.SemesterId = Request.SemesterId
                    OUTER APPLY (
                        SELECT TOP 1 Stage
                        FROM   RequestStages
                        WHERE  RequestId = Request.RequestId
                        ORDER  BY StageAt DESC, RequestStageId DESC
                    ) AS LatestStage
                    WHERE Request.StudentId = @StudentId
                    ORDER BY Request.SubmittedAt DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    requestList.Add(new clsRequestInfo
                    {
                        RequestId     = (int)reader["RequestId"],
                        RequestTypeId = (int)reader["RequestTypeId"],
                        FormCode      = (string)reader["FormCode"],
                        FormName      = (string)reader["FormName"],
                        SemesterName  = (string)reader["SemesterName"],
                        SubmittedAt   = (DateTime)reader["SubmittedAt"],
                        CurrentStage  = (string)reader["CurrentStage"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestData.GetRequestsByStudentId] {ex}");
            }
            return requestList;
        }

        public static clsRequestDetailInfo? GetRequestDetail(int requestId, int studentId)
        {
            clsRequestDetailInfo? detail = null;
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();

                using (SqlCommand cmd = new(@"
                    SELECT
                        Request.RequestId,
                        Request.RequestTypeId,
                        RequestType.FormCode,
                        RequestType.FormName,
                        Semester.AcademicYear + N' ' + Semester.Term AS SemesterName,
                        Request.SubmittedAt,
                        Request.Reason,
                        Request.DeclaredGano,
                        CASE
                            WHEN Advisor.AdvisorId IS NULL THEN NULL
                            ELSE ISNULL(Advisor.AcademicTitle + N' ', N'')
                                 + Advisor.FirstName + N' ' + Advisor.LastName
                        END AS AdvisorFullName
                    FROM Requests AS Request
                    INNER JOIN RequestTypes AS RequestType
                        ON RequestType.RequestTypeId = Request.RequestTypeId
                    INNER JOIN Semesters AS Semester
                        ON Semester.SemesterId = Request.SemesterId
                    LEFT JOIN Advisors AS Advisor
                        ON Advisor.AdvisorId = Request.AdvisorId
                    WHERE Request.RequestId = @RequestId
                      AND Request.StudentId = @StudentId", conn))
                {
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.Read()) return null;

                    detail = new clsRequestDetailInfo
                    {
                        RequestId       = (int)reader["RequestId"],
                        RequestTypeId   = (int)reader["RequestTypeId"],
                        FormCode        = (string)reader["FormCode"],
                        FormName        = (string)reader["FormName"],
                        SemesterName    = (string)reader["SemesterName"],
                        SubmittedAt     = (DateTime)reader["SubmittedAt"],
                        Reason          = reader["Reason"]          == DBNull.Value ? null : (string)reader["Reason"],
                        DeclaredGano    = reader["DeclaredGano"]    == DBNull.Value ? null : (decimal?)reader["DeclaredGano"],
                        AdvisorFullName = reader["AdvisorFullName"] == DBNull.Value ? null : (string)reader["AdvisorFullName"]
                    };
                }

                using (SqlCommand cmd = new(@"
                    SELECT RequestCourseId, RequestId, TableGroup, CourseCode, CourseName,
                           AttendanceStatus, Operation, PreviousGrade, CourseSemester,
                           TheoryHours, PracticeHours, Ects, University, Department
                    FROM   RequestCourses
                    WHERE  RequestId = @RequestId
                    ORDER  BY RequestCourseId", conn))
                {
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        detail.Courses.Add(new clsRequestCourse
                        {
                            RequestCourseId  = (int)reader["RequestCourseId"],
                            RequestId        = (int)reader["RequestId"],
                            TableGroup       = reader["TableGroup"]       == DBNull.Value ? null : (string)reader["TableGroup"],
                            CourseCode       = (string)reader["CourseCode"],
                            CourseName       = (string)reader["CourseName"],
                            AttendanceStatus = reader["AttendanceStatus"] == DBNull.Value ? null : (string)reader["AttendanceStatus"],
                            Operation        = reader["Operation"]        == DBNull.Value ? null : (string)reader["Operation"],
                            PreviousGrade    = reader["PreviousGrade"]    == DBNull.Value ? null : (string)reader["PreviousGrade"],
                            CourseSemester   = reader["CourseSemester"]   == DBNull.Value ? null : (int?)(int)reader["CourseSemester"],
                            TheoryHours      = reader["TheoryHours"]      == DBNull.Value ? null : (int?)(int)reader["TheoryHours"],
                            PracticeHours    = reader["PracticeHours"]    == DBNull.Value ? null : (int?)(int)reader["PracticeHours"],
                            Ects             = reader["Ects"]             == DBNull.Value ? null : (int?)(int)reader["Ects"],
                            University       = reader["University"]       == DBNull.Value ? null : (string)reader["University"],
                            Department       = reader["Department"]       == DBNull.Value ? null : (string)reader["Department"]
                        });
                    }
                }

                using (SqlCommand cmd = new(@"
                    SELECT RequestStageId, RequestId, Stage, StageAt, ByUserId, Note
                    FROM   RequestStages
                    WHERE  RequestId = @RequestId
                    ORDER  BY StageAt, RequestStageId", conn))
                {
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    using SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        detail.Stages.Add(new clsRequestStage
                        {
                            RequestStageId = (int)reader["RequestStageId"],
                            RequestId      = (int)reader["RequestId"],
                            Stage          = (string)reader["Stage"],
                            StageAt        = (DateTime)reader["StageAt"],
                            ByUserId       = (int)reader["ByUserId"],
                            Note           = reader["Note"] == DBNull.Value ? null : (string)reader["Note"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestData.GetRequestDetail] {ex}");
                return null;
            }
            return detail;
        }

        public static int AddNewRequestAtomic(clsRequest request, List<clsRequestCourse> courses, int byUserId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    int requestId;

                    using (SqlCommand cmd = new(@"
                        INSERT INTO Requests
                            (StudentId, RequestTypeId, AdvisorId, SemesterId, SubmittedAt,
                             Reason, DeclaredGano, Excuse, ContactAddress, RejectionReason, DecidedAt)
                        VALUES
                            (@StudentId, @RequestTypeId, @AdvisorId, @SemesterId, SYSDATETIME(),
                             @Reason, @DeclaredGano, NULL, NULL, NULL, NULL);
                        SELECT SCOPE_IDENTITY();", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@StudentId",     request.StudentId);
                        cmd.Parameters.AddWithValue("@RequestTypeId", request.RequestTypeId);
                        cmd.Parameters.AddWithValue("@AdvisorId",     request.AdvisorId);
                        cmd.Parameters.AddWithValue("@SemesterId",    request.SemesterId);
                        cmd.Parameters.AddWithValue("@Reason",        (object?)request.Reason      ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DeclaredGano",  (object?)request.DeclaredGano ?? DBNull.Value);

                        requestId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (clsRequestCourse course in courses)
                    {
                        using SqlCommand cmd = new(@"
                            INSERT INTO RequestCourses
                                (RequestId, TableGroup, CourseCode, CourseName,
                                 AttendanceStatus, Operation, PreviousGrade, CourseSemester,
                                 TheoryHours, PracticeHours, Ects, University, Department)
                            VALUES
                                (@RequestId, @TableGroup, @CourseCode, @CourseName,
                                 @AttendanceStatus, @Operation, @PreviousGrade, @CourseSemester,
                                 @TheoryHours, @PracticeHours, @Ects, @University, @Department)",
                            conn, transaction);
                        cmd.Parameters.AddWithValue("@RequestId",       requestId);
                        cmd.Parameters.AddWithValue("@TableGroup",      (object?)course.TableGroup       ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseCode",      course.CourseCode);
                        cmd.Parameters.AddWithValue("@CourseName",      course.CourseName);
                        cmd.Parameters.AddWithValue("@AttendanceStatus",(object?)course.AttendanceStatus ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Operation",       (object?)course.Operation        ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PreviousGrade",   (object?)course.PreviousGrade    ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CourseSemester",  (object?)course.CourseSemester   ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TheoryHours",     (object?)course.TheoryHours      ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PracticeHours",   (object?)course.PracticeHours    ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Ects",            (object?)course.Ects             ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@University",      (object?)course.University       ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Department",      (object?)course.Department       ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    DateTime stageTime = DateTime.Now;

                    using (SqlCommand cmd = new(@"
                        INSERT INTO RequestStages (RequestId, Stage, StageAt, ByUserId, Note)
                        VALUES (@RequestId, @Stage, @StageAt, @ByUserId, NULL)",
                        conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@Stage",     clsRequestStages.Submitted);
                        cmd.Parameters.AddWithValue("@StageAt",   stageTime);
                        cmd.Parameters.AddWithValue("@ByUserId",  byUserId);
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new(@"
                        INSERT INTO RequestStages (RequestId, Stage, StageAt, ByUserId, Note)
                        VALUES (@RequestId, @Stage, @StageAt, @ByUserId, NULL)",
                        conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@Stage",     clsRequestStages.NotifiedToAdvisor);
                        cmd.Parameters.AddWithValue("@StageAt",   stageTime);
                        cmd.Parameters.AddWithValue("@ByUserId",  byUserId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return requestId;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestData.AddNewRequestAtomic] {ex}");
                return -1;
            }
        }
    }
}
