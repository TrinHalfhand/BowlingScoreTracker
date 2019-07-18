using BowlingScoreTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BowlingScoreTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingGameController : ControllerBase
    {
        private FrameScores currentFrame = new FrameScores();

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
            if (currentFrame.GameStatus == Status.Active)
                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
                });
            else
            {
                //clear frames and/or start new game
                if (currentFrame.Frames != null)
                    currentFrame.Frames = null;

                currentFrame.Frames = new System.Collections.Generic.List<FrameScore>();
                currentFrame.GameStatus = Status.Active;

                return await Task<HttpResponseMessage>.Factory.StartNew(() =>
                {
                    return new HttpResponseMessage(HttpStatusCode.Created);
                });
            }
        }

        [HttpPost("Roll/{roll}")]
        public HttpResponseMessage Roll(int roll)
        {
            //Function to identify to head to next frame (and when finished)
            //Set roll value into frame
            //Create function to calculate score for each frame and set into model
            return null;
        }
    }
}
