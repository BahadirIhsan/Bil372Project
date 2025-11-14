var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();   // BU OLMALI (statik dosyaları wwwroot'tan servis eder)

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// (ileride auth eklenecek)
// app.UseAuthentication();
// app.UseAuthorization();

// Şimdilik giriş ekranı default olsun
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();