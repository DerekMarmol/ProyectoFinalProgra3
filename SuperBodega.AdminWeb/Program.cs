var builder = WebApplication.CreateBuilder(args);

// Add services to the container before building the app.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("SuperBodegaAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
