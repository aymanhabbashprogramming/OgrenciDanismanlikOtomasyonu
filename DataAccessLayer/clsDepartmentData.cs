using EntityLayer;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace DataAccessLayer
{
    public static class clsDepartmentData
    {
        private static clsDepartment _MapDepartment(SqlDataReader reader) => new clsDepartment
        {
            DepartmentId   = (int)reader["DepartmentId"],
            DepartmentName = (string)reader["DepartmentName"],
            FacultyId      = (int)reader["FacultyId"]
        };

        public static clsDepartment? GetDepartmentInfoByID(int departmentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT DepartmentId, DepartmentName, FacultyId FROM Departments WHERE DepartmentId = @DepartmentId", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                return reader.Read() ? _MapDepartment(reader) : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentData.GetDepartmentInfoByID] {ex.Message}");
                return null;
            }
        }

        public static List<clsDepartment> GetAllDepartments()
        {
            List<clsDepartment> departmentList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT Department.DepartmentId, Department.DepartmentName, Department.FacultyId
                    FROM   Departments AS Department
                    INNER JOIN Faculties AS Faculty ON Department.FacultyId = Faculty.FacultyId
                    ORDER BY Faculty.FacultyName, Department.DepartmentName", conn);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departmentList.Add(_MapDepartment(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentData.GetAllDepartments] {ex.Message}");
            }
            return departmentList;
        }

        public static List<clsDepartment> GetDepartmentsByFacultyId(int facultyId)
        {
            List<clsDepartment> departmentList = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT DepartmentId, DepartmentName, FacultyId FROM Departments WHERE FacultyId = @FacultyId ORDER BY DepartmentName", conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);
                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    departmentList.Add(_MapDepartment(reader));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentData.GetDepartmentsByFacultyId] {ex.Message}");
            }
            return departmentList;
        }

        public static bool IsDepartmentExist(int departmentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT COUNT(*) FROM Departments WHERE DepartmentId = @DepartmentId", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[clsDepartmentData.IsDepartmentExist] {ex.Message}");
                return false;
            }
        }
    }
}
