using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memochka.Models.Entities
{
    public record User
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "The FirstName cannot be more than 50 characters long")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "The LastName cannot be more than 50 characters long")]
        public string? LastName { get; set; }

        [StringLength(20, ErrorMessage = "The Nickname cannot be more than 20 characters long")]
        public string Nickname { get; set; }
        public int RoleId { get; set; }

        [StringLength(25, ErrorMessage = "The Login cannot be more than 20 characters long")]
        public string Login { get; set; }

        [StringLength(25, ErrorMessage = "The Password cannot be more than 20 characters long")]
        public string Password { get; set; }

        public Role? Role { get; set; }
        public List<Meme>? Memes { get; set; }
        public List<Article>? Articles { get; set; }

    }
}
