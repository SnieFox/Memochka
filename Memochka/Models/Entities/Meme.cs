 using System.ComponentModel.DataAnnotations;

 namespace Memochka.Models.Entities
{
    public record Meme
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "The title cannot be more than 200 characters long")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Meaning is required")]
        public string Meaning { get; set; }

        [Required(ErrorMessage = "Origins is required")]
        public string Origins { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(2007,2023, ErrorMessage = "Year must be in range between 2007 and 2023")]
        public int Year { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool IsApproved { get; set; }
        public int Views { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }


        public List<MemePicture> MemePictures { get; set; }
        public MemeCategory MemeCategory { get; set; }
        public User User { get; set; }
    }
}
