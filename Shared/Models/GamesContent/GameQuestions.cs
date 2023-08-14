using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TriangleProject.Shared.Models.GamesContent
{
	public class GameQuestions
	{
		public int ID { get; set; }
		public int GameID { get; set; }
		public string QuestionDescription { get; set; }
		public bool HasImage { get; set; }
		public string QuestionImage { get; set; }
		public string CorrectCategoryName { get; set; }
		public string WrongCategoryName { get; set; }
		public List<GameAnswers> Answers { get; set; }
	}
}