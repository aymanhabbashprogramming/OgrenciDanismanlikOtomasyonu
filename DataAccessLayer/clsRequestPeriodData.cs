using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsRequestPeriodData
    {
        private static clsRequestPeriodInfo _MapPeriodInfo(SqlDataReader reader) => new clsRequestPeriodInfo
        {
            RequestPeriodId = (int)reader["RequestPeriodId"],
            SemesterId      = (int)reader["SemesterId"],
            RequestTypeId   = (int)reader["RequestTypeId"],
            FormCode        = (string)reader["FormCode"],
            FormName        = (string)reader["FormName"],
            StartDate       = DateOnly.FromDateTime((DateTime)reader["StartDate"]),
            EndDate         = DateOnly.FromDateTime((DateTime)reader["EndDate"])
        };

        private const string _infoSelect = @"
            SELECT RequestPeriod.RequestPeriodId, RequestPeriod.SemesterId, RequestPeriod.RequestTypeId,
                   RequestType.FormCode, RequestType.FormName,
                   RequestPeriod.StartDate, RequestPeriod.EndDate
            FROM   RequestPeriods AS RequestPeriod
            INNER JOIN RequestTypes AS RequestType ON RequestPeriod.RequestTypeId = RequestType.RequestTypeId";

        public static List<clsRequestPeriodInfo> GetPeriodsBySemester(int semesterId)
        {
            List<clsRequestPeriodInfo> periodList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + @"
                    WHERE RequestPeriod.SemesterId = @SemesterId
                    ORDER BY RequestType.FormCode", conn);
                cmd.Parameters.AddWithValue("@SemesterId", semesterId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    periodList.Add(_MapPeriodInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestPeriodData.GetPeriodsBySemester] {ex}");
            }
            return periodList;
        }

        public static clsRequestPeriodInfo? GetOpenPeriod(int requestTypeId, DateOnly date)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + @"
                    WHERE RequestPeriod.RequestTypeId = @RequestTypeId
                      AND RequestPeriod.StartDate    <= @Date
                      AND RequestPeriod.EndDate      >= @Date", conn);
                cmd.Parameters.AddWithValue("@RequestTypeId", requestTypeId);
                cmd.Parameters.AddWithValue("@Date",          date.ToDateTime(TimeOnly.MinValue));
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapPeriodInfo(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestPeriodData.GetOpenPeriod] {ex}");
                return null;
            }
        }

        public static List<clsRequestPeriodInfo> GetUpcomingPeriods(int maxCount)
        {
            List<clsRequestPeriodInfo> periodList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(_infoSelect + @"
                    WHERE  RequestPeriod.EndDate >= CAST(GETDATE() AS DATE)
                    ORDER  BY RequestPeriod.StartDate
                    OFFSET 0 ROWS FETCH NEXT @MaxCount ROWS ONLY", conn);
                cmd.Parameters.AddWithValue("@MaxCount", maxCount);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    periodList.Add(_MapPeriodInfo(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestPeriodData.GetUpcomingPeriods] {ex}");
            }
            return periodList;
        }

        public static bool IsRequestTypeOpen(string formCode)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*)
                    FROM   RequestPeriods AS RequestPeriod
                    INNER JOIN RequestTypes AS RequestType ON RequestPeriod.RequestTypeId = RequestType.RequestTypeId
                    WHERE  RequestType.FormCode   = @FormCode
                      AND  RequestPeriod.StartDate <= CAST(GETDATE() AS DATE)
                      AND  RequestPeriod.EndDate   >= CAST(GETDATE() AS DATE)", conn);
                cmd.Parameters.AddWithValue("@FormCode", formCode);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestPeriodData.IsRequestTypeOpen] {ex}");
                return false;
            }
        }
    }
}
