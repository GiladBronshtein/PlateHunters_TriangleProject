using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleProject.Shared.Models.Users
{
	public class UserWithGames
	{
        public string FirstName { get; set; }
        public List<string> Games { get; set; }
    }
}
