using System.ComponentModel.DataAnnotations.Schema;

namespace Soundy.CatalogService.Entities
{
    [NotMapped]
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
    }
}
