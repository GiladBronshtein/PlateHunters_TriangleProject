using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleProject.Shared.Models.GamesContent;

namespace TriangleProject.Shared.Models.Games
{
	public class GameWithContent
	{
		public int ID { get; set; }
        public int UserID { get; set; } //maybe we don't need that
        public string GameCode { get; set; }
		public string GameName { get; set; }
        public string QuestionDescription { get; set; }
        public string GameEndMessage { get; set; }
        public string CorrectCategoryName { get; set; }
        public string WrongCategoryName { get; set; }
        public List<GameAnswers> Answers { get; set; }
        public bool IsPublished { get; set; }
    }
}