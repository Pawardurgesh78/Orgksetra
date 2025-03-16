using Microsoft.EntityFrameworkCore;
using OrgkSetra;
using OrgkSetra.Data;
using OrgkSetra.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiService>();
builder.Services.AddDbContext<CartDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CartConnection")));
builder.Services.AddDbContext<OrgksetraAdmin>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AdminConnection")));
builder.Services.AddScoped<ICartItemRepository, ManagerCartItem>();
builder.Services.AddScoped<ManagerCartItem>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set timeout
    options.Cookie.HttpOnly = true; // Make the cookie accessible only via HTTP
    options.Cookie.IsEssential = true; // Ensure the cookie is set even if the user hasn't consented to cookies
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();   
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
