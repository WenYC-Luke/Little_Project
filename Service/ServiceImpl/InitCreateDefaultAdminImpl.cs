using Farmer_Project.Models.Entity;

namespace Farmer_Project.Service.ServiceImpl
{
    public class InitCreateDefaultAdminImpl : InitCreateDefaultAdmin
    {
        //注入資料庫操作
        private readonly AppDbContext _dbContext;
        public InitCreateDefaultAdminImpl(AppDbContext dbContext) { 
            _dbContext = dbContext;
        }

        //建立預設管理員帳戶方法
        public void CreateDefaultAdmin()
        {
            //判斷資料表有無管理員角色
            var hasAdmin = _dbContext.User.Any(a => a.Role == Role.Admin); 
            if (hasAdmin)
            {
                return;
            }
            else
            {
                var defaultAdmin = new User
                {
                    Name = "Default_Admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("123456"), 
                    Phone = "0912-XXXOOO",
                    Role = Role.Admin,
                    CreatAt = DateTime.Now
                };
                _dbContext.User.Add(defaultAdmin);
                _dbContext.SaveChanges();
            }
            
        }
    }
}
