
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace MobileController.Models
{
    [PrimaryKey("RecruitmentID", "StudentID")]
    public class Shift
    {
        public Shift()
        {
        }
        //[ForeignKey("RecruitmentID")]
        public int RecruitmentID { get; set; }

        public int StudentID { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime CheckOutTime { get; set; }
        public bool IsLate { get; set; }
        public bool IsOvertime { get; set; }
        public string StudentComment { get; set; }
        public string StaffReview { get; set; }
        public int Rating { get; set; }
        public bool IsAuthorized { get; set; }

    }
}
