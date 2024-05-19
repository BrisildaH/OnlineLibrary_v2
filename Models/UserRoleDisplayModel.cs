using Microsoft.AspNetCore.Identity;

namespace OnlineLibrary.Models
{
	public class UserRoleDisplayModel
	{
		public string FullName { get; set; }
		public string Role { get; set; }
		public List<IdentityRole> Roles { get; set; }
	}
}
