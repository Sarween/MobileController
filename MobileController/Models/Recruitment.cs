using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileController.Models
{
    [PrimaryKey("RecruitmentID")]
    public class Recruitment
    {
        public Recruitment() { }

        public int RecruitmentID { get; set; }
        [ForeignKey("StaffID")]
        public int StaffID { get; set; }
        public int StuNumReq { get; set; }
        public DateTime JobShiftDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string JobLocation { get; set; }
        public string JobDescription { get; set; }
        public int isFCFS { get; set; }
        public int StuNumReqRemain { get; set; }

    }
}
