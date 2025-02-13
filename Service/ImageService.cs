namespace Farmer_Project.Service
{
    public interface ImageService
    {
        Task<string> SaveImageAndToImgPath(IFormFile image);
    }
}
