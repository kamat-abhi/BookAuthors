namespace BookAuthors.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = String.Empty;

        public ICollection<Author> Authors { get; set; } = new HashSet<Author>();
    }
}
