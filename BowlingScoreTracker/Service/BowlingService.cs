using BowlingScoreTracker.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BowlingScoreTracker.Service
{
    public class BowlingService : IBowlingService
    {
        private readonly BowlingGame _currentGame;

        public BowlingService()
        {
            this._currentGame = new BowlingGame();
        }

        public async Task<HttpResponseMessage> ScoreByFrame()
        {
            //validate that a game exists
            if (_currentGame.GameStatus == Status.Unknown)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            //setup totalscore content
            string jsonResponse = JsonConvert.SerializeObject(_currentGame.Frames);
            var message = await Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK));
            message.Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json");
            return message;
        }

        public async Task<HttpResponseMessage> TotalScore()
        {
            //validate that a game exists
            if (_currentGame.GameStatus == Status.Unknown)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            //setup totalscore content
            try
            {
                int totalScore = _currentGame.Frames.Where(x => x.Rolls.Count > 0).Last<FrameScore>().ScoreTotal;
                string jsonResponse = JsonConvert.SerializeObject(new TotalScore { Total = totalScore });
                var message = await Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK));
                message.Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json");
                return message;
            }
            catch
            {
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });
            }
        }

        public async Task<HttpResponseMessage> CreateNewGame()
        {
            //validate that not in middle of game
            if (_currentGame.GameStatus == Status.Active)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                });
            else
            {
                //clear frames and/or start new game
                if (_currentGame.Frames != null)
                    _currentGame.Frames = null;

                _currentGame.Frames = new List<FrameScore>();
                _currentGame.NextFrame = new FrameScore();

                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Created);
                });
            }
        }

        public async Task<HttpResponseMessage> Roll(int roll)
        {
            //check game status
            if (_currentGame.GameStatus != Status.Active)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            //get current frame
            var currentFrame = _currentGame.CurrentFrame;

            //validate roll
            bool validateRoll = ValidateRoll(roll, currentFrame);

            if (!validateRoll)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            currentFrame.Roll = roll;

            //set score
            CalculateFrameScores(_currentGame.Frames, currentFrame.FrameNumber - 1);

            //Function to identify to head to next frame (and when finished)
            if (currentFrame.FrameNumber == 10)
            {
                //can be up to 3 rolls if strike or spare in first 2 rolls
                if (currentFrame.Rolls.Count == 3)
                    _currentGame.GameStatus = Status.Complete;
                else if (currentFrame.Rolls.Count > 1 && currentFrame.Rolls.Sum() < 10)
                    _currentGame.GameStatus = Status.Complete;
            }
            else
            {
                //next set?
                if (roll == 10)
                    _currentGame.NextFrame = new FrameScore();
                else if (currentFrame.Rolls.Count > 1)
                    _currentGame.NextFrame = new FrameScore();
            }

            return await Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            });
        }

        /// <summary>
        /// Validation of roll
        ///     roll must be between 0-10 
        ///     in first 9 frames total of both rolls cannot exceed 10
        ///     in the 10th frame if the first roll is not 10 then the total of the first two roll cannot exceed 10
        /// </summary>
        /// <param name="roll">roll value being entered</param>
        /// <param name="currentFrame">Frame where the roll is to be applied</param>
        /// <returns>if passes validation will return true (default is false)</returns>
        private static bool ValidateRoll(int roll, FrameScore currentFrame)
        {
            bool validateRoll = false;

            if ((roll >= 0) && (roll <= 10))
            {
                if (currentFrame.FrameNumber == 10)
                    validateRoll = currentFrame.Rolls.Take(1).Sum() == 10 || roll + currentFrame.Rolls.Take(1).Sum() <= 10;
                else if (roll + currentFrame.Rolls.Sum() <= 10)
                    validateRoll = true;
            }

            return validateRoll;
        }

        /// <summary>
        /// Computes the current frame score total by first adjusting the scores in
        /// prior frames (up to 2 is possible) finishing with the current frame.
        /// 
        /// From starting index look up to two previous frames (if available)
        /// Iterate through taking each frame and scoring according to the
        /// next 0, 1, or 2 frames ahead.
        /// example:
        ///     spare, strike, strike, spare 
        ///     spare round: 10
        ///     strike round: back to spare update to 20, add 20 to strike = 30
        ///     strike round: back to strike update to 40, add 40 to strike = 50
        ///     spare round: back to strike, strike update add spare first value to 40
        ///         forward new score to next strike, add full spare value to strike = 60 + first spare value
        /// </summary>
        /// <param name="activeGameFrame">Frames to apply scoring to</param>
        /// <param name="upToThisIndex">This frame needs a new score and it's rolls can effect up to 2 frames prior</param>
        private void CalculateFrameScores(List<FrameScore> activeGameFrame, int upToThisIndex)
        {
            for (int frame = (upToThisIndex - 2 < 0) ? 0 : upToThisIndex - 2; frame <= upToThisIndex; frame++)
            {
                CalculateNewScore(activeGameFrame[frame]);
            }
        }

        /// <summary>
        /// Does the actual scoring based on standard 10 frame bowling rules
        /// First 9 frames are scored base on two rolls;
        ///     if the first roll = 10 (strike) scoring is effected by the next two rolls that contains a value (0-10)
        ///     if the sum of both rolls = 10 (spare) score is effected by the next roll value
        ///     all other rolls score will just add rolls sum to the total score
        /// The 10th frame can have up to 3 rolls if the sum of the first two meets or exceeds 10.
        /// </summary>
        /// <param name="frameToScore">The selected frame to adjust it's total score on</param>
        private void CalculateNewScore(FrameScore frameToScore)
        {
            int index = frameToScore.FrameNumber - 1;
            int previousFrameScore = (index > 0) ? _currentGame.Frames[index - 1].ScoreTotal : 0;

            //no bonus in 10th frame
            if (frameToScore.FrameNumber < 10)
            {
                switch (frameToScore.BonusType)
                {
                    case BonusType.NoBonus:
                        frameToScore.ScoreTotal = previousFrameScore + frameToScore.Rolls.Sum();
                        break;
                    case BonusType.Spare:
                        bool hasBonusToApply = (_currentGame.Frames.Count > index + 1);
                        frameToScore.ScoreTotal = hasBonusToApply ? _currentGame.Frames[index + 1].Rolls.Take(1).Sum() : 0;
                        frameToScore.ScoreTotal += previousFrameScore + frameToScore.Rolls.Sum();
                        break;
                    case BonusType.Strike:
                        bool hasBonusToApplyFromFirstFrame = (_currentGame.Frames.Count > index + 1);
                        bool hasBonusToApplyFromSecondFrame = (_currentGame.Frames.Count > index + 2)
                            && _currentGame.Frames[index + 1].BonusType == BonusType.Strike;
                        frameToScore.ScoreTotal = hasBonusToApplyFromFirstFrame ? _currentGame.Frames[index + 1].Rolls.Take(2).Sum() : 0;
                        frameToScore.ScoreTotal += hasBonusToApplyFromSecondFrame ? _currentGame.Frames[index + 2].Rolls.Take(1).Sum() : 0;
                        frameToScore.ScoreTotal += previousFrameScore + frameToScore.Rolls.Sum();
                        break;
                }
            }
            else
                frameToScore.ScoreTotal = previousFrameScore + frameToScore.Rolls.Sum();
        }
    }
}
