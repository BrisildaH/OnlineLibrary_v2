using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.DataLayer.Entiteties
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        
        public string? Description{ get; set; }
        public int? NumberOfBooks { get; set; }

        public virtual List <Book> Books { get; set; }
    }
}
