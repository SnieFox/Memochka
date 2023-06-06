namespace Memochka.Models.Entities
{
    public class MemePicture
    {
        public Guid Id { get; set; }
        public int MemeId { get; set; }
        public int PictureId { get; set; }

        public Meme Meme { get; set; }
    }
}
