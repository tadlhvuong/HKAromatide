using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using HKMain.Areas.Admin.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, Manager")]
    public class MemberController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private IHttpContextAccessor _accessor;

        public MemberController(
            ILogger<MemberController> logger,
            AppDBContext dbContext,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IHttpContextAccessor accessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _accessor = accessor;
        }

        #region Member

        [Route("Admin/Member/LoadMemberList")]
        public JsonResult LoadMemberList()
        {
            _logger.LogInformation("Load list member");

            var usersWithRoles = (
                      from u in _dbContext.Users
                      select new UserViewModel()
                      {
                          Id = u.Id,
                          AvatarImg = u.AvatarImg,
                          Email = u.Email,
                          UserName = u.UserName,
                          EmailConfirmed = u.EmailConfirmed,
                          PhoneNumber = u.PhoneNumber,
                          Status = u.Status,
                          Roles = (
                          from ur in _dbContext.UserRoles
                          join r in _dbContext.Roles on ur.RoleId equals r.Id
                          where ur.UserId == u.Id
                          select r.Name).ToArray()
                      });
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(usersWithRoles) });
        }
        [Route("Admin/thanh-vien")]
        public IActionResult Index()
        {
            _logger.LogInformation("Page member");
            return View();
        }

        [Route("Admin/thanh-vien/chi-tiet/{id?}")]
        public IActionResult Details(string id)
        {
            _logger.LogInformation("Page details member");

            var usersWithRoles = (
                      from u in _dbContext.Users
                      where u.Id == id
                      select new UserViewModel()
                      {
                          Id = u.Id,
                          AvatarImg = u.AvatarImg,
                          Email = u.Email,
                          UserName = u.UserName,
                          EmailConfirmed = u.EmailConfirmed,
                          PhoneNumber = u.PhoneNumber,
                          Status = u.Status,
                          Roles = (
                          from ur in _dbContext.UserRoles
                          join r in _dbContext.Roles on ur.RoleId equals r.Id
                          where ur.UserId == u.Id
                          select r.Name).ToArray()
                      }).FirstOrDefault();

            if (usersWithRoles == null)
                return NotFound();

            return View(usersWithRoles);
        }
        [Route("Admin/thanh-vien/chinh-sua/{id?}")]
        public IActionResult Edit(string id)
        {
            _logger.LogInformation("Page update Member");

            AppUser appUser = _dbContext.Users.Find(id);
            if (appUser == null)
            {
                _logger.LogWarning("Page Member fail");
                return NotFound();
            }
            EditUserModel model = new EditUserModel()
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Password = appUser.PasswordHash,
                Email = appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                Status = appUser.Status,
            };

            //var model = (from u in _dbContext.Users
            //             join ur in _dbContext.UserRoles on u.Id equals ur.UserId into tb1
            //             from t1 in tb1.DefaultIfEmpty()
            //             join r in _dbContext.Roles on t1.RoleId equals r.Id into tb2
            //             from t2 in tb2.DefaultIfEmpty()
            //             select new UserViewModel()
            //             {
            //                 Id = u.Id,
            //                 AvatarImg = u.AvatarImg,
            //                 Email = u.Email,
            //                 UserName = u.UserName,
            //                 EmailConfirmed = u.EmailConfirmed,
            //                 PhoneNumber = u.PhoneNumber,
            //                 Status = u.Status,
            //                 RoleName = t2.Name,
            //             }).ToList().Where(s => s.Id == id).FirstOrDefault();

            model.modelResult = new ModalFormResult() { Code = -1 };
            return View(model);
        }
        [Authorize(Roles = "Administrator")]
        [Route("Admin/thanh-vien/chinh-sua/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AppUser appUser = _dbContext.Users.Find(model.UserId);
                    if (appUser == null)
                        return NotFound();

                    appUser.UserName = model.UserName;
                    appUser.Email = model.Email;
                    appUser.EmailConfirmed = !string.IsNullOrEmpty(model.Email);

                    appUser.PhoneNumber = model.PhoneNumber;
                    appUser.PhoneNumberConfirmed = !string.IsNullOrEmpty(model.PhoneNumber);

                    appUser.LastUpdate = DateTime.Now;
                    appUser.UpdateUser = User.Identity.Name;

                    appUser.Status = model.Status;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        await _userManager.RemovePasswordAsync(appUser);
                        await _userManager.AddPasswordAsync(appUser, model.Password);
                    }
                    _dbContext.AccountLogs.Add(new AccountLog() { UserId = appUser.Id, AppUser = appUser, Action = UserAction.UpdateDate, LogTime = DateTime.Now, RemoteIP = Common.GET_IP(), LogData = string.Format("User {0} login success", model.UserName) });
                    _dbContext.SaveChanges();
                    model.modelResult = new ModalFormResult() { Code = 1, Message = "Cập nhật thành công" };
                    _logger.LogDebug("update Member success");
                    return View(model);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("update Member fail");
                    model.modelResult = new ModalFormResult() { Code = 0, Message = "Cập nhật không thành công" };
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            model.modelResult = new ModalFormResult() { Code = 0, Message = "Cập nhật không thành công" };
            return View(model);
        }

        [Route("Admin/thanh-vien/tim-kiem/{id?}")]
        public IActionResult Search()
        {
            _logger.LogInformation("Page search member");
            MemberSearchModel model = new MemberSearchModel() { FindMode = 1 };
            return View(model);
        }
        [Route("Admin/thanh-vien/tim-kiem/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Search(MemberSearchModel model)
        {
            if (ModelState.IsValid)
            {
                bool doSearch = false;
                IQueryable<AppUser> usersQuery = _dbContext.Users;

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.UserName.Equals(model.UserName));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.UserName.StartsWith(model.UserName));
                    else
                        usersQuery = usersQuery.Where(u => u.UserName.Contains(model.UserName));
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.Email.Equals(model.Email));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.Email.StartsWith(model.Email));
                    else
                        usersQuery = usersQuery.Where(u => u.Email.Contains(model.Email));
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    doSearch = true;
                    if (model.FindMode == 1)
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.Equals(model.PhoneNumber));
                    else if (model.FindMode == 2)
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.StartsWith(model.PhoneNumber));
                    else
                        usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(model.PhoneNumber));
                }

                model.Results = doSearch ? usersQuery.OrderBy(u => u.UserName).Take(200).ToList() : null;
            }
            _logger.LogDebug("Search member success");
            return View(model);
        }


        [Route("Admin/Member/ResetPassword/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("reset passowrd error");
                //return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogError("reset passowrd error not found user");
                //return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            model.Code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("reset passowrd success");
                return Json(new ModalFormResult() { Code = 1, Message = "Change password successed!", data = JsonConvert.SerializeObject(model) });
            }
            _logger.LogError("reset passowrd error");
            return Json(new ModalFormResult() { Code = 0, Message = "Change password failed!", data = JsonConvert.SerializeObject(model) });
        }

        [Route("Admin/Member/SuspendAccount/{id?}")]
        public IActionResult SuspendAccount(string id)
        {
            _logger.LogInformation("Suspend account");
            if(_dbContext.Users.ToList().Count == 0)
            {
                _logger.LogWarning("Suspend account fail");
                return BadRequest();
            }
            var model = _dbContext.Users.Where(x => x.Id.Equals(id)).Select(s => new CommonModel()
            {
                Key = s.Id,
                Value = s.UserName
            }).FirstOrDefault();
            if (model == null)
            {
                _logger.LogWarning("Suspend account fail");
                return BadRequest();
            }
            return PartialView("_PartialSuspend", model);
        }
        [Route("Admin/Member/SuspendAccount/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuspendAccount(string id, IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("reset passowrd error");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = EntityStatus.Disabled;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                _logger.LogError("reset passowrd error not found user");

                return Json(new ModalFormResult() { Code = 1, Message = "Suspend successed!", data = JsonConvert.SerializeObject(user) });
            }
            return Json(new ModalFormResult() { Code = 0, Message = "Suspend failed!", data = JsonConvert.SerializeObject(user) });

        }


        [Route("Admin/Member/UnSuspendAccount/{id?}")]
        public IActionResult UnSuspendAccount(string id)
        {
            var model = _dbContext.Users.Where(x => x.Id.Equals(id)).Select(s => new CommonModel()
            {
                Key = s.Id,
                Value = s.UserName
            }).FirstOrDefault();
            if (model == null)
            {
                _logger.LogWarning("Page delete album fail");
                return BadRequest();
            }

            _logger.LogInformation("Page delete album");
            return View("_PartialSuspend", model);
        }
        [Route("Admin/Member/UnSuspendAccount/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UnSuspendAccount(string id, IFormCollection collection)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("reset passowrd error");
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Status = EntityStatus.Enabled;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
                _logger.LogError("reset passowrd error not found user");

                return Json(new ModalFormResult() { Code = 1, Message = "Suspend successed!", data = JsonConvert.SerializeObject(user) });
            }
            return Json(new ModalFormResult() { Code = 0, Message = "Suspend failed!", data = JsonConvert.SerializeObject(user) });

        }

        #endregion

        #region Helper
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
                _logger.LogError("Error in creating user: {error}", error.Description);
            }
        }
        #endregion
    }
}
