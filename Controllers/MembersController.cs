using Farmer_Project.Models.Entity;
using Farmer_Project.Models.ViewModel;
using Farmer_Project.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Farmer_Project.Controllers
{
    public class MembersController : Controller
    {
        
        private readonly AppDbContext _dbContext; 
        private readonly ImageService _imageService; 


        //建構子
        public MembersController(AppDbContext dbContext, ImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }

        //會員搜尋
        public IActionResult Search(string search)
        {
            var membersQuery = _dbContext.User.AsQueryable();

            if (!string.IsNullOrEmpty(search?.Trim()))
            {
                membersQuery = membersQuery.Where(m => m.Name.Contains(search) || m.Email.Contains(search));
            }
            var members = membersQuery.ToList();


            var allFarmers = _dbContext.FarmersInfo.Include(fa => fa.FarmersArticles)
                                             .ThenInclude(fad => fad.FarmersArticlesDetails)
                                             .ToList();

            // 創建 ViewModel
            var viewModel = new DashboardViewModel
            {
                MemberInfo = members,
                FarmersInfo = allFarmers
            };

            TempData["show"] = "MemberTable";
            return View("~/Views/Backend/DashBoard.cshtml", viewModel);
        }

        [HttpPost]
        public IActionResult SearchCancel()
        {
            // 重新抓取所有會員資料
            var allMembers = _dbContext.User.ToList();
            var allFarmers = _dbContext.FarmersInfo.Include(fa => fa.FarmersArticles)
                                            .ThenInclude(fad => fad.FarmersArticlesDetails)
                                            .ToList();
            // 創建 ViewModel
            var viewModel = new DashboardViewModel
            {
                MemberInfo = allMembers,
                FarmersInfo = allFarmers
            };

            TempData["show"] = "MemberTable";
            return View("~/Views/Backend/DashBoard.cshtml", viewModel);
        }


        //會員新增
        [HttpGet]
        public IActionResult Create()
        {
            var member = new UserRegisterViewModel();

            return View(member);
        }    

        [HttpPost]
        public IActionResult Create(UserRegisterViewModel model)
        {
            //判斷是否存在帳號
            var hasMember = _dbContext.User.Any(u => u.Email == model.Email);

            if (hasMember)
            {
                TempData["Message"] = "此帳號已註冊";
                return View(model);
            }
            
            // 創建新使用者物件
            var newUser = new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password), // 雜湊密碼（使用 BCrypt）
                Role = model.Role,
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? "未填寫" : model.Phone
            };

            // 新增至資料庫
            _dbContext.User.Add(newUser);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "會員新增成功！";
            TempData["show"] = "MemberTable";
            return RedirectToAction("DashBoard", "Backend");
           
        }

        //會員編輯
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var member = _dbContext.User.FirstOrDefault(u => u.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                Id = member.Id,
                Role = member.Role.ToString(),
                Name = member.Name,
                Password = member.Password,
                Email = member.Email,
                Phone = member.Phone ?? "未填寫"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(UserEditViewModel model ,int id)
        {
            var member = _dbContext.User.FirstOrDefault(u => u.Id == id);
            if (member == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(model.Role))
            {
                member.Role = Enum.Parse<Role>(model.Role);
            }

            if (!string.IsNullOrEmpty(model.Name)) member.Name = model.Name;
            if (!string.IsNullOrEmpty(model.Email)) member.Email = model.Email;
            if (!string.IsNullOrEmpty(model.Password)) member.Password = model.Password;
            if (!string.IsNullOrEmpty(model.Phone)) member.Phone = model.Phone;

            _dbContext.SaveChanges();

            TempData["show"] = "MemberTable";
            return RedirectToAction("DashBoard", "Backend");
        }

        [HttpPost]
        public IActionResult Delete(int MemberId)
        {
            var member = _dbContext.User.FirstOrDefault(u => u.Id == MemberId);
            
            if (member == null)
            {
                return NotFound();
            }

            // 刪除會員資料
            _dbContext.User.Remove(member);
            _dbContext.SaveChanges();

            TempData["show"] = "MemberTable";
            return RedirectToAction("DashBoard", "Backend");
        }



    }
}
