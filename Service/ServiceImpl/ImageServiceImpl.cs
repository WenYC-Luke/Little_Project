
using System.Security.Cryptography;
using System.Text;

namespace Farmer_Project.Service.ServiceImpl
{
    public class ImageServiceImpl : ImageService
    {
        //圖檔轉路徑，同時存到wwwroot
        public async Task<string> SaveImageAndToImgPath(IFormFile image)
        {
            // 1. 檢查圖片格式
            if (!IsImageFormat(image))
            {
                throw new InvalidOperationException("不支援的圖片格式");
            }

            // 2. 計算圖片的 MD5 編碼
            string md5Hash = ComputeMd5Hash(image);

            // 3. 儲存圖片並獲取儲存路徑
            return await SaveImage(image, md5Hash);

        }

        //判斷圖片格式
        private bool IsImageFormat(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        //圖片內容MD-5編碼
        private string ComputeMd5Hash(IFormFile image)
        {
            //using區塊結束後會釋放MD5占用的資源
            using (var md5 = MD5.Create())
            {
                using (var stream = image.OpenReadStream())  // 讀取檔案流
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        sb.Append(b.ToString("x2")); // 轉換為十六進位格式
                    }
                    return sb.ToString();
                }
            }
        }

        //圖片存進wwwroot
        private async Task<string> SaveImage(IFormFile image, string md5Hash)
        {
            //產生一個唯一的圖片檔案名稱，使用 MD5 作為檔名
            string fileExtension = Path.GetExtension(image.FileName); // 獲取圖片副檔名
            string fileName = md5Hash + fileExtension;  // 以 MD5 哈希值作為檔名

            //設定儲存路徑 (Directory.GetCurrentDirectory() -返回當前應用程式的根目錄路徑)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            // 檢查圖片是否已存在，如果已存在則不儲存並返回現有的檔案名稱
            if (System.IO.File.Exists(filePath))
            {
                return "/images/" + fileName;  // 返回現有圖片的路徑
            }

            // 儲存檔案 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream); // 複製圖片資料到伺服器
            }
            return "/images/" + fileName; ;
        }
    }
}
