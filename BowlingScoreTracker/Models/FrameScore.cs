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
    public class FrameScores
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

        public FrameScore NextFrame
        {
            get
            {
                return CurrentFrame;
            }
            set
            {
                value.Rolls = new List<int>();
                value.FrameNumber = Frames.Count + 1;
                Frames.Add(value);

                if (Frames.Count <= 10)
                    GameStatus = Status.Active;
                else
                    GameStatus = Status.Complete;
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
