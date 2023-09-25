using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleProject.Shared.Models.Games
{
    public class GameToAdd
    {
        [Required(ErrorMessage = "יש להזין שם משחק")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "שם המשחק חייב להכיל 2 תווים לפחות")]
        public string GameName { get; set; }
    }
}
