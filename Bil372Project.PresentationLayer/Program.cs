using System.Text;
using Bil372Project.BusinessLayer.Services;
using Bil372Project.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Bil372Project.BusinessLayer.FluentValidation;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.BusinessLayer.Dtos.Jwt;
using Bil372Project.BusinessLayer.Services.Jwt;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var aiBaseUrl = builder.Configuration["AiService:BaseUrl"];

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// FluentValidation entegrasyonu
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserSettingsDtoValidator>();
builder.Services.AddScoped<IModelInputService, ModelInputService>();
builder.Services.AddScoped<IAiDietService, AiDietService>();
builder.Services.AddScoped<IDietPlanService, DietPlanService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAdminService, AdminService>();


builder.Services.AddHttpClient<IAiDietService, AiDietService>(client =>
{
    client.BaseAddress = new Uri(aiBaseUrl!);
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Cookie şeması = "Cookies" (CookieAuthenticationDefaults.AuthenticationScheme)
        options.LoginPath = "/Account/Login";     // login olmayanı buraya atar
        options.LogoutPath = "/Account/Logout";   // istersen kullanırsın
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
        
    })
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var secretKey = jwtSection.GetValue<string>("SecretKey") ?? string.Empty;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.GetValue<string>("Issuer"),
            ValidAudience = jwtSection.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// dependency injection
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IUserMeasurementService, UserMeasurementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    var adminOptions = builder.Configuration.GetSection("AdminUser").Get<AdminUserOptions>();

    if (adminOptions is not null && !string.IsNullOrWhiteSpace(adminOptions.Email))
    {
        var appUserService = scope.ServiceProvider.GetRequiredService<IAppUserService>();
        await appUserService.EnsureAdminUserAsync(
            adminOptions.FullName ?? "Admin",
            adminOptions.Email,
            string.IsNullOrWhiteSpace(adminOptions.Password) ? "Admin123!" : adminOptions.Password);
    }
    
    // await Bil372Project.DataAccessLayer.Seed.FakeDataSeeder.SeedAsync(db, 1000);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();  


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// (ileride auth eklenecek)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");



app.Run();  