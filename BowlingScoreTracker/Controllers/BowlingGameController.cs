using BowlingScoreTracker.Service;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

/// TO DO place detailed header above each function
/// TO DO datamap work
/// TO DO review readme and send questions remaining
/// TO DO plan what it would take to expand this to include multiple users, groups, history tracking
/// TO DO refactor and reduce code smell
/// <summary>
/// Main Bowling Game Controller with four public functions:
/// GetScoreByFrame, GetTotalScore, CreateNewGame, Roll
/// </summary>
namespace BowlingScoreTracker.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BowlingGameController : ControllerBase
    {
        private IBowlingService currentGame;

        public BowlingGameController(IBowlingService bowlingService)
        {
            currentGame = bowlingService;
        }

        [HttpGet("ScoreByFrame")]
        public async Task<HttpResponseMessage> GetScoreByFrame()
        {
            return await currentGame.ScoreByFrame();
        }

        [HttpGet("TotalScore")]
        public async Task<HttpResponseMessage> GetTotalScore()
        {
            return await currentGame.TotalScore();
        }

        [HttpPost("CreateNewGame")]
        public async Task<HttpResponseMessage> CreateNewGame()
        {
            return await currentGame.CreateNewGame();
        }

        [HttpPost("Roll/{roll}")]
        public async Task<HttpResponseMessage> Roll(int roll)
        {
            return await currentGame.Roll(roll);
        }

    }
}
