namespace Memochka.Models.Entities
{
    public record User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Nickname { get; set; }
        public int RoleId { get; set; }
        public int? ProfilePictureId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Role? Role { get; set; }
        public List<Meme>? Memes { get; set; }
        public List<Article>? Articles { get; set; }

    }
}
