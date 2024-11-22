using Microsoft.AspNetCore.Mvc;
using PROG3.Models;
using System.Diagnostics;

namespace PROG3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Homepage
        public IActionResult Index()
        {
            return View();
        }
    
    }
}
