namespace BowlingScoreTracker.Models
{
    public class FrameScore
    {
        public int[] Rolls { get; set; }
        public int Frame { get; set; }
        public int Total
        {
            get
            {
                int total = 0;

                foreach (int roll in Rolls)
                {
                    total += roll;
                }

                return total;
            }
        }
    }
}
