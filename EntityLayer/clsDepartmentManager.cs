namespace EntityLayer
{
    public class clsDepartmentManager
    {
        public int ManagerId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Position { get; set; }
    }
}
