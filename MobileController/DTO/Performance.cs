using MobileController.Models;

namespace MobileController.DTO
{
    public class Performance
    {
        public String name {  get; set; }
        public double punctuality { get; set; }
        public double rating { get; set; }
        public double commitment { get; set; }
        public double reliability { get; set; }
        public int duration { get; set; }
        public int overtimeCount { get; set; }
        public double score { get; set; }
        public Performance() { }

        public Performance(string name, double punctuality, double rating, double commitment, double reliability, int duration, int overtimeCount, double score)
        {
            this.name = name;
            this.punctuality = punctuality;
            this.rating = rating;
            this.commitment = commitment;
            this.reliability = reliability;
            this.duration = duration;
            this.overtimeCount = overtimeCount;
            this.score = score;
        }
    }
}
