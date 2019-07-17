using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace BowlingScoreTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingGameController : ControllerBase
    {
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
        public HttpResponseMessage CreateNewGame()
        {
            //validate that not in middle of game
            //clear frames and/or start new game
            return null;
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
