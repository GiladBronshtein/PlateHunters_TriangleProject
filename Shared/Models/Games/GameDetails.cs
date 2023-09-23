using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleProject.Shared.Models.GamesContent;

namespace TriangleProject.Shared.Models.Games
{
    public class GameDetails
    {
        public int ID { get; set; }
        public string GameCode { get; set; }

        [Required(ErrorMessage = "Game name is required")]
        [StringLength(30, ErrorMessage = "A game max name can have 30 chars")]
        [MinLength(3, ErrorMessage = "A game min name can have 3 chars")]
        public string GameName { get; set; }
        public int UserID { get; set; }
        public string GameEndMessage { get; set; }
        public bool IsPublished { get; set; }
        public bool CanPublish { get; set; }
        public string QuestionDescription { get; set; }
        public bool QuestionHasImage { get; set; }
        public string QuestionImageText { get; set; }
        public string QuestionCorrectCategory { get; set; }
        public string QuestionWrongCategory { get; set; }
        public List<GameAnswers> Answers { get; set; }
        public List<GameAnswers> AnswersToDelete { get; set; }
    }
}