using BowlingScoreTracker.Controllers;
using BowlingScoreTracker.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace TestBowlingScoreTracker
{
    [TestClass]
    public class ScoreCheck
    {
        private BowlingGameController bowlingGameAPI;

        [TestInitialize]
        public void SetupBowlingSession()
        {
            bowlingGameAPI = new BowlingGameController();
        }

        [DataRow(1, HttpStatusCode.BadRequest)]
        [DataRow(-1, HttpStatusCode.BadRequest)]
        [DataRow(10, HttpStatusCode.BadRequest)]
        [DataRow(9, HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public void TestRolls_BeforeGameCreation(int roll, HttpStatusCode result)
        {
            var rollResult = bowlingGameAPI.Roll(roll);

            Assert.IsNotNull(rollResult);
            Assert.AreEqual(rollResult.Result.StatusCode, result);
        }

        [DataRow(HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public void TestScoreByFrame_AfterGameCreation(HttpStatusCode result)
        {
           
        }

        [DataRow(HttpStatusCode.BadRequest)]
        [DataTestMethod]
        public void TestTotalScore_BeforeGameCreation(HttpStatusCode result)
        {
            HttpResponseMessage scoreResult = bowlingGameAPI.GetTotalScore();

            Assert.IsNotNull(scoreResult);
            Assert.AreEqual(scoreResult.StatusCode, result);
        }

        [DataRow(HttpStatusCode.Created, 0)]
        [DataRow(HttpStatusCode.Conflict, 1)]
        [DataTestMethod]
        public void TestCreatingANewGame(HttpStatusCode result, int repeatRequest)
        {
            var rollResult = bowlingGameAPI.CreateNewGame();

            for (int repeat = 0; repeat < repeatRequest; repeat++)
                rollResult = bowlingGameAPI.CreateNewGame();

            Assert.IsNotNull(rollResult);
            Assert.AreEqual(rollResult.Result.StatusCode, result);
        }

        [DataRow(1, HttpStatusCode.Accepted)]
        [DataRow(-1, HttpStatusCode.BadRequest)]
        [DataRow(11, HttpStatusCode.BadRequest)]
        [DataRow(9, HttpStatusCode.Accepted)]
        [DataTestMethod]
        public void TestSingleRoll(int roll, HttpStatusCode result)
        {
            TestCreatingANewGame(HttpStatusCode.Created, 0);
            var rollResult = bowlingGameAPI.Roll(roll);

            Assert.IsNotNull(rollResult);
            Assert.AreEqual(rollResult.Result.StatusCode, result);
        }

        [DataRow(new int[] {1, 10}, HttpStatusCode.BadRequest, 1)]
        [DataRow(new int[] { -1, 10 }, HttpStatusCode.Accepted, 2)]
        [DataRow(new int[] { 1, 11 }, HttpStatusCode.BadRequest, 1)]
        [DataRow(new int[] { 1, 9, 10, 10, 9, 1 }, HttpStatusCode.Accepted, 5)]
        [DataTestMethod]
        public void TestMultipleRolls(int[] rolls, HttpStatusCode result, int currentFrameNumber)
        {
            HttpResponseMessage finalStatus = null;
            TestCreatingANewGame(HttpStatusCode.Created, 0);

            foreach (var roll in rolls)
            {
                var rollResult = bowlingGameAPI.Roll(roll);
                Assert.IsNotNull(rollResult);
                finalStatus = rollResult.Result;
            }

            //should confirm with scorebyframe if frame count matches
            Assert.AreEqual(finalStatus.StatusCode, result);
        }

        [DataRow(10, 1, new int[] { 1, 9 })]
        [DataTestMethod]
        public void TestScoreByFrame_AfterGameCreation(int total, int frame, int[] rolls)
        {
            HttpResponseMessage scoreResult = bowlingGameAPI.GetScoreByFrame();
            Assert.IsNotNull(scoreResult);
            Assert.AreEqual(scoreResult.StatusCode, HttpStatusCode.OK);

            var frameScores = JsonConvert.DeserializeObject<FrameScores>(scoreResult.Content.ToString());

            Assert.IsNotNull(frameScores);
            Assert.IsNotNull(frameScores.Frames);
            Assert.AreEqual(1, frameScores.Frames.Count);
            Assert.AreEqual(total, frameScores.Frames[frame].ScoreTotal);
            Assert.AreEqual(rolls[0], frameScores.Frames[frame].Rolls[0]);
            Assert.AreEqual(rolls[1], frameScores.Frames[frame].Rolls[1]);
        }

        [DataRow(10)]
        [DataTestMethod]
        public void TestTotalScore_AfterGameCreation(int total)
        {
            HttpResponseMessage scoreResult = bowlingGameAPI.GetTotalScore();
            //in case more may be wanted in json message
            Assert.IsNotNull(scoreResult);
            Assert.AreEqual(scoreResult.StatusCode, HttpStatusCode.OK);

            var totalScore = JsonConvert.DeserializeObject<TotalScore>(scoreResult.Content.ToString());
            Assert.AreEqual(total, totalScore.Total);
        }
    }
}
