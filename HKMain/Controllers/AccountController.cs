using Microsoft.AspNetCore.Mvc;

namespace HKMain.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string returnUrl)
        {
            return RedirectToAction(nameof(AccountController.Login), "Account", new { area = "Admin" });
        }
        public IActionResult AccessDenied(string returnUrl)
        {
            return RedirectToAction(nameof(AccountController.AccessDenied), "Account", new { area = "Admin" });
        }
    }
}
