using System.ComponentModel.DataAnnotations;

namespace Memochka.Models.Entities
{
    public record Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "The title cannot be more than 200 characters long")]
        public string Title { get; set; }
        public int Views { get; set; }
        public DateTime PublicationDate { get; set; }
        public int UserId { get; set; }
        public bool IsApproved { get; set; }

        public User User { get; set; }
        public List<ArticleParagraph> ArticleParagraphs { get; set; }
    }
}
