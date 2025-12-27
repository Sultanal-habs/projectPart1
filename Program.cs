using projectPart1.Services;
using projectPart1.Data;
using projectPart1.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new AuthenticationFilter(
        builder.Services.BuildServiceProvider().GetRequiredService<ILogger<AuthenticationFilter>>()
    ));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ArtGalleryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".ArtGallery.Session";
});

builder.Services.AddScoped<AuthenticationFilter>();
builder.Services.AddScoped<DatabaseHelper>();
builder.Services.AddScoped<FileUploadService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCors("AllowReact");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var dbHelper = scope.ServiceProvider.GetRequiredService<DatabaseHelper>();
    bool isConnected = await dbHelper.TestConnectionAsync();
    if (isConnected)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Database Connection: SUCCESS");
        Console.WriteLine("Database: ArtGalleryDB");
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ArtGalleryDbContext>();
            bool canConnect = await dbContext.Database.CanConnectAsync();
            Console.WriteLine($"Entity Framework: {(canConnect ? "SUCCESS" : "FAILED")}");
            int userCount = await dbContext.Users.CountAsync();
            Console.WriteLine($"Users in database: {userCount}");
            Console.WriteLine("Session Management: ENABLED (30 min timeout)");
            Console.WriteLine("Authentication Filter: ACTIVE");
            Console.WriteLine("MVC Controllers: ENABLED");
            Console.WriteLine("Web API: ENABLED");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Entity Framework: ERROR - {ex.Message}");
        }
        Console.WriteLine("========================================");
    }
    else
    {
        Console.WriteLine("========================================");
        Console.WriteLine("WARNING: Database connection failed!");
        Console.WriteLine("Check connection string in appsettings.json");
        Console.WriteLine("========================================");
    }
}

app.Run();
