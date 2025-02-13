using Farmer_Project.Helpers;
using Farmer_Project.Models;
using Farmer_Project.Models.Entity;
using Farmer_Project.Models.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Farmer_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly EmailHelper _emailHelper;
        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _emailHelper = new EmailHelper();
        }

        public IActionResult index()
        {
            return View();
        }

        public IActionResult about()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult crop()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        //部落格
        [HttpGet]
        public IActionResult Blog(string? CropsType)
        {
            ViewData["BodyClass"] = "sub_page";

            var articleQuery = _dbContext.FarmersArticles
                        .Include(a => a.FarmersInfo)
                        .Where(a => a.ArticleType == "一般文章" && a.IsPublished == true);

            //根據CropsType篩選文章
            if (!string.IsNullOrEmpty(CropsType) && CropsType != "All")
            {
                articleQuery = articleQuery.Where(a => a.FarmersInfo.CropsType == CropsType);
            }

            var article = articleQuery.OrderByDescending(a => a.CreatedDate).ToList();

            return View(article);
        }


        public IActionResult testimonial()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        public IActionResult contact()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        //會員登入
        public IActionResult login()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> login(UserLoginViewModel model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (model != null) {
                var user = await _dbContext.User.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (user == null) {
                    ModelState.AddModelError("Email", "帳號不存在，請選擇正確帳號。");
                    return View(model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    ModelState.AddModelError("Password", "密碼錯誤，請再次確認。");
                    return View(model);
                }

                // 設置 Session 來標記用戶登入狀態
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", user.Role.ToString());
                HttpContext.Session.SetString("UserEmail", user.Email);

                if (user.Role == Role.Admin)
                {
                    return RedirectToAction("DashBoard", "Backend");
                }
                else if (user.Role == Role.User) {

                    TempData["success"] = "您已成功登入！";
                    return RedirectToAction("index", "Home");
                }

            }

            return View("index");
        }

        //會員登出
        [HttpGet]
        public IActionResult logout()
        {
            // 清除 Session 資料
            HttpContext.Session.Clear();

            TempData["success"] = "您已成功登出！";
            return RedirectToAction("index");
        }


        //會員註冊
        [HttpGet]
        public IActionResult register()
        {
            //
            ViewData["BodyClass"] = "sub_page";
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> register(UserRegisterViewModel model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid)
            {
                //判斷帳號是否存在
                var existUser = await _dbContext.User.FirstOrDefaultAsync(u => u.Email == model.Email);
                
                //如果帳號存在
                if (existUser != null)
                {
                    ModelState.AddModelError("Email", "此信箱已經註冊過，請選擇其他帳號。");
                    return View(model);
                }

                //如果帳號不存在
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password); // 密碼加密
            
                User newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = passwordHash,
                    Phone = string.IsNullOrWhiteSpace(model.Phone) ? "未填寫" : model.Phone,
                    Role = model.Role,
                    CreatAt = DateTime.Now
                };

                _dbContext.User.Add(newUser);
                await _dbContext.SaveChangesAsync();

                TempData["success"] = "註冊成功，請登入！";
                return RedirectToAction("login");                  
            }
            TempData["error"] = "資料輸入錯誤，請確認!";
            return View(model);
        }

        //忘記密碼
        [HttpGet]
        public IActionResult forgetPassword()
        {
            ViewData["BodyClass"] = "sub_page";
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> forgetPassword(UserLoginViewModel model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid) { 
                var existUser = await _dbContext.User.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (existUser == null)
                {
                    ModelState.AddModelError("Email", "帳號錯誤，請重新輸入帳號");
                    return View(model);
                }

                try
                {
                    // 使用 BCrypt 雜湊密碼
                    existUser.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    _dbContext.User.Update(existUser);
                    await _dbContext.SaveChangesAsync();

                    TempData["success"] = "密碼已成功更新！請使用新密碼登入。";
                    return RedirectToAction("login");
                }
                catch
                {
                    ModelState.AddModelError("", "發生錯誤，請稍後再試！");
                    return View(model);
                }
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult Contact(ContactMessage model)
        {
            ViewData["BodyClass"] = "sub_page";

            if (ModelState.IsValid)
            {
                var subject = "感謝您的聯絡！";
                var body = $"親愛的 {model.Name}，<br><br>我們已收到您的訊息：<br><blockquote>{model.Message}</blockquote>";

                try
                {
                    _emailHelper.SendEmail(model.Email, subject, body);
                    ViewBag.Message = "您的訊息已成功發送！";
                }
                catch
                {
                    ViewBag.Message = "發送郵件失敗，請稍後再試。";
                }
            }

            return View();
        }




    }
}
