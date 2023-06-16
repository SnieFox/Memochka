namespace Memochka.Models.Entities
{
    public record ArticleParagraph
    {
        public int Id { get; set; }
        public string ParagraphTitle { get; set; }
        public string Description { get; set; }
        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}
