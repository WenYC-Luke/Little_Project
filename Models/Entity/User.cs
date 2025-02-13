using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmer_Project.Models.Entity
{
    public enum Role{
        User = 0, 
        Admin = 1
    };

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(60)]
        public string Password { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [EnumDataType(typeof(Role))]
        public Role Role { get; set; } = Role.User;

        public DateTime CreatAt { get; set; }
    }
}
