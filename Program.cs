using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ------------------- Repositorios -------------------
// Registramos todos los repositorios de la app de vacunación
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<string>(connectionString);

builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddTransient<IRepositorioEscuela, RepositorioEscuela>();
builder.Services.AddTransient<IRepositorioFotoEscuela, RepositorioFotoEscuela>();
builder.Services.AddTransient<IRepositorioAlumno, RepositorioAlumno>();
builder.Services.AddTransient<IRepositorioVacuna, RepositorioVacuna>();
builder.Services.AddTransient<IRepositorioRegistroVacunacion, RepositorioRegistroVacunacion>();

// ------------------- Autenticación -------------------
// Registramos el servicio de Auth personalizado
builder.Services.AddTransient<IAuthService, AuthService>();

// Configuración de Autenticación por Cookies 
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

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    // Política para administradores solamente
    options.AddPolicy("Administrador", policy => 
        policy.RequireRole("Admin")); // Tu BD usa el rol "Admin"
});
//---------------------------------------------------//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Necesario para servir CSS, JS y los avatares/fotos subidos

app.UseRouting();

// IMPORTANTE: El orden aquí es crucial
app.UseAuthentication(); // 1. Identifica quién es el usuario
app.UseAuthorization();  // 2. Verifica si tiene permiso

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();