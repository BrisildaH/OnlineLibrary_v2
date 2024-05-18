using Microsoft.AspNetCore.Identity;

namespace OnlineLibrary.DataLayer.Entiteties
{
    public class User: IdentityUser
    {
        public string FullName { get; set; }
        //public string Email { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public User()
        {
            IsActive = true;
        }
    }
}
