using HKMain.Services;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("MySQLConnection");
var passDB = configuration["DB:Password"];

connectionString = string.Concat(connectionString, passDB);
services.AddDbContext<AppDBContext>(options =>
    options.UseMySql(connectionString, (ServerVersion.AutoDetect(connectionString)), b => b.MigrationsAssembly("HKMain")));

services.Configure<CookiePolicyOptions>(options =>
{
    options.ConsentCookie.IsEssential = true;
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

services.Configure<AppSetting>(configuration.GetSection("AppSetting"));

services.AddIdentity<AppUser, AppRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;

    options.User.RequireUniqueEmail = true;

    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

services.AddHttpClient();
services.AddMvc().AddControllersAsServices();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie("HKAromatideCookie", options =>
{
    options.Cookie.Name = "HKAromatideCookie";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
});
services.AddAuthentication()
    .AddFacebook(fbOptions =>
    {
        fbOptions.AppId = configuration["Authentication:Facebook:AppId"];
        fbOptions.AppSecret = configuration["Authentication:Facebook:AppSecret"];
    }).AddGoogle(ggOptions =>
    {
        ggOptions.ClientId = configuration["Authentication:Google:ClientId"];
        ggOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30); // Set session timeout
});
//services.AddAuthorization(option =>
//{
//    option.AddPolicy("AllowEdit", policyBuilder => //tao phan quyen gop nhieu role
//    {
//        policyBuilder.RequireRole("Admin");//phan quyen theo role
//        policyBuilder.RequireRole("Manager");

//        policyBuilder.RequireClaim("nameclaim", "value"); //phan quyen theo dac tinh (write, read,..)
//        policyBuilder.RequireClaim("canedit", "user");
//    });
//});

services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
var env = hostingContext.HostingEnvironment;
config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
.AddJsonFile($"appsettings.{ env.EnvironmentName}.json", optional: true, reloadOnChange: true);
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
app.UseAuthentication();
app.UseAuthorization();
app.InitAppSettings();
app.UseRequestLocalization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "Product",
      pattern: "{area:exists}/san-pham/{id?}", new
      {
          controller = "Product",
          action = "Index"
      }
    );
    endpoints.MapControllerRoute(
      name: "Media",
      pattern: "{area:exists}/thu-vien/{id?}", new
      {
          controller = "Media",
          action = "Index"
      }
    );
    endpoints.MapControllerRoute(
      name: "Member",
      pattern: "{area:exists}/thanh-vien/{id?}", new
      {
          controller = "Member",
          action = "Index"
      }
    );
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
