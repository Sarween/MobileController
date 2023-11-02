using Microsoft.EntityFrameworkCore;

namespace MobileController.Models
{
    [PrimaryKey("StaffID")]
    public class Staff
    {

        public Staff() { }

        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string StaffPassword { get; set; }
        public string StaffEmail { get; set; }
        public string DepartmentID { get; set; }
    }
}
