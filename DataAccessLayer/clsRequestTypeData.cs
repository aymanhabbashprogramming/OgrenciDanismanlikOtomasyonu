using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsRequestTypeData
    {
        private static clsRequestType _MapRequestType(SqlDataReader reader) => new clsRequestType
        {
            RequestTypeId           = (int)reader["RequestTypeId"],
            FormCode                = (string)reader["FormCode"],
            FormName                = (string)reader["FormName"],
            RequiresAdvisorApproval = (bool)reader["RequiresAdvisorApproval"]
        };

        public static List<clsRequestType> GetAll()
        {
            List<clsRequestType> typeList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT RequestTypeId, FormCode, FormName, RequiresAdvisorApproval
                    FROM   RequestTypes
                    ORDER  BY FormCode", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    typeList.Add(_MapRequestType(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestTypeData.GetAll] {ex}");
            }
            return typeList;
        }

        public static clsRequestType? GetByFormCode(string formCode)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT RequestTypeId, FormCode, FormName, RequiresAdvisorApproval
                    FROM   RequestTypes
                    WHERE  FormCode = @FormCode", conn);
                cmd.Parameters.AddWithValue("@FormCode", formCode);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapRequestType(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsRequestTypeData.GetByFormCode] {ex}");
                return null;
            }
        }
    }
}
