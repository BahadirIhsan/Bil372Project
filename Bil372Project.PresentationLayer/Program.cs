using Bil372Project.BusinessLayer.Services;
using Bil372Project.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Bil372Project.BusinessLayer.FluentValidation;
using Bil372Project.BusinessLayer.Dtos;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// FluentValidation entegrasyonu
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserSettingsDtoValidator>();
builder.Services.AddScoped<IModelInputService, ModelInputService>();
builder.Services.AddScoped<IAiDietService, AiDietService>();
builder.Services.AddScoped<IDietPlanService, DietPlanService>();



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
// authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Cookie şeması = "Cookies" (CookieAuthenticationDefaults.AuthenticationScheme)
        options.LoginPath = "/Account/Login";     // login olmayanı buraya atar
        options.LogoutPath = "/Account/Logout";   // istersen kullanırsın
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
    });


// dependency injection
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IUserMeasurementService, UserMeasurementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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