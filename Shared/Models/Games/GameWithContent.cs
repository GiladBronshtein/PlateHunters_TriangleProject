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
		public string GameCode { get; set; }
		public string GameName { get; set; }
		public List<GameAnswers> Answers { get; set; }
		public GameQuestions Questions { get; set; }
	}
}