using System.ComponentModel.DataAnnotations;

namespace Memochka.Models.Entities
{
    public record ArticleParagraph
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "The title cannot be more than 200 characters long")]
        public string ParagraphTitle { get; set; }
        public string Description { get; set; }
        public int ArticleId { get; set; }

        public Article Article { get; set; }
    }
}
