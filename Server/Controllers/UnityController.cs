using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using TriangleProject.Shared.Models.Games;
using TriangleProject.Shared.Models.GamesContent;

namespace TriangleProject.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnityController : ControllerBase
    {
        private readonly DbRepository _db;

        public UnityController(DbRepository db)
        {
            _db = db;
        }

        [HttpGet("GameCode/{GameCode}")]
        public async Task<IActionResult> GetGameCode(int GameCode)
        {
            object getParam = new
            {
                gameCode = GameCode
            };
            //Get game code
            string getGameQuery = "SELECT GameCode FROM Games WHERE GameCode = @gameCode and IsPublished = 1";
            var getRecords = await _db.GetRecordsAsync<string>(getGameQuery, getParam); //Get game code
            string gameCode = getRecords.FirstOrDefault();
            if (gameCode == null) //If no game found
            {
                return BadRequest("no game found");
            }

            //Get course details for the game code
            string getGameDetailsQuery = "select * from games g where GameCode = @gameCode";
            var getGameDetailsRecords = await _db.GetRecordsAsync<GameDetails>(getGameDetailsQuery, getParam);
            GameDetails gameDetails = getGameDetailsRecords.FirstOrDefault();

            //Get question details for the game code
            string getAnswersQuery = "select i.id,i.AnswerDescription,i.IsCorrect,i.HasImage,i.AnswerImageText from items i, games g where i.GameID = g.id and g.GameCode = @gameCode";
            var getAnswersRecords = await _db.GetRecordsAsync<GameAnswers>(getAnswersQuery, getParam);
            gameDetails.Answers = getAnswersRecords.ToList();
            return Ok(gameDetails);
        }
    }
}
