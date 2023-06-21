using Memochka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Memochka.Models.Entities;
using Memochka.Models.MemochkaDbContext;
using Memochka.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
using Memochka.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Memochka.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult MainPage() => View();
    }
}