﻿using Memochka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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