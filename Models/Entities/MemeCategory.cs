namespace Memochka.Models.Entities
{
    public class MemeCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }

        public List<Meme> Memes { get; set; }
    }
}
