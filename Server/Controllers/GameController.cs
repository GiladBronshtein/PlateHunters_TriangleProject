﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using TriangleProject.Shared.Models.Games;
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

        [HttpGet("All")]
        public async Task<IActionResult> GetGamesByUserWithDetails(int userId)
        {
            Console.WriteLine("GetGamesByUserWithDetails");
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
                        List<GameMainTable> listGames = new List<GameMainTable>();
                        string gameQuery = "SELECT GameName,GameCode,IsPublished FROM Games WHERE UserId = @UserId";
                        var gamesRecords = await _db.GetRecordsAsync<GameMainTable>(gameQuery, param);
                        listGames = gamesRecords.ToList();
                        return Ok(listGames);
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }

        [HttpPost("addGame")]
        public async Task<IActionResult> AddGames(int userId, GameToAdd gameToAdd)
        {
            //בדיקה האם יש משתמש מחובר
            Console.WriteLine("userId");
            int? sessionId = HttpContext.Session.GetInt32("userId");
            if (sessionId != null)
            {
                //בדיקה שהמשתמש שמנסה להוסיף משחק הוא אותו משתמש שמחובר
                if (userId == sessionId)
                {
                    object param = new
                    {
                        UserId = userId
                    };

                    string userQuery = "SELECT FirstName FROM Users WHERE ID = @UserId";
                    var userRecords = await _db.GetRecordsAsync<UserWithGames>(userQuery, param);
                    UserWithGames user = userRecords.FirstOrDefault();
                    //בדיקה שיש משתמש כזה במחולל שלנו
                    if (user != null)
                    {
                        Console.WriteLine("user != null");
                        //ניצור משחק חדש בבסיס הנתונים
                        object newGameParam = new
                        {
                            CanPublish = false,
                            GameCode = 0,
                            GameEndMessage = "empty",
                            GameName = gameToAdd.GameName,
                            IsPublished = false,
                            QuestionCorrectCategory = "empty",
                            QuestionDescription = "empty",
                            QuestionHasImage = false,
                            QuestionImageText = "empty",
                            QuestionWrongCategory = "empty",
                            UserID = userId  // Use UserID instead of UserId
                        };

                        string insertGameQuery = "INSERT INTO Games (CanPublish, GameCode, GameEndMessage, GameName, IsPublished, QuestionCorrectCategory, QuestionDescription, QuestionHasImage, QuestionImageText, QuestionWrongCategory, UserID) " +
                                                        "VALUES (@CanPublish, @GameCode, @GameEndMessage, @GameName, @IsPublished, @QuestionCorrectCategory, @QuestionDescription, @QuestionHasImage, @QuestionImageText, @QuestionWrongCategory, @UserID)";

                        int newGameId = await _db.InsertReturnId(insertGameQuery, newGameParam);
                        if (newGameId != 0)
                        {
                            //אם המשחק נוצר בהצלחה, נחשב את הקוד עבורו
                            int gameCode = newGameId + 100;
                            object updateParam = new
                            {
                                ID = newGameId,
                                GameCode = gameCode
                            };
                            string updateCodeQuery = "UPDATE Games SET GameCode = @GameCode	WHERE ID=@ID";
                            bool isUpdate = await _db.SaveDataAsync(updateCodeQuery, updateParam);
                            if (isUpdate == true)
                            {
                                //אם המשחק עודכן בהצלחה- נחזיר את הפרטים שלו לעורך
                                object param2 = new
                                {
                                    ID = newGameId
                                };
                                string gameQuery = "SELECT ID, GameName, GameCode, IsPublished, CanPublish FROM Games WHERE ID = @ID";

                                var gameRecord = await _db.GetRecordsAsync<GameForCreation>(gameQuery, param2);
                                GameForCreation newGame = gameRecord.FirstOrDefault();
                                return Ok(newGame);
                            }
                            return BadRequest("Game code not created");
                        }
                        return BadRequest("Game not created");
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }

        [HttpDelete("deleteGame/{deleteGameCode}")]
        public async Task<IActionResult> DeleteGame(int userId, int deleteGameCode)
        {
            Console.WriteLine("deleteGameCode");
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
                        object param2 = new
                        {
                            GameCodeId = deleteGameCode
                        };
                        string gameQuery = "SELECT GameName FROM Games WHERE GameCode = @GameCodeId";
                        var gameRecords = await _db.GetRecordsAsync<UserWithGames>(gameQuery, param2);
                        UserWithGames game = gameRecords.FirstOrDefault();
                        if (game != null)
                        {
                            object param3 = new
                            {
                                GameCodeId = deleteGameCode
                            };
                            string deleteGameQuery = "DELETE FROM Games WHERE GameCode = @GameCodeId";
                            bool isDelete = await _db.SaveDataAsync(deleteGameQuery, param3);
                            if (isDelete == true)
                            {
                                return Ok("Game deleted");
                            }
                            return BadRequest("Game not deleted");
                        }
                        return BadRequest("Game Not Found");
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }
    }
}