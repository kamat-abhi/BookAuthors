namespace BookAuthors.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = String.Empty;

        public ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }
}
