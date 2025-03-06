using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Authentication (Facebook và Google) và Cookie Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;  // Mặc định là Cookie
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Login"; // Chỉ định đường dẫn khi chưa đăng nhập
    options.LogoutPath = "/Logout"; // Chỉ định đường dẫn khi đăng xuất
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];  // Lấy từ appsettings.json hoặc secrets
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];  // Lấy từ appsettings.json hoặc secrets
    options.Scope.Add("email");  // Lấy email của người dùng
    options.Fields.Add("email"); // Cung cấp quyền truy cập email
    options.SaveTokens = true;   // Lưu lại token khi login thành công
    options.CallbackPath = "/signin-facebook";  // Callback URI sau khi login thành công
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];  // Lấy từ appsettings.json hoặc secrets
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];  // Lấy từ appsettings.json hoặc secrets
    options.SaveTokens = true;  // Lưu lại token khi login thành công
    options.CallbackPath = "/signin-google"; // Callback URI sau khi login thành công
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
