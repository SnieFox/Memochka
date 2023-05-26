namespace Memochka.Models
{
    public class News
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Views { get; set; }

        public News(int number, string title, string description, string views)
        {
            number = Number;
            Title = title;
            Description = description;
            Views = views;
        }
    }
}
