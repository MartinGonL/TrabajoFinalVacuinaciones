using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySql");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'MySql' no encontrada. Verifica appsettings.json y el entorno.");
}

builder.Services.AddControllersWithViews();

// Repositorios con fábrica para pasar connectionString
builder.Services.AddTransient<IRepositorioUsuario>(sp => new RepositorioUsuario(connectionString));
builder.Services.AddTransient<IRepositorioEscuela>(sp => new RepositorioEscuela(connectionString));
builder.Services.AddTransient<IRepositorioFotoEscuela>(sp => new RepositorioFotoEscuela(connectionString));
builder.Services.AddTransient<IRepositorioAlumno>(sp => new RepositorioAlumno(connectionString));
builder.Services.AddTransient<IRepositorioVacuna>(sp => new RepositorioVacuna(connectionString));
builder.Services.AddTransient<IRepositorioRegistroVacunacion>(sp => new RepositorioRegistroVacunacion(connectionString));

// Auth service
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Auth/Login";
        options.LogoutPath       = "/Auth/Logout";
        options.AccessDeniedPath = "/Home/AccesoDenegado"; 
        options.Cookie.Name      = "Vacunacion.Auth"; 
        options.Cookie.HttpOnly  = true;
        options.SlidingExpiration = true;        
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

Console.WriteLine($"Aplicación arrancada. Entorno: {app.Environment.EnvironmentName}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();