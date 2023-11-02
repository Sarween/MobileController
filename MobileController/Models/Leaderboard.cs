namespace MobileController.Models
{
    public class Leaderboard
    {
        public Leaderboard() { }

        public Leaderboard(int lateCount, int totalDuration, int rating, int job_cancellation, int score)
        {
            this.lateCount = lateCount;
            this.totalDuration = totalDuration;
            this.rating = rating;
            this.job_cancellation = job_cancellation;
            this.score = score;
        }

        public int lateCount { get; set; }
        public int totalDuration { get; set; }
        public int rating { get; set; }
        public int job_cancellation { get; set; }
        public int score { get; set; }
    }
}
