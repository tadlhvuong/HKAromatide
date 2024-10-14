using DocumentFormat.OpenXml.Spreadsheet;
using HKMain.Areas.Admin.Models;
using HKMain.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HKMain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        private readonly string mediaUrl;
        private readonly string mediaPath;

        public HomeController(ILogger<HomeController> logger, AppDBContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;

            mediaUrl = AppSettings.Strings["MediaUrl"] ?? "./media";
            mediaPath = AppSettings.Strings["MediaPath"] ?? "./wwwroot/media";
        }

        [Route("")]
        [Route("trang-chu")]
        public IActionResult Index()
        {
            var model = _dbContext.Products.Include(x => x.MediaAlbum).Include(x => x.MediaAlbum.MediaFiles).Where(x => x.Status == ProductStatus.Published).ToList();

            foreach (var product in model)
            {
                product.Image = (product.MediaAlbum.MediaFiles.FirstOrDefault() != null) ? product.MediaAlbum.MediaFiles.FirstOrDefault().FullPath : "/images/logo.webp";
                var FullPath = Path.Combine(mediaUrl, product.Image);
                product.LinkImage = FullPath;
            }
            var sale = model.Where(x => x.SalePrice > 0 || x.Status == ProductStatus.Published || x.Stock == true).ToList();
            ViewBag.SaleProducts = sale;
            return View(model);
            //Admin
            //return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Admin" });
        }
        [Route("ve-aromatide")]
        public IActionResult AboutUs()
        {
            return View();

        }
        [Route("chuyen-gia")]
        public IActionResult Terminal()
        {
            return View();

        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Subscribers()
        {
            return View();

        }

        [Route("chinh-sach-mua-hang-va-thanh-toan")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("chinh-sach-tra-hang-va-hoan-tien")]
        public IActionResult Refund()
        {
            return View();
        }


        [Route("chinh-sach-bao-mat-thong-tin")]
        public IActionResult Security()
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