using Farmer_Project.Service.ServiceImpl;
using Farmer_Project.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 註冊 Session 服務
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // 設定 Session 過期時間
    options.Cookie.HttpOnly = true; // 防範 XSS 攻擊
    options.Cookie.IsEssential = true; // 設定為基本 cookie，以便不被禁止
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//建立資料表映射串接
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊Service
builder.Services.AddScoped<AppServiceInitializer, AppServiceInitializerImpl>();
builder.Services.AddScoped<InitCreateDefaultAdmin, InitCreateDefaultAdminImpl>();
builder.Services.AddScoped<ImageService, ImageServiceImpl>();


var app = builder.Build();

// 啟用 Session 中介軟體
app.UseSession();
app.UseAuthorization();  // 啟用授權


using (var scope = app.Services.CreateScope())
{
    // 自動執行遷移與更新資料庫
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();


    // 創建作用域並呼叫初始化邏輯
    var appInitializer = scope.ServiceProvider.GetRequiredService<AppServiceInitializer>();
    appInitializer.initialize();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

//客製化路由
app.MapControllerRoute(
    name: "backend",
    pattern: "Backend/{action=Index}/{id?}",
    defaults: new { controller = "Backend" });

app.MapControllerRoute(
    name: "farmersArticles",
    pattern: "Farmers/Articles/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Farmers", action = "Articles" });

app.MapControllerRoute(
    name: "farmersDetail",
    pattern: "Farmers/FarmerDetail/{FarmersId}/{ArticlesId}",
    defaults: new { controller = "Farmers", action = "FarmerDetail" });

app.MapControllerRoute(
    name: "farmersIndex",
    pattern: "Farmers/FarmersIndex/{CropsType}",
    defaults: new { controller = "Farmers", action = "FarmersIndex" });

app.MapControllerRoute(
    name: "Blog",
    pattern: "Home/Blog/{CropsType?}",
    defaults: new { controller = "Home", action = "Blog" });

//預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
