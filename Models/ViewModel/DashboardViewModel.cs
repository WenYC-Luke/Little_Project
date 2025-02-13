using Farmer_Project.Models.Entity;

namespace Farmer_Project.Models.ViewModel
{
    public class DashboardViewModel
    {
        public List<FarmersInfo> FarmersInfo { get; set; } = new List<FarmersInfo>();
        public List<User> MemberInfo { get; set; } = new List<User>();
    }
}
