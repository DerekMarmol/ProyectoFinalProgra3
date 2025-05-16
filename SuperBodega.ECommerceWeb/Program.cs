var builder = WebApplication.CreateBuilder(args);

// Add services to the container before building the app.
builder.Services.AddControllersWithViews();

// Agregar servicio de HttpClient
builder.Services.AddHttpClient("SuperBodegaAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:8080/");
});

// Agregar servicio de sesión para el carrito
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseRouting();

// Usar la sesión después de UseRouting
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
