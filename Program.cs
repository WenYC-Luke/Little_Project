using Farmer_Project.Service.ServiceImpl;
using Farmer_Project.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ���U Session �A��
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120); // �]�w Session �L���ɶ�
    options.Cookie.HttpOnly = true; // ���d XSS ����
    options.Cookie.IsEssential = true; // �]�w���� cookie�A�H�K���Q�T��
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//�إ߸�ƪ�M�g�걵
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���UService
builder.Services.AddScoped<AppServiceInitializer, AppServiceInitializerImpl>();
builder.Services.AddScoped<InitCreateDefaultAdmin, InitCreateDefaultAdminImpl>();
builder.Services.AddScoped<ImageService, ImageServiceImpl>();


var app = builder.Build();

// �ҥ� Session �����n��
app.UseSession();
app.UseAuthorization();  // �ҥα��v


using (var scope = app.Services.CreateScope())
{
    // �۰ʰ���E���P��s��Ʈw
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();


    // �Ыا@�ΰ�éI�s��l���޿�
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

//�Ȼs�Ƹ���
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

//�w�]����
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
