using IcreamShopCilent.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});




// Cấu hình Authentication (Facebook và Google) và Cookie Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    /*  options.LoginPath = "/Login";
      options.LogoutPath = "/Logout";*/
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Login?handler=Logout";
    options.AccessDeniedPath = "/Auth/Login";
})
.AddFacebook(options =>
{
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
    options.Scope.Add("email");
    options.Fields.Add("email");
    options.SaveTokens = true;
    options.CallbackPath = "/signin-facebook";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SaveTokens = true;
    options.CallbackPath = "/signin-google";
});

// Thêm Authorization
builder.Services.AddAuthorization();

// Thêm Razor Pages, HttpClient và CORS
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://localhost:7068")
              .AllowAnyHeader()
              .AllowAnyMethod()
        .AllowCredentials();

    });
});

var app = builder.Build();

// Cấu hình pipeline xử lý request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseCors("AllowSpecificOrigins");
app.UseRouting();

// Thêm Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


// Ánh xạ Razor Pages
app.MapRazorPages();


app.Run();