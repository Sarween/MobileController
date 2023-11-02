using Microsoft.EntityFrameworkCore;

namespace MobileController.Models
{
    [PrimaryKey("ParentID")]
    public class Parent
    {
        public int ParentID { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
