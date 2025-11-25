using projectPart1.Services;
using projectPart1.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Add DatabaseHelper for direct SQL access
builder.Services.AddScoped<DatabaseHelper>();

// Keep legacy services for backward compatibility (will be replaced gradually)
builder.Services.AddSingleton<DataStore>();
builder.Services.AddScoped<ArtworkService>();
builder.Services.AddScoped<ArtistService>();
builder.Services.AddScoped<ExhibitionService>();
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
app.UseCors("AllowReact");
app.UseAuthorization();
app.MapRazorPages();

// Test database connection on startup
using (var scope = app.Services.CreateScope())
{
    var dbHelper = scope.ServiceProvider.GetRequiredService<DatabaseHelper>();
    bool isConnected = await dbHelper.TestConnectionAsync();
    
    if (isConnected)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("Database Connection: SUCCESS");
        Console.WriteLine("Database: ArtGalleryDB");
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
