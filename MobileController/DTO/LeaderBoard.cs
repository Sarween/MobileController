using MobileController.Models;

namespace MobileController.DTO
{
    public class LeaderBoard
    {
        public LeaderBoard(Student student, Performance performance, int score)
        {
            this.student = student;
            this.performance = performance;
            this.score = score;
        }

        public Student student { get; set; }
        public Performance performance { get; set; }
        public double score { get; set; }
    }
}
