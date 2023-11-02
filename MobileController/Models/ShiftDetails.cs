using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileController.Models
{
    public class ShiftDetails
    {
        public ShiftDetails()
        {
        }

        public int RecruitmentID { get; set; }

        public int StudentID { get; set; }


    }
}
