using MobileController.Models;

namespace MobileController.DTO
{
    public class Payroll
    {
        public Payroll(Student student, int totalDuration, double totalWage)
        {
            this.student = student;
            this.totalDuration = totalDuration;
            this.totalWage = totalWage;
        }

        public Student student { get; set; }
        public int totalDuration { get; set; }  
        public double totalWage { get; set; }

    }
}
