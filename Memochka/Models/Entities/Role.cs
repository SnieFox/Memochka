namespace Memochka.Models.Entities
{
    public record Role
    {
        public int Id { get; set; }
        public string Roles { get; set; }

        public List<User> Users { get; set; }
    }
}
