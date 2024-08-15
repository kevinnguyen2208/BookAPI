namespace API.Models
{
    public class BookDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }
    }

    public class BookWithIdDto : BookDto
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class BookViewModel : BookDto 
    {
        public int Id { get; set; }
    }
}
