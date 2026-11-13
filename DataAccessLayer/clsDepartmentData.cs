using EntityLayer;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public static class clsDepartmentData
    {
        private static clsDepartment _Map(SqlDataReader r) => new clsDepartment
        {
            DepartmentId   = (int)r["DepartmentId"],
            DepartmentName = (string)r["DepartmentName"],
            FacultyId      = (int)r["FacultyId"]
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
                using SqlDataReader r = cmd.ExecuteReader();
                return r.Read() ? _Map(r) : null;
            }
            catch { return null; }
        }

        public static int AddNewDepartment(clsDepartment dept)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    INSERT INTO Departments (DepartmentName, FacultyId)
                    VALUES (@DepartmentName, @FacultyId);
                    SELECT SCOPE_IDENTITY();", conn);
                cmd.Parameters.AddWithValue("@DepartmentName", dept.DepartmentName);
                cmd.Parameters.AddWithValue("@FacultyId",      dept.FacultyId);
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result is not null ? Convert.ToInt32(result) : -1;
            }
            catch { return -1; }
        }

        public static bool UpdateDepartment(clsDepartment dept)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    UPDATE Departments SET
                        DepartmentName = @DepartmentName,
                        FacultyId      = @FacultyId
                    WHERE DepartmentId = @DepartmentId", conn);
                cmd.Parameters.AddWithValue("@DepartmentId",   dept.DepartmentId);
                cmd.Parameters.AddWithValue("@DepartmentName", dept.DepartmentName);
                cmd.Parameters.AddWithValue("@FacultyId",      dept.FacultyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public static bool DeleteDepartment(int departmentId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "DELETE FROM Departments WHERE DepartmentId = @DepartmentId", conn);
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public static List<clsDepartment> GetAllDepartments()
        {
            List<clsDepartment> list = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT d.DepartmentId, d.DepartmentName, d.FacultyId
                    FROM   Departments d
                    INNER JOIN Faculties f ON d.FacultyId = f.FacultyId
                    ORDER BY f.FacultyName, d.DepartmentName", conn);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read()) list.Add(_Map(r));
            }
            catch { }
            return list;
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
            catch { return false; }
        }

        public static List<clsDepartment> GetDepartmentsByFacultyId(int facultyId)
        {
            List<clsDepartment> list = new();
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(
                    "SELECT DepartmentId, DepartmentName, FacultyId FROM Departments WHERE FacultyId = @FacultyId ORDER BY DepartmentName", conn);
                cmd.Parameters.AddWithValue("@FacultyId", facultyId);
                conn.Open();
                using SqlDataReader r = cmd.ExecuteReader();
                while (r.Read()) list.Add(_Map(r));
            }
            catch { }
            return list;
        }

        public static bool IsDepartmentNameExistInFaculty(string name, int facultyId, int excludeId)
        {
            try
            {
                using SqlConnection conn = new(clsDataAccessSettings.ConnectionString);
                using SqlCommand cmd = new(@"
                    SELECT COUNT(*) FROM Departments
                    WHERE DepartmentName = @DepartmentName
                      AND FacultyId     = @FacultyId
                      AND DepartmentId <> @ExcludeId", conn);
                cmd.Parameters.AddWithValue("@DepartmentName", name);
                cmd.Parameters.AddWithValue("@FacultyId",      facultyId);
                cmd.Parameters.AddWithValue("@ExcludeId",      excludeId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch { return false; }
        }
    }
}
