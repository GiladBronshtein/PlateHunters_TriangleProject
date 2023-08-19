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
                codeFromUser = GameCode
            };
           
            //Get course details for the codeFromUser if it exists and is published
            string getGameDetailsQuery = "select * from games g where GameCode = @codeFromUser and IsPublished = 1";
            var getGameDetailsRecords = await _db.GetRecordsAsync<GameDetails>(getGameDetailsQuery, getParam);
            GameDetails gameDetails = getGameDetailsRecords.FirstOrDefault();

            //If no game found, return bad request
            if(gameDetails == null)
            {
                return BadRequest("No game found for game code: " + GameCode);
            }           

            //Get question details for the game code
            string getAnswersQuery = "select i.id,i.AnswerDescription,i.IsCorrect,i.HasImage,i.AnswerImageText " +
                "from items i, games g where i.GameID = g.id and g.GameCode = @codeFromUser";
            var getAnswersRecords = await _db.GetRecordsAsync<GameAnswers>(getAnswersQuery, getParam);
            gameDetails.Answers = getAnswersRecords.ToList();
            return Ok(gameDetails);
        }
    }
}
