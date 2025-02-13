namespace Farmer_Project.Models.ViewModel
{
    public class FarmersInfoViewModel
    {
        public int FarmersId { set; get; }

        public string Name { set; get; }

        public string FarmName { set; get; }

        public IFormFile? Image { set; get; }

        public string? ImagePath { set; get; }

        public string Email { set; get; }

        public string Phone { set; get; }

        public string Address { set; get; }

        public string CropsType { set; get; }

        public string PlantType { set; get; }
    }
}
