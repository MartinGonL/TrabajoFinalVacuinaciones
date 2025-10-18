using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TrabajoFinalVacuinaciones.Models;
 

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        
        return View(); 
    }
}