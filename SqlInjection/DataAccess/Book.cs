namespace SqlInjection.DataAccess
{
    public class Book
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string AuthorFirstName { get; set; }
        public required string AuthorLastName { get; set; }
    }
}
