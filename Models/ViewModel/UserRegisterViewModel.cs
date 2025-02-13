
using Farmer_Project.Models.Entity;

namespace Farmer_Project.Models.ViewModel
{
    public class UserRegisterViewModel
    {
        public Role Role { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

