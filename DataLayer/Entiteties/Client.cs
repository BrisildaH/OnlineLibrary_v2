using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.DataLayer.Entiteties
{
    public class Client
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string FullName { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
		public string? PhotoPath { get; set; }
		public virtual List<ClientBook> ClientBooks { get; set; }

	}

}
