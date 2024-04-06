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
        public DateOnly? JobShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string JobLocation { get; set; }
        public string JobDescription { get; set; }
        public int isFCFS { get; set; }
        public int StuNumReqRemain { get; set; }

        //public virtual Staff Staff { get; set; }

    }
}
