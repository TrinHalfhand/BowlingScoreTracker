using BowlingScoreTracker.Models;
using BowlingScoreTracker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        private readonly IBowlingService currentGame;

        public BowlingGameController(IBowlingService bowlingService)
        {
            currentGame = bowlingService;
        }

        /// <summary>
        /// Returns a set of scores player for each frame
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /ScoreByFrame
        /// </remarks>
        /// <returns>HttpResponseMessage</returns>
        /// <response code="200">Returned message with json content in body containing List<FrameScore> model structured data</response>
        /// <response code="400">Game is not active</response>
        [ProducesResponseType(typeof(List<FrameScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("ScoreByFrame")]
        public async Task<ActionResult> GetScoreByFrame()
        {
            var result = await currentGame.ScoreByFrame();

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return BadRequest();
            else
                return new JsonResult(JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// Returns the current total score
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /TotalScore
        /// </remarks>
        /// <returns>HttpResponseMessage</returns>
        /// <response code="200">Returned message with json content in body containing the total score</response>
        /// <response code="400">Game is not active</response>
        /// <response code="400">Something failed while totaling the score</response>
        [ProducesResponseType(typeof(TotalScore), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("TotalScore")]
        public async Task<ActionResult> GetTotalScore()
        {
            var result = await currentGame.TotalScore();

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                return BadRequest();
            else
                return new JsonResult(JsonConvert.DeserializeObject(await result.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// Generates a new game (only if not already in the middle of an active game)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /CreateNewGame
        /// </remarks>
        /// <returns>HttpResponseMessage</returns>
        /// <response code="201">New game is created</response>
        /// <response code="409">Already running a game</response>
        [HttpPost("CreateNewGame")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateNewGame()
        {
            var result = await currentGame.CreateNewGame();

            if (result.StatusCode != System.Net.HttpStatusCode.Created)
                return Conflict();
            else
                return new StatusCodeResult(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Enters the next roll into the game.
        /// If the game has not started or is complete it will return this information instead.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Roll
        ///     {
        ///        "roll": 5,
        ///     }
        /// </remarks>
        /// <param name="roll"></param>
        /// <returns>HttpResponseMessage</returns>
        /// <response code="202">Roll was successfully entered and score calculated</response>
        /// <response code="400">If the roll is not between 0-10</response>
        /// <response code="400">If the total of both rolls in the set is > 10</response>
        /// <response code="400">If the game is not actively running</response>
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Roll/{roll}")]
        public async Task<ActionResult> Roll(int roll)
        {
            var result = await currentGame.Roll(roll);

            if (result.StatusCode != System.Net.HttpStatusCode.Accepted)
                return BadRequest();
            else
                return new StatusCodeResult(StatusCodes.Status202Accepted);
        }
    }
}
