using Memochka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Memochka.Models.MemochkaDbContext;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MemochkaContext _context;
        public HomeController(ILogger<HomeController> logger, MemochkaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult MainPage()
        {
            return View();
        }

        public IActionResult Articles()
        {
            return View();
        }
        public IActionResult Memes()
        {
            return View();
        }
        public IActionResult MemesOffer()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}