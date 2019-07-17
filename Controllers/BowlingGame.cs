using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace BowlingScoreTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BowlingGame : ControllerBase
    {
        [HttpGet]
        public HttpResponseMessage GetScoreByFrame()
        {
            return null;
        }

        [HttpGet]
        public HttpResponseMessage GetTotalScore()
        {
            return null;
        }

        [HttpPost]
        public HttpResponseMessage CreateNewGame()
        {
            return null;
        }

        [HttpPost]
        public HttpResponseMessage Roll(int roll)
        {
            return null;
        }
    }
}
