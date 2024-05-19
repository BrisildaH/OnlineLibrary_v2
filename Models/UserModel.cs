using Microsoft.AspNetCore.Identity;
using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.Models
{
	public class UserModel:User
	{
		public virtual List<string> Roles { get; set; }
		
	}
}
