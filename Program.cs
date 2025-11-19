using projectPart1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
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

using (var scope = app.Services.CreateScope())
{
    var dataStore = scope.ServiceProvider.GetRequiredService<DataStore>();
    Console.WriteLine("========================================");
    Console.WriteLine("تحقق من البيانات:");
    int artworksCount = dataStore.Artworks.Count;
    int artistsCount = dataStore.Artists.Count;
    int exhibitionsCount = dataStore.Exhibitions.Count;
    Console.WriteLine($"عدد الأعمال الفنية: {artworksCount}");
    Console.WriteLine($"عدد الفنانين: {artistsCount}");
    Console.WriteLine($"عدد المعارض: {exhibitionsCount}");
    Console.WriteLine("========================================");
}

app.Run();
