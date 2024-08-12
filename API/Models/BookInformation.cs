namespace API.Models
{
    public class BookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string PublishedDate { get; set; }
    }

    public class BookWithIdDto : BookDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class BookViewModel : BookDto 
    {
        public Guid Id { get; set; }
    }
}
