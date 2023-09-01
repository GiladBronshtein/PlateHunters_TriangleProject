﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TriangleDbRepository;
using TriangleProject.Shared.Models.Games;
using TriangleProject.Shared.Models.GamesContent;
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
                        var gamesRecords = await _db.GetRecordsAsync<Games>(gameQuery, param);
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
                        List<Games> listGames = new List<Games>();
                        string gameQuery = "SELECT ID,GameName,GameCode,IsPublished,CanPublish FROM Games WHERE UserId = @UserId";
                        var gamesRecords = await _db.GetRecordsAsync<Games>(gameQuery, param);
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

        [HttpPut("updateGame/{updateGameCode}")]
        public async Task<IActionResult> UpdateGame(int userId, int updateGameCode, GameToUpdate gameToUpdate)
        {
            Console.WriteLine(gameToUpdate.ID);
            Console.WriteLine(gameToUpdate.GameCode);
            Console.WriteLine(gameToUpdate.GameName);
            Console.WriteLine("updateGameCode");
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
                    
                    
                    //Console.WriteLine(param);
                    UserWithGames user = userRecords.FirstOrDefault();
                    if (user != null)
                    {
                        object param2 = new
                        {
                            GameCode = gameToUpdate.GameCode

                        };
                        string gameQuery = "SELECT GameName FROM Games WHERE GameCode = @GameCode";
                        var gameRecords = await _db.GetRecordsAsync<UserWithGames>(gameQuery, param2);
                        UserWithGames game = gameRecords.FirstOrDefault();
                        if (game != null)
                        {
                            object param3 = new
                            {
                                GameCode = gameToUpdate.GameCode,
                                GameName = gameToUpdate.GameName,
                                GameQuestion = gameToUpdate.QuestionDescription

                            };
                            string updateGameQuery = "UPDATE Games SET GameName = @GameName, " +
                                "QuestionDescription = @GameQuestion " +
                                "WHERE GameCode = @GameCode";
                            bool isUpdate = await _db.SaveDataAsync(updateGameQuery, param3);
                            if (isUpdate == true)
                            {
                                return Ok("Game updated");
                            }
                            return BadRequest("Game not updated");
                        }
                        return BadRequest("Game Not Found");
                    }
                    return BadRequest("User Not Found");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }

        [HttpGet("getGame/{gameCode}")]
        public async Task<IActionResult> GetGame(int userId, int gameCode)
        {

            object getParam = new
            {
                codeFromUser = gameCode
            };

            //Get course details for the codeFromUser if it exists and is published
            string getGameDetailsQuery = "select * from games g where GameCode = @codeFromUser";
            var getGameDetailsRecords = await _db.GetRecordsAsync<GameDetails>(getGameDetailsQuery, getParam);
            GameDetails gameDetails = getGameDetailsRecords.FirstOrDefault();

            //If no game found, return bad request
            if (gameDetails == null)
            {
                return BadRequest("No game found for game code: " + gameCode);
            }

            //Get question details for the game code
            string getAnswersQuery = "select i.id,i.AnswerDescription,i.IsCorrect,i.HasImage,i.AnswerImageText " +
                "from items i, games g where i.GameID = g.id and g.GameCode = @codeFromUser";
            var getAnswersRecords = await _db.GetRecordsAsync<GameAnswers>(getAnswersQuery, getParam);
            gameDetails.Answers = getAnswersRecords.ToList();
            return Ok(gameDetails);

            
        }

        [HttpPost("publishGame")]
        public async Task<IActionResult> publishGame(int userId, PublishGame game)
        {
             int? sessionId = HttpContext.Session.GetInt32("userId");
            if (sessionId != null)
            {
                if (userId == sessionId)
                {
                    object param = new
                    {
                        UserId = userId,
                        gameID = game.ID
                    };
                    Console.WriteLine(param);

                    string checkQuery = "SELECT GameName FROM Games WHERE UserId = @UserId and ID=@gameID";
                    var checkRecords = await _db.GetRecordsAsync<string>(checkQuery, param);
                    string gameName = checkRecords.FirstOrDefault();
                    if (gameName != null)
                    {
                        if (game.IsPublished == true)
                        {
                            CanPublishFunc(game.ID);    
                            object canPublishParam = new
                            {
                                gameID = game.ID
                            };
                            string canPublishQuery = "SELECT CanPublish FROM Games WHERE ID=@gameID";
                            var canPublishRecords = await _db.GetRecordsAsync<bool>(canPublishQuery, canPublishParam);
                            bool canPublish = canPublishRecords.FirstOrDefault();
                            if (canPublish == false)
                            {
                                return BadRequest("This game cannot be published");
                            }
                        }
                        string updateQuery = "UPDATE Games SET IsPublished=@IsPublished WHERE ID=@ID";
                        bool isUpdate = await _db.SaveDataAsync(updateQuery, game);
                        if (isUpdate == true)
                        {
                            return Ok();
                        }
                        return BadRequest("Update Failed");
                    }
                    return BadRequest("It's Not Your Game");
                }
                return BadRequest("User Not Logged In");
            }
            return BadRequest("No Session");
        }

        private async Task CanPublishFunc(int gameId)
        {
            int minQuestions = 0;
            bool isPublished = false;
            bool canPublish = false;
            object param = new
            {
                ID = gameId
            };
            string queryQuestionCount = "SELECT Count(ID) from Items WHERE GameID=@ID";
            var recordQuestionCount = await _db.GetRecordsAsync<int>(queryQuestionCount, param);
            int numberOfQuestions = recordQuestionCount.FirstOrDefault();
            if (numberOfQuestions >= minQuestions)
            {
                canPublish = true;
            }
            if (canPublish == true)
            {
                string updateQuery = "UPDATE Games SET CanPublish= true WHERE ID = @ID";
                bool isUpdate = await _db.SaveDataAsync(updateQuery, param);
                Console.WriteLine($"The update of game: {gameId} was completed successfully {isUpdate}");
            }
            else
            {
                string updateQuery = "UPDATE Games SET IsPublished=false, CanPublish= false WHERE ID = @ID";
                bool isUpdate = await _db.SaveDataAsync(updateQuery, param);
                Console.WriteLine($"The update of game: {gameId} was completed successfully {isUpdate}");
            }
        }

    }
}