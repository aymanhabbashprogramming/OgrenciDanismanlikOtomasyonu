using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsParentData
    {
        private static clsParent _MapParent(SqlDataReader reader) => new clsParent
        {
            ParentId          = (int)reader["ParentId"],
            StudentFileId     = (int)reader["StudentFileId"],
            Relation          = (string)reader["Relation"],
            FirstName         = reader["FirstName"]         == DBNull.Value ? null : (string)reader["FirstName"],
            LastName          = reader["LastName"]          == DBNull.Value ? null : (string)reader["LastName"],
            Phone             = reader["Phone"]             == DBNull.Value ? null : (string)reader["Phone"],
            Occupation        = reader["Occupation"]        == DBNull.Value ? null : (string)reader["Occupation"],
            HasSocialSecurity = reader["HasSocialSecurity"] == DBNull.Value ? null : (bool?)  (bool)reader["HasSocialSecurity"],
            Workplace         = reader["Workplace"]         == DBNull.Value ? null : (string)reader["Workplace"],
            MonthlyIncome     = reader["MonthlyIncome"]     == DBNull.Value ? null : (decimal?)(decimal)reader["MonthlyIncome"],
            EducationLevel    = reader["EducationLevel"]    == DBNull.Value ? null : (string)reader["EducationLevel"],
            IsAlive           = reader["IsAlive"]           == DBNull.Value ? null : (bool?)  (bool)reader["IsAlive"],
            DeathYear         = reader["DeathYear"]         == DBNull.Value ? null : (int?)   (int) reader["DeathYear"]
        };

        public static List<clsParent> GetParentsByStudentFileId(int studentFileId)
        {
            List<clsParent> parentList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT ParentId, StudentFileId, Relation, FirstName, LastName, Phone, Occupation,
                           HasSocialSecurity, Workplace, MonthlyIncome, EducationLevel, IsAlive, DeathYear
                    FROM Parents
                    WHERE StudentFileId = @StudentFileId", conn);
                cmd.Parameters.AddWithValue("@StudentFileId", studentFileId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    parentList.Add(_MapParent(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsParentData.GetParentsByStudentFileId] {ex}");
            }
            return parentList;
        }

        public static clsParent? GetParentInfoByID(int parentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT ParentId, StudentFileId, Relation, FirstName, LastName, Phone, Occupation,
                           HasSocialSecurity, Workplace, MonthlyIncome, EducationLevel, IsAlive, DeathYear
                    FROM Parents
                    WHERE ParentId = @ParentId", conn);
                cmd.Parameters.AddWithValue("@ParentId", parentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapParent(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsParentData.GetParentInfoByID] {ex}");
                return null;
            }
        }

        public static bool IsParentExist(int parentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Parents WHERE ParentId = @ParentId", conn);
                cmd.Parameters.AddWithValue("@ParentId", parentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsParentData.IsParentExist] {ex}");
                return false;
            }
        }
    }
}
