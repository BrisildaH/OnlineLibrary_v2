using OnlineLibrary.DataLayer.Entiteties;

namespace OnlineLibrary.Models
{
    public class RentBookModel
    {
        public int ClientID { get; set; }
        public int BookID { get; set; }
        public List<Client>Clients { get; set; }
        public List<Book> Books { get; set; }
    }
}
