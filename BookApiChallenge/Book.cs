namespace BookApiChallenge
{

    // Represents basic book with ID, title, publication status and publication date.

    public class Book
    {

        public int BookID { get; set; }
        public string BookTitle { get; set; }
        public string PublicationStatus { get; set; }
        public DateTime? PublicationDate { get; set; }

    }
}
