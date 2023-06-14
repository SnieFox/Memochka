namespace Memochka.Models.Entities
{
    public record MemePicture
    {
        public int Id { get; set; }
        public int MemeId { get; set; }
        public string PictureId { get; set; }

        public Meme Meme { get; set; }
    }
}
