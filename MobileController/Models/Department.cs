using Microsoft.EntityFrameworkCore;

namespace MobileController.Models
{
    [PrimaryKey("DepartmentID")]
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
