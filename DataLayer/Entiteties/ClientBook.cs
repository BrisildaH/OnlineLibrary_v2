using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineLibrary.DataLayer.Entiteties
{
    public class ClientBook
    {

        [Key]
        public int Id { get; set; }
        [ForeignKey("Client")]
        public int ClientID { get; set; }
        public virtual Client? Client { get; set; }
        [ForeignKey("Book")]
        public int BookID { get; set; }
        public virtual Book? Book { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDelete { get; set; }
        public ClientBook()
        {
            CreatedDate = DateTime.Now;
            Status = "New";
        }
    }
}

