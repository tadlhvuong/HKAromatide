using HKMain.Areas.Admin.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDBContext _dbContext;
        private readonly ILogger _logger;
        public AccountController(
          UserManager<AppUser> userManager,
          SignInManager<AppUser> signInManager,
          AppDBContext dbContext,
          ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: Admin

        public ActionResult Index()
        {
            _logger.LogInformation("Page login Admin");
            return RedirectToAction("Index", "Account", new { area = "Admin" });
        }

        #region Login

        [Route("Admin/tai-khoan/dang-nhap")]
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Route("Admin/tai-khoan/dang-nhap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogDebug("Wait Login Admin");
            if (ModelState.IsValid)
            {
                var user = _signInManager.UserManager.Users.Where(u => u.Email == model.UserName || u.UserName == model.UserName).FirstOrDefault();
                if (user == null)
                {
                    _logger.LogWarning("User {0} not found", model.UserName);
                    ModelState.AddModelError(string.Empty, string.Format("Tài khoản {0} không tồn tại", model.UserName));
                    return View(model);
                }
                else
                {
                    if (!user.EmailConfirmed)
                    {
                        _logger.LogWarning("User {0} not confirmed account", model.UserName);
                        ModelState.AddModelError(string.Empty, string.Format("Tài khoản {0} chưa xác thực email", model.UserName));
                        return View(model);
                    }

                    if (user.IsLocked)
                    {
                        if (user.OpenTime > DateTime.Now)
                        {
                            var waitTime = (user.OpenTime - DateTime.Now).Value.Minutes + 1;
                            _logger.LogWarning("User {0} blocked to {1}", model.UserName, waitTime);
                            ModelState.AddModelError(string.Empty, string.Format("Tài khoản {0} đang bị khóa. Vui lòng quay lại sau: {1} phút", model.UserName, waitTime));
                            return View(model);
                        }
                        if (user.OpenTime < DateTime.Now)
                        {
                            user.OpenTime = null;
                            user.IsLocked = false;
                            _dbContext.SaveChanges();
                        }
                    }
                }
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.Now;
                    user.LastLoginIP = Common.GET_IP();
                    _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, AppUser = user, Action = UserAction.Login, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login success", model.UserName) });
                    _dbContext.SaveChanges();
                    _logger.LogDebug("User {0} loggin successful.", model.UserName);
                    return RedirectToLocal(model.ReturnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User {0} lock 5 minitus.", model.UserName);
                    var userId = _dbContext.Users.FirstOrDefault(m => m.Email.Equals(model.UserName) || m.UserName.Equals(model.UserName)).Id;
                    user.IsLocked = true;
                    user.OpenTime = DateTime.Now.AddMinutes(5);
                    _dbContext.AccountLogs.Add(new AccountLog() { UserId = userId, Action = UserAction.LockAccount, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login when locked", model.UserName) });
                    _dbContext.SaveChanges();
                    ModelState.AddModelError(string.Empty, string.Format("Tài khoản {0} bị tạm khóa. Vui lòng thử lại sau", model.UserName));
                }
                else
                {
                    _logger.LogWarning("User {0} login incorrect password.", model.UserName);
                    var userId = _dbContext.Users.FirstOrDefault(m => m.Email.Equals(model.UserName) || m.UserName.Equals(model.UserName)).Id;
                    _dbContext.AccountLogs.Add(new AccountLog() { UserId = userId, Action = UserAction.Login, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login failed wrong password", model.UserName) });
                    _dbContext.SaveChanges();
                    ModelState.AddModelError(string.Empty, "Mật khẩu không đúng");
                }
            }
            else
            {
                _logger.LogWarning("User {0} login failed.", model.UserName);
                var user = _dbContext.Users.FirstOrDefault(m => m.Email.Equals(model.UserName) || m.UserName.Equals(model.UserName));
                if (user == null)
                {
                    _logger.LogWarning("User {0} not found", model.UserName);
                    ModelState.AddModelError(string.Empty, string.Format("Tài khoản {0} không tồn tại", model.UserName));
                    return View(model);
                }
                var userId = user.Id;
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = userId, Action = UserAction.Login, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login failed underfined", model.UserName) });
                _dbContext.SaveChanges();
                ModelState.AddModelError(string.Empty, "Đăng nhập thất bại");
            }
            return View(model);
        }
        #endregion

        #region Register
        [Route("Admin/tai-khoan/dang-ky")]
        [HttpGet]
        public ActionResult Register()
        {
            _logger.LogInformation("Page Register Admin");
            return View();
        }

        [Route("Admin/tai-khoan/dang-ky")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogDebug("Register Admin");
            var checkUser = _signInManager.UserManager.Users.Where(u => u.Email == model.Email).FirstOrDefault();
            if (checkUser != null)
            {
                _logger.LogWarning("Email {0} duplicate", model.Email);
                ModelState.AddModelError(string.Empty, string.Format("Email {0} đã có người sử dụng", model.Email));
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = Common.ConvertEmailToName(model.Email), Email = model.Email, LastLoginIP = Common.GET_IP(), CreateTime = DateTime.Now };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User {0} was created.", user.UserName);
                    string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await MessageServices.SendEmailAsync(model.Email, "Aromatide | Xác nhận tài khoản",
                                                         "<div style = \"width: 25%; margin-bottom: 1rem; " +
                                                         "border: 3px solid #007bff; word-wrap: break-word; " +
                                                         "border-radius: 1rem; display: block; background-color: azure;\" >" +
                                                         "<div style = \"padding: 1.25rem;\" >" +
                                                         "<div style = \"text-align: center; height: 50px; display: inline-flex;\" >" +
                                                         "<img style = \"border: 3px solid #adb5bd; margin: 0 auto; " +
                                                         "border-radius: 50%; \" src=\"https://i.pinimg.com/originals/94/0f/fc/940ffc44dd92dabf6f4a8826e6d0b826.jpg\" alt=\"logo\">" +
                                                         "<h3 style = \"padding-left: 1rem;\" > Xin chào " + 
                                                         Common.ConvertEmailToName(model.Email)  + "</h3 ></div >" +
                                                         "<ul style = \"list-style-type: none; padding-left: 0;\" >" +
                                                         " <li><b>Email:</b ><span> " + model.Email + " </span></li>" +
                                                         "<hr>" +
                                                         "<li><b> Nội dung:</b > <span> Nhấn xác nhận để kích hoạt tài khoản </span></li>" +
                                                         "</ul>" +
                                                         "<div style = \"cursor: pointer;display: block; padding: 0.375rem 0.75rem; " +
                                                         "font-size: 1rem; line-height: 1.5; border-radius: 0.25rem; background-color: #007bff;" +
                                                         "border-color: #007bff; box-shadow: none; text-align: center; \">" +
                                                         "<a style=\"color: #fff; text-decoration: none;\" href=\"" + callbackUrl + "\"><b>Xác nhận</b></a></div>" +
                                                         "</div></div> ");
                    _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, Action = UserAction.Register, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} register success", user.UserName) });
                    _dbContext.SaveChanges();
                    return RedirectToAction("RegisterConfirmation", "Account", new { link = callbackUrl });
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Route("Admin/tai-khoan/xac-nhan-dang-ky/{id?}")]
        public ActionResult RegisterConfirmation(string link)
        {
            return View();
        }

        [Route("Admin/tai-khoan/xac-nhan-email/{id?}")]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogError("register comfirm error not found user or code");
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("register comfirm error not found user");
                return View("Error");
            }
            user.Status = EntityStatus.Enabled;
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                _logger.LogInformation("register comfirm success");
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, Action = UserAction.VerifyEmail, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} register confirm success", user.UserName) });
                _dbContext.SaveChanges();
                return View("ConfirmEmail");
            }
            else
            {
                _logger.LogError("register comfirm error");
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, Action = UserAction.VerifyEmail, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} register confirm failed", user.UserName) });
                _dbContext.SaveChanges();
                return View("Error");
            }
        }
        #endregion

        #region ForgotPassword
        [Route("Admin/tai-khoan/quen-mat-khau")]
        public ActionResult ForgotPassword()
        {
            _logger.LogInformation("Page forgotpassword");
            return View();
        }

        [Route("Admin/tai-khoan/quen-mat-khau/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogWarning("Email {0} not found", model.Email);
                    ModelState.AddModelError(string.Empty, string.Format("Email {0} không tồn tại", model.Email));
                    return View(model);
                }
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await MessageServices.SendEmailAsync(model.Email, "Aromatide | Đổi mật khẩu",
                                                         "<div style = \"width: 25%; margin-bottom: 1rem; " +
                                                         "border: 3px solid #007bff; word-wrap: break-word; " +
                                                         "border-radius: 1rem; display: block; background-color: azure;\" >" +
                                                         "<div style = \"padding: 1.25rem;\" >" +
                                                         "<div style = \"text-align: center; height: 50px; display: inline-flex;\" >" +
                                                         "<img style = \"border: 3px solid #adb5bd; margin: 0 auto; " +
                                                         "border-radius: 50%; \" src=\"https://i.pinimg.com/originals/94/0f/fc/940ffc44dd92dabf6f4a8826e6d0b826.jpg\" alt=\"logo\">" +
                                                         "<h3 style = \"padding-left: 1rem;\" > Xin chào " +
                                                         Common.ConvertEmailToName(model.Email) + "</h3 ></div >" +
                                                         "<ul style = \"list-style-type: none; padding-left: 0;\" >" +
                                                         " <li><b>Email:</b ><span> "+ model.Email + " </span></li>" +
                                                         "<hr>" +
                                                         "<li><b> Nội dung:</b > <span> Nhấn xác nhận để đổi mật khẩu mới </span></li>" +
                                                         "</ul>" +
                                                         "<div style = \"cursor: pointer;display: block; padding: 0.375rem 0.75rem; " +
                                                         "font-size: 1rem; line-height: 1.5; border-radius: 0.25rem; background-color: #007bff;" +
                                                         "border-color: #007bff; box-shadow: none; text-align: center; \">" +
                                                         "<a style=\"color: #fff; text-decoration: none;\" href=\"" + callbackUrl + "\"><b>Xác nhận</b></a></div>" +
                                                         "</div></div> ");
                _logger.LogInformation("Email {0} forgot password success", model.Email);
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, AppUser = user, Action = UserAction.ForgotPassword, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("Email {0} forgot password", user.Email) });
                _dbContext.SaveChanges();
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [Route("Admin/tai-khoan/xac-nhan-quen-mat-khau/{id?}")]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();////
        }

        [Route("Admin/tai-khoan/doi-mat-khau/{id?}")]
        public async Task<ActionResult> ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogError("reset password error not found user or code");
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("reset password error not found user");
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, AppUser = user, Action = UserAction.ResetPassByEmail, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} not found", user.UserName) });
                _dbContext.SaveChanges();
                return View("Error");
            }
            var resetPasswordViewModel = new ResetPasswordViewModel() { UserId = userId, Code = code };

            _logger.LogInformation("reset comfirm");
            _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, AppUser = user, Action = UserAction.ResetPassByEmail, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} reset password", user.UserName) });
            _dbContext.SaveChanges();
            return code == null ? View("Error") : View(resetPasswordViewModel);
        }

        [Route("Admin/tai-khoan/doi-mat-khau/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("reset passowrd error");
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogError("reset passowrd error not found user");
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("reset passowrd success");
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            _logger.LogError("reset passowrd error");
            return View();
        }
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}
        #endregion

        #region LoginEx

        [Route("Admin/tai-khoan/lien-ket-dang-nhap/{id?}")]
        public IActionResult ExternalLogin(LoginExViewModel model)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { model.ReturnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl);
            return Challenge(properties, model.Provider);
        }

        [Route("Admin/tai-khoan/lien-ket-dang-nhap/{id?}")]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                _logger.LogWarning("Login Admin external fail remote");
                ModelState.AddModelError(string.Empty, $"Đăng nhập Facebook/Google lỗi: {remoteError}");
                return RedirectToAction(nameof(Login));
            }
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                _logger.LogWarning("User login ex not found");
                ModelState.AddModelError(string.Empty, "Không tìm thấy tài khoản đã đăng ký.");
                return RedirectToAction("Login");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User login ex success.");
                var userId = (from x in _dbContext.UserLogins
                              where x.ProviderKey == loginInfo.ProviderKey
                              select x.UserId).SingleOrDefault();
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = userId, Action = UserAction.ExternalAddLogin, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login external", loginInfo.ProviderDisplayName.ToString()) });
                _dbContext.SaveChanges();
                return (ActionResult)RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User locked 5 minitus.");
                ModelState.AddModelError(string.Empty, "Tài khoản đang bị tạm khóa");
                var user = _dbContext.Users.Find(loginInfo.ProviderDisplayName);
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = user.Id, Action = UserAction.LockAccount, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login external locked", loginInfo.ProviderDisplayName.ToString()) });
                _dbContext.SaveChanges();
                ModelState.AddModelError(string.Empty, "Tài khoản bị tạm khóa. Vui lòng thử lại sau.");
            }
            else
            {
                var appUser = await CreateUserEx(loginInfo);
                if (appUser == null)
                {
                    _logger.LogWarning("not create user ex");
                    ModelState.AddModelError(string.Empty, "Lỗi: Không thể tạo Tài khoản liên kết");
                    return RedirectToAction(nameof(Login));
                }

                await _signInManager.SignInAsync(appUser, true);
                _dbContext.AccountLogs.Add(new AccountLog() { UserId = appUser.Id, Action = UserAction.ExternalAddLogin, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} create account external success and login", appUser.UserName) });
                _dbContext.SaveChanges();
                return (ActionResult)RedirectToLocal(returnUrl);
            }
            return View();
        }

        [Route("Admin/tai-khoan/xac-nhan-lien-ket-dang-nhap/{id?}")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            _logger.LogDebug("Login Admin external");
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Admin" });
            }

            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogWarning("Login Admin external fail");
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser { UserName = Common.ConvertEmailToName(model.Email), Email = model.Email };
                var result = await _userManager.CreateAsync(user);

                var manageClaim = info.Principal.Claims.Where(c => c.Type == "ManageStore").FirstOrDefault();
                if (manageClaim != null)
                {
                    await _userManager.AddClaimAsync(user, manageClaim);
                }

                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogDebug("Login Admin external success");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return (ActionResult)RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        #endregion

        [Route("Admin/tai-khoan/khoa-man-hinh/{id?}")]
        [HttpGet]
        public async Task<IActionResult> LockScreen(string name, string returnUrl)
        {
            _logger.LogInformation("Page lock screen Admin");
            if (name == null || returnUrl == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var lvm = new LoginViewModel()
            {
                UserName = name,
                Password = "",
                RememberMe = false,
                ReturnUrl = returnUrl
            };

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            _logger.LogInformation("User lockscreen successful.");
            return View(lvm);
        }

        [Route("Admin/tai-khoan/thoat")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Logout successful.");
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Route("Admin/tai-khoan/tu-choi-dang-nhap")]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogError("User login access denied.");
            return View();
        }

        #region Helper
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction(nameof(HomeController.Index), "Home", new { area = "Admin" });

            return Redirect(returnUrl);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                _logger.LogError("Error: {error}", error.Description);
            }
        }
        private async Task<string> GetUserName(ExternalLoginInfo loginInfo)
        {
            _logger.LogDebug("Get username");
            string defaultName = null;
            if (loginInfo.LoginProvider == "Facebook" || loginInfo.LoginProvider == "Google")
            {
                var nameClaim = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                    defaultName = Common.NormalizeVietnamese(nameClaim.Value);
            }

            if (defaultName == null)
                return null;

            string newUserName = defaultName;
            for (int i = 0; i < 30; i++)
            {
                AppUser newUser = await _userManager.FindByNameAsync(newUserName);
                if (newUser == null)
                    break;

                int randNo = Common.Random(99) + 1;
                newUserName = string.Format("{0}{1:D2}", defaultName, randNo);
            }

            return newUserName;
        }
        private async Task<AppUser> CreateUserEx(ExternalLoginInfo loginInfo)
        {
            _logger.LogInformation("Create userEx");
            string newUserName = await GetUserName(loginInfo);
            if (newUserName == null)
                return null;

            var exEmail = loginInfo.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            AppUser appUser = new AppUser
            {
                UserName = newUserName,
                Email = exEmail?.Value,
                EmailConfirmed = (exEmail != null),
                CreateTime = DateTime.Now,
                LastUpdate = DateTime.Now,
                Status = EntityStatus.Enabled
            };

            var result = await _userManager.CreateAsync(appUser);
            if (!result.Succeeded)
            {
                return null;
            }

            result = await _userManager.AddLoginAsync(appUser, loginInfo);
            if (!result.Succeeded)
            {
                return null;
            }

            return appUser;
        }
        #endregion
    }
}
