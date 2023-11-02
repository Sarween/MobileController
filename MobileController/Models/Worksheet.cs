using Microsoft.EntityFrameworkCore;

namespace MobileController.Models
{
    [PrimaryKey("StudentID", "Year", "Month")]
    public class Worksheet
    {
        public Worksheet() { }
        public int StudentID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool IsChecked { get; set; }
        public DateTime claimedDate { get; set; }
        public bool IsConfirmed { get; set; }
        public int ranking { get; set; }
    }
}
