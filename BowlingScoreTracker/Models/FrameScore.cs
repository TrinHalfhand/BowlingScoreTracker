using System.Collections.Generic;

namespace BowlingScoreTracker.Models
{
    public class FrameScores
    {
        public List<FrameScore> Frames { get; set; }
    }

    public enum BonusType
    {
        NoBonus,
        Spare,
        Strike,
    }
    public class FrameScore
    {
        public List<int> Rolls {
            get
            {
                return Rolls;
            }
            set
            {
                if (Rolls == null)
                    Rolls = new List<int>();
            }
        }
        private BonusType BonusType
        {
            get
            {
                int totalRoll = 0;

                foreach (int roll in Rolls) { totalRoll += roll; }

                BonusType type;
                if (totalRoll == 10)
                    if (Rolls[0] == 10)
                        type = BonusType.Strike;
                    else
                        type = BonusType.Spare;
                else
                    type = BonusType.NoBonus;

                return type;
            }
        }

        public int ScoreTotal { get; set; }
    }
}
