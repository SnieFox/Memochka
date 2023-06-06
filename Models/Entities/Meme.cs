namespace Memochka.Models.Entities
{
    public class Meme
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Meaning { get; set; }
        public string Origins { get; set; }
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
