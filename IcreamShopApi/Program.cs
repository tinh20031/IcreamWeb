using IcecreamShopApi.Services;
using IcreamShopApi;
using IcreamShopApi.Data;
using IcreamShopApi.Models;
using IcreamShopApi.Repository;
using IcreamShopApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// C?u hình d?ch v? cho Razor Pages và API
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.Configure<VNPAYConfig>(builder.Configuration.GetSection("VNPAY")); builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderDetailRepository>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderDetailService>();
builder.Services.AddScoped<OrderDetailRepository>();
builder.Services.AddControllers().AddOData(opt => opt
    .AddRouteComponents("odata", GetEdmModel())
    .Filter()     
    .Select()      
    .Expand()   
    .OrderBy()      
    .Count()       
    .SetMaxTop(100) 
);

builder.Services.AddSignalR();
IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<IceCream>("IceCreams"); 
    builder.EntitySet<Category>("Categories"); 
    return builder.GetEdmModel();
}

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});



// C?u hình Authentication s? d?ng JWT Bearer

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});


builder.Services.AddDbContext<CreamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {



        policy.WithOrigins("https://localhost:7068")  // Thay thế bằng URL frontend của bạn

              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IceCreamRepository>();
builder.Services.AddScoped<IceCreamService>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CartRepository>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderDetailRepository>();
builder.Services.AddScoped<OrderDetailService>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<AddressRepository>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddSwaggerGen(c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Sử dụng CORS với policy đã cấu hình
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHub<ChatHub>("/chatHub");

app.UseStaticFiles();

app.UseAuthentication();

app.UseCors("AllowSpecificOrigins");    
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();
