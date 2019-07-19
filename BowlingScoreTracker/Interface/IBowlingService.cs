using BowlingScoreTracker.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace BowlingScoreTracker.Service
{
    public interface IBowlingService
    {
        Task<HttpResponseMessage> ScoreByFrame();
        Task<HttpResponseMessage> TotalScore();
        Task<HttpResponseMessage> CreateNewGame();
        Task<HttpResponseMessage> Roll(int roll);
    }
}