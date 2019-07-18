using BowlingScoreTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BowlingScoreTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingGameController : ControllerBase
    {
        private readonly FrameScores currentGame = new FrameScores();

        [HttpGet]
        public string Get()
        {
            return "Welcome to the Bowling Game Controller";
        }

        [HttpGet("GetScoreByFrame")]
        public HttpResponseMessage GetScoreByFrame()
        {
            //return framescores
            return null;
        }

        [HttpGet("GetTotalScore")]
        public HttpResponseMessage GetTotalScore()
        {
            //return the last frame's current score
            return null;
        }

        [HttpPost("CreateNewGame")]
        public async Task<HttpResponseMessage> CreateNewGame()
        {
            //validate that not in middle of game
            if (currentGame.GameStatus == Status.Active)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                });
            else
            {
                //clear frames and/or start new game
                if (currentGame.Frames != null)
                    currentGame.Frames = null;

                currentGame.Frames = new List<FrameScore>();
                currentGame.NextFrame = new FrameScore();
                
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Created);
                });
            }
        }

        [HttpPost("Roll/{roll}")]
        public async Task<HttpResponseMessage> Roll(int roll)
        {
            //check game status
            if (currentGame.GameStatus != Status.Active)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            //get current frame
            var currentFrame = currentGame.CurrentFrame;
            
            //validate roll
            if ((roll < 0) || (roll > 10))
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });
            else if (roll + currentFrame.Rolls.Sum() > 10)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                });

            currentFrame.Roll = roll;

            //Function to identify to head to next frame (and when finished)
            if (currentFrame.FrameNumber == 10)
            {
                //can be up to 3 rolls if strike or spare in first 2 rolls
                if (currentFrame.Rolls.Count == 3)
                    currentGame.GameStatus = Status.Complete;
                else if (currentFrame.Rolls.Count > 1 && currentFrame.Rolls.Sum() < 10)
                    currentGame.GameStatus = Status.Complete;
            }
            else
            {
                //next set?
                if (roll == 10)
                    currentGame.NextFrame = new FrameScore();
                else if (currentFrame.Rolls.Count > 1)
                    currentGame.NextFrame = new FrameScore();
            }

            return await Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            });
        }
    }
}
