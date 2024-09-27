using HKMain.Models;
using HKShared.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, Manager")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDBContext _dbContext;
        private readonly ILogger _logger;
        public HomeController(
            UserManager<AppUser> userManager,
            AppDBContext dbContext,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: HomeController

        public ActionResult Index()
        {
            _logger.LogInformation("Login Admin. Dashboard");

            return View();
        }

        public async Task<IActionResult> Member()
        {
            return View();
        }
        public async Task<IActionResult> Contact()
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
