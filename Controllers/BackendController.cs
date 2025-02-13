using Farmer_Project.Models.Entity;
using Farmer_Project.Models.ViewModel;
using Farmer_Project.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace Farmer_Project.Controllers
{
    public class BackendController : Controller
    {

        private readonly AppDbContext _dbContext; //注入AppDbContext(調用操作資料表的CRUD)
        private readonly ImageService _imageService; //注入自訂義服務(調用圖檔1.判斷格式 2.存入wwwroot 3.返回路徑)


        //建構子
        public BackendController(AppDbContext dbContext, ImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }

        //後台管理頁面(主頁)
        [HttpGet]
        public async Task<IActionResult> DashBoard(string? show)
        {
            // 如果未登入或不是 Admin，則重定向到登入頁面
            if (HttpContext.Session.GetString("IsLoggedIn") != "true" || HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("login", "Home");
            }

            var farmers = await _dbContext.FarmersInfo
                .Include(f => f.FarmersArticles)
                .ToListAsync();

            foreach (var farmer in farmers)
            {
                farmer.FarmersArticles ??= new List<FarmersArticles>();
            }

            var members = await _dbContext.User.ToListAsync();

            var viewModel = new DashboardViewModel
            {
                FarmersInfo = farmers,
                MemberInfo = members
            };

            //區域切換
            if (!string.IsNullOrEmpty(show))
            {
                TempData["show"] = show;
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SendEmail(string message, string actionType)
        {
            if (actionType == "yes")
            {
                if (string.IsNullOrEmpty(message))
                {
                    TempData["LabelResult"] = "請輸入有效的訊息內容！"; // 發送失敗的提示
                    return RedirectToAction("DashBoard", "Backend");
                }

                try
                {
                    TempData["messageBtn"] = true;
                    using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential("a08265214@gmail.com", "muyl viyu lrtj ndoa");
                        smtpClient.EnableSsl = true;

                        var accounts = _dbContext.User.Select(m => m.Email).ToList();

                        if (!accounts.Any())
                        {
                            TempData["LabelResult"] = "没有找到任何收件人地址！";
                        }

                        foreach (var account in accounts)
                        {
                            try
                            {
                                using (MailMessage mail = new MailMessage())
                                {
                                    mail.From = new MailAddress("a08265214@gmail.com");
                                    mail.Subject = "測試郵件";
                                    mail.Body = message;
                                    mail.IsBodyHtml = false;
                                    mail.To.Add(account);

                                    smtpClient.Send(mail);
                                }

                                TempData["LabelResult"] = "信件發送成功！"; // 發送成功的提示
                            }
                            catch (Exception innerEx)
                            {
                                TempData["LabelResult"] = "信件發送失敗！錯誤信息：" + innerEx.Message;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["LabelResult"] = "信件發送失敗！錯誤信息：" + ex.Message;
                }
            }
            TempData["show"] = "MemberTable";
            return RedirectToAction("DashBoard", "Backend");
        }
       
        [HttpPost]
        public IActionResult logout()
        {
            
            HttpContext.Session.Clear();

            return RedirectToAction("index", "Home");
        }



    }
}
