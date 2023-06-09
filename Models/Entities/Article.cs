namespace Memochka.Models.Entities
{
    public record Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Views { get; set; }
        public DateTime PublicationDate { get; set; }
        public int UserId { get; set; }
        public int MainPictureId { get; set; }

        public User User { get; set; }
        public List<ArticleParagraph> ArticleParagraphs { get; set; }
    }
}
