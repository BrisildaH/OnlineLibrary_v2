using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.DataLayer.Entiteties
{
    public class Book
    {
        [Key]

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        [Required]
        public string Category { get; set; }
        [ForeignKey("Author")]
        public int AuthorID { get; set; }

        public virtual Author Author { get; set; }

        public string ISBN { get; set; }


    }
}
