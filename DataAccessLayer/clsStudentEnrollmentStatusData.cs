using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsStudentEnrollmentStatusData
    {
        public static clsStudentEnrollmentStatus? GetCurrentStatus(int studentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT EnrollmentStatusId, StudentId, Status, StartDate, EndDate, Note
                    FROM   StudentEnrollmentStatuses
                    WHERE  StudentId = @StudentId AND EndDate IS NULL", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return _Map(reader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentEnrollmentStatusData.GetCurrentStatus] {ex}");
                return null;
            }
        }

        public static List<clsEnrollmentStatusHistory> GetStatusHistory(int studentId)
        {
            List<clsEnrollmentStatusHistory> historyList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT EnrollmentStatusId, Status, StartDate, EndDate, Note
                    FROM   StudentEnrollmentStatuses
                    WHERE  StudentId = @StudentId
                    ORDER BY StartDate DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    historyList.Add(new clsEnrollmentStatusHistory
                    {
                        EnrollmentStatusId = (int)reader["EnrollmentStatusId"],
                        Status             = (string)reader["Status"],
                        StartDate          = DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                        EndDate            = reader["EndDate"] == DBNull.Value
                                             ? null
                                             : DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                        Note               = reader["Note"] == DBNull.Value ? null : (string)reader["Note"]
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentEnrollmentStatusData.GetStatusHistory] {ex}");
            }
            return historyList;
        }

        public static List<clsExchangeProgram> GetExchangeHistory(int studentId)
        {
            List<clsExchangeProgram> exchangeList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT ExchangeId, StudentId, ProgramName, HostUniversity, Country,
                           SemesterId, StartDate, EndDate
                    FROM   ExchangePrograms
                    WHERE  StudentId = @StudentId
                    ORDER BY ISNULL(StartDate, CAST('1900-01-01' AS DATE)) DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", studentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exchangeList.Add(_MapExchange(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentEnrollmentStatusData.GetExchangeHistory] {ex}");
            }
            return exchangeList;
        }

        public static bool ChangeStatus(int studentId, string newStatus, string? note,
                                        clsExchangeProgram? exchangeProgram)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                try
                {
                    // 1. Get the current open status so we know if we're leaving an exchange.
                    string? previousStatus = null;
                    using (SqlCommand cmdRead = new(@"
                        SELECT Status FROM StudentEnrollmentStatuses
                        WHERE StudentId = @StudentId AND EndDate IS NULL", conn, transaction))
                    {
                        cmdRead.Parameters.AddWithValue("@StudentId", studentId);
                        object? result = cmdRead.ExecuteScalar();
                        if (result is not null and not DBNull)
                        {
                            previousStatus = (string)result;
                        }
                    }

                    // 2. Close the current open status record.
                    using (SqlCommand cmdClose = new(@"
                        UPDATE StudentEnrollmentStatuses
                        SET    EndDate = CAST(GETDATE() AS DATE)
                        WHERE  StudentId = @StudentId AND EndDate IS NULL", conn, transaction))
                    {
                        cmdClose.Parameters.AddWithValue("@StudentId", studentId);
                        cmdClose.ExecuteNonQuery();
                    }

                    // 3. Insert the new status record.
                    using (SqlCommand cmdInsert = new(@"
                        INSERT INTO StudentEnrollmentStatuses (StudentId, Status, StartDate, EndDate, Note)
                        VALUES (@StudentId, @Status, CAST(GETDATE() AS DATE), NULL, @Note)", conn, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("@StudentId", studentId);
                        cmdInsert.Parameters.AddWithValue("@Status",    newStatus);
                        cmdInsert.Parameters.AddWithValue("@Note",      (object?)note ?? DBNull.Value);
                        cmdInsert.ExecuteNonQuery();
                    }

                    // 4. If leaving an exchange status, close the open ExchangePrograms row.
                    bool wasExchange = _IsExchangeStatus(previousStatus);
                    if (wasExchange)
                    {
                        using SqlCommand cmdCloseExchange = new(@"
                            UPDATE ExchangePrograms
                            SET    EndDate = CAST(GETDATE() AS DATE)
                            WHERE  StudentId = @StudentId AND EndDate IS NULL", conn, transaction);
                        cmdCloseExchange.Parameters.AddWithValue("@StudentId", studentId);
                        cmdCloseExchange.ExecuteNonQuery();
                    }

                    // 5. If entering an exchange status, insert a new ExchangePrograms row.
                    if (_IsExchangeStatus(newStatus) && exchangeProgram is not null)
                    {
                        using SqlCommand cmdInsertExchange = new(@"
                            INSERT INTO ExchangePrograms
                                (StudentId, ProgramName, HostUniversity, Country, SemesterId, StartDate, EndDate)
                            VALUES
                                (@StudentId, @ProgramName, @HostUniversity, @Country,
                                 @SemesterId, CAST(GETDATE() AS DATE), NULL)", conn, transaction);
                        cmdInsertExchange.Parameters.AddWithValue("@StudentId",     studentId);
                        cmdInsertExchange.Parameters.AddWithValue("@ProgramName",   exchangeProgram.ProgramName);
                        cmdInsertExchange.Parameters.AddWithValue("@HostUniversity",(object?)exchangeProgram.HostUniversity ?? DBNull.Value);
                        cmdInsertExchange.Parameters.AddWithValue("@Country",       (object?)exchangeProgram.Country       ?? DBNull.Value);
                        cmdInsertExchange.Parameters.AddWithValue("@SemesterId",    exchangeProgram.SemesterId);
                        cmdInsertExchange.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[clsStudentEnrollmentStatusData.ChangeStatus inner] {ex}");
                    transaction.Rollback();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsStudentEnrollmentStatusData.ChangeStatus outer] {ex}");
                return false;
            }
        }

        private static bool _IsExchangeStatus(string? status) =>
            status == "Erasmus" || status == "Farabi" || status == "Mevlana";

        private static clsStudentEnrollmentStatus _Map(SqlDataReader reader) =>
            new clsStudentEnrollmentStatus
            {
                EnrollmentStatusId = (int)reader["EnrollmentStatusId"],
                StudentId          = (int)reader["StudentId"],
                Status             = (string)reader["Status"],
                StartDate          = DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                EndDate            = reader["EndDate"] == DBNull.Value
                                     ? null
                                     : DateOnly.FromDateTime((DateTime)reader["EndDate"]),
                Note               = reader["Note"] == DBNull.Value ? null : (string)reader["Note"]
            };

        private static clsExchangeProgram _MapExchange(SqlDataReader reader) =>
            new clsExchangeProgram
            {
                ExchangeId     = (int)reader["ExchangeId"],
                StudentId      = (int)reader["StudentId"],
                ProgramName    = (string)reader["ProgramName"],
                HostUniversity = reader["HostUniversity"] == DBNull.Value ? null : (string)reader["HostUniversity"],
                Country        = reader["Country"]        == DBNull.Value ? null : (string)reader["Country"],
                SemesterId     = (int)reader["SemesterId"],
                StartDate      = reader["StartDate"] == DBNull.Value
                                 ? null
                                 : DateOnly.FromDateTime((DateTime)reader["StartDate"]),
                EndDate        = reader["EndDate"] == DBNull.Value
                                 ? null
                                 : DateOnly.FromDateTime((DateTime)reader["EndDate"])
            };
    }
}
