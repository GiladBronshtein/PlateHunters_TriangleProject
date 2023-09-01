using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleProject.Shared.Models.Games
{
    public class GameForCreation
    {
        public int ID { get; set; }
        public string GameName { get; set; }
        public string GameCode { get; set; }
        public bool IsPublished { get; set; }
        public bool CanPublish { get; set; }
    }
}
