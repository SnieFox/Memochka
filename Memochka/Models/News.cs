namespace Memochka.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Views { get; set; }

        public News(int id, string title, string description, string views)
        {
            Id = id;
            Title = title;
            Description = description;
            Views = views;
        }
    }
}
