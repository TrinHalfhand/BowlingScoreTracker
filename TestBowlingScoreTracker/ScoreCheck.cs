using BowlingScoreTracker.Controllers;
using BowlingScoreTracker.Models;
using BowlingScoreTracker.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestBowlingScoreTracker
{
    [TestClass]
    public class ScoreCheck
    {
        private BowlingGameController bowlingGameAPI;

        [TestInitialize]
        public void SetupBowlingSession()
        {
            BowlingService bowlingService = new BowlingService();
            bowlingGameAPI = new BowlingGameController(bowlingService);
        }

        [DataRow(1, HttpStatusCode.BadRequest)]
        [DataRow(-1, HttpStatusCode.BadRequest)]
        [DataRow(10, HttpStatusCode.BadRequest)]
        [DataRow(9, HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public async Task TestRolls_BeforeGameCreation(int roll, HttpStatusCode result)
        {
            var rollResult = await bowlingGameAPI.Roll(roll);

            Assert.IsNotNull(rollResult);
            Assert.AreEqual((int)result, (rollResult as StatusCodeResult).StatusCode);
        }

        [DataRow(HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public void TestScoreByFrame_BeforeGameCreation(HttpStatusCode result)
        {
            var scoreResult = bowlingGameAPI.GetScoreByFrame();

            Assert.IsNotNull(scoreResult);
            Assert.AreEqual((int)result, (scoreResult.Result as StatusCodeResult).StatusCode);
        }

        [DataRow(HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public async Task TestTotalScore_BeforeGameCreation(HttpStatusCode result)
        {
            var scoreResult = await bowlingGameAPI.GetTotalScore();

            Assert.IsNotNull(scoreResult);
            Assert.AreEqual((int)result, (scoreResult as StatusCodeResult).StatusCode);
        }

        [DataRow(HttpStatusCode.Created, 0)]
        [DataRow(HttpStatusCode.Conflict, 1)]
        [DataTestMethod]
        public async Task TestCreatingANewGame(HttpStatusCode result, int repeatRequest)
        {
            var rollResult = await bowlingGameAPI.CreateNewGame();

            for (int repeat = 0; repeat < repeatRequest; repeat++)
                rollResult = await bowlingGameAPI.CreateNewGame();

            Assert.IsNotNull(rollResult);
            Assert.AreEqual((int)result, (rollResult as StatusCodeResult).StatusCode);
        }

        [DataRow(1, HttpStatusCode.Accepted)]
        [DataRow(-1, HttpStatusCode.BadRequest)]
        [DataRow(11, HttpStatusCode.BadRequest)]
        [DataRow(9, HttpStatusCode.Accepted)]
        [DataTestMethod]
        public async Task TestRolls_Single(int roll, HttpStatusCode result)
        {
            await TestCreatingANewGame(HttpStatusCode.Created, 0);
            var rollResult = await bowlingGameAPI.Roll(roll);
            StatusCodeResult statusOfRoll = null;

            Assert.IsNotNull(rollResult);

            if (rollResult is StatusCodeResult)
            {
                statusOfRoll = (rollResult as StatusCodeResult);
                Assert.IsNotNull(statusOfRoll);
            }

            Assert.AreEqual((int)result, statusOfRoll.StatusCode);
        }

        [DataRow(new int[] {1, 10}, HttpStatusCode.BadRequest)]
        [DataRow(new int[] { -1, 10 }, HttpStatusCode.Accepted)]
        [DataRow(new int[] { 1, 11 }, HttpStatusCode.BadRequest)]
        [DataRow(new int[] { 1, 9, 10, 10, 9, 1 }, HttpStatusCode.Accepted)]
        [DataTestMethod]
        public async Task TestRolls_Multiple(int[] rolls, HttpStatusCode result)
        {
            StatusCodeResult finalStatus = null;
            await TestCreatingANewGame(HttpStatusCode.Created, 0);

            foreach (var roll in rolls)
            {
                var rollResult = await bowlingGameAPI.Roll(roll);
                Assert.IsNotNull(rollResult);
                finalStatus = rollResult as StatusCodeResult;
            }

            //should confirm with scorebyframe if frame count matches
            Assert.AreEqual((int) result, finalStatus.StatusCode);
        }

        [DataRow(10, 1, 1, new int[] { 1, 9 })]
        [DataRow(20, 1, 2, new int[] { 1, 9, 10 })]
        [DataRow(30, 2, 2, new int[] { 1, 9, 10 })]
        [DataRow(60, 5, 10, new int[] { 1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 1, 7, 3, 6, 4, 10, 2, 8, 6 })]
        [DataRow(300, 10, 10, new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 })]
        [DataTestMethod]
        public async Task TestScoreByFrame_AfterGameCreation(int total, int frame, int frameCount, int[] rolls)
        {
            await TestRolls_Multiple(rolls, HttpStatusCode.Accepted);
            var scoreResult = await bowlingGameAPI.GetScoreByFrame();
            Assert.IsNotNull(scoreResult);
            Assert.IsTrue(scoreResult is JsonResult);
            
            string text = (scoreResult as JsonResult).Value.ToString();
            List<FrameScore> frameScores = JsonConvert.DeserializeObject<List<FrameScore>>(text);
            int indexOfFrame = frame - 1;

            Assert.IsNotNull(frameScores);
            Assert.AreEqual(frameCount, frameScores.Count);
            Assert.AreEqual(total, frameScores[indexOfFrame].ScoreTotal);

            var rollList = frameScores.SelectMany(x => x.Rolls).ToList();

            for (int count = 0; count < rolls.Count(); count++)
            {
                Assert.AreEqual(rolls[count], rollList[count]);
            }
        }

        [DataRow(new int[] { 1, 9 }, 10)]
        [DataRow(new int[] { 1, 9, 10 }, 30)]
        [DataRow(new int[] { 1, 9, 10, 10 }, 50)]
        [DataRow(new int[] { 1, 9, 10, 10, 9, 0 }, 77)]
        [DataRow(new int[] { 1, 9, 10, 10, 9, 1 }, 79)]
        [DataRow(new int[] { 1, 4, 4, 5, 6, 4, 5, 5, 10, 0, 1, 7, 3, 6, 4, 10, 2, 8, 6 }, 133)]
        [DataRow(new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }, 300)]
        [DataTestMethod]
        public async Task TestTotalScore_AfterGameCreationAsync(int[] rolls, int total)
        {
            await TestRolls_Multiple(rolls, HttpStatusCode.Accepted);

            var scoreResult = await bowlingGameAPI.GetTotalScore();
            //in case more may be wanted in json message
            Assert.IsNotNull(scoreResult);
            Assert.IsTrue(scoreResult is JsonResult);

            string text = (scoreResult as JsonResult).Value.ToString();
            var totalScore = JsonConvert.DeserializeObject<TotalScore>(text);
            Assert.AreEqual(total, totalScore.Total);
        }
    }
}
