using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BowlingScoreTracker.Models
{
    public enum Status
    {
        Unknown, //default value
        Active,
        Complete
    }
    public class BowlingGame
    {
        public List<FrameScore> Frames { get; set; }
        [DefaultValue(Status.Unknown)]
        public Status GameStatus { get; set; }
        public FrameScore CurrentFrame
        {
            get
            {
                return Frames[Frames.Count - 1];
            }
        }
        public FrameScore BowlingFrame
        {
            get
            {
                return CurrentFrame;
            }
            set
            {
                Frames.Add(value);
            }
        }
    }

    public enum BonusType
    {
        NoBonus,
        Spare,
        Strike,
    }
    public class FrameScore
    {
        [Range(0, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Roll
        {
            set
            {
                Rolls.Add(value);
            }
        }
        public List<int> Rolls { get; set; }
        public int FrameNumber { get; set; }
        public BonusType BonusType
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
