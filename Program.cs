using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Bind Firebase config
builder.Services.Configure<ContactManagerApp.Configuration.FirebaseOptions>(builder.Configuration.GetSection("Firebase"));

// Register application services
builder.Services.AddSingleton<ContactManagerApp.Services.FirebaseService>();
builder.Services.AddSingleton<ContactManagerApp.Services.LocalStorageService>();
builder.Services.AddSingleton<ContactManagerApp.Services.HybridStorageService>();
builder.Services.AddSingleton<ContactManagerApp.Services.AuthService>();

// HttpClient for Firebase Auth REST calls
builder.Services.AddHttpClient("FirebaseAuthClient", client =>
{
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();