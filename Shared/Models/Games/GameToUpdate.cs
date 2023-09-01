﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleProject.Shared.Models.Games
{
    public class GameToUpdate
    {
        public int ID { get; set; }
        public string GameCode { get; set; }
        public string GameName { get; set; }
        public string QuestionDescription { get; set; }
    }
}
