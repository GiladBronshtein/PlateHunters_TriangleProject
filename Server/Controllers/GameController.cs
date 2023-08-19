using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using TriangleProject.Shared.Models.Users;

namespace TriangleProject.Server.Controllers
{
    [Route("api/[controller]/{userId}")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly DbRepository _db;
        public GameController(DbRepository db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetGamesByUser(int userId)
        {
            Console.WriteLine("GetGamesByUser");
            int? sessionId = HttpContext.Session.GetInt32("userId");
            if (sessionId != null)
            {
                if (userId == sessionId)
                {
                    object param = new
                    {
                        UserId = userId
                    };
                    string userQuery = "SELECT FirstName FROM Users WHERE ID = @UserId";
                    var userRecords = await _db.GetRecordsAsync<UserWithGames>(userQuery, param);
                    UserWithGames user = userRecords.FirstOrDefault();
                    if (user != null)
                    {
                        string gameQuery = "SELECT GameName FROM Games WHERE UserId = @UserId";
                        var gamesRecords = await _db.GetRecordsAsync<string>(gameQuery, param);
                        user.Games = gamesRecords.ToList();
                        return Ok(user);
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }
    }
}