using DocumentFormat.OpenXml.Office2010.Excel;
using HKMain.Areas.Admin.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, Manager")]
    public class OrderController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;

        public OrderController(
            ILogger<ProductController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        // GET: Load data Product
        [Route("Admin/Order/LoadOrderList")]
        public JsonResult LoadOrderList()
        {
            _logger.LogInformation("Load list order");
            var model = _dbContext.Orders.Select(x => new OrderViewModel()
            {
                Id = x.Id,
                UserId = x.UserId,
                Date = x.ShippingTime,
                NameUser = x.AppUser.UserName,
                EmailUser = x.AppUser.Email,
                PaymentStatus = x.PaymentStatus,
                Status  = x.OrderStatus,
                Method = x.PaymentInfo
            });

            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }

        // GET: Product
        [Route("Admin/don-hang")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Admin/don-hang/chi-tiet/{id?}")]
        public ActionResult Details(int id)
        {
            var model = _dbContext.Orders.Where(x => x.Id == id).ToList().FirstOrDefault();
            return View();
        }
        // GET: Product/Details/5
        [Route("Admin/don-hang/tao-hoa-don/{id?}")]
        public ActionResult Create()
        {
            return View(new Order());
        }

        // GET: Product/Details/5
        [Route("Admin/don-hang/chinh-sua/{id?}")]
        public ActionResult Edit(int id)
        {
            return View(new OrderEditViewModel());
        }
        [Route("Admin/Order/Delete/{id?}")]
        public ActionResult Delete(int id)
        {
            _logger.LogInformation("Delete product:" + id);
            var model = _dbContext.Orders.Where(s => s.Id == id).Select(x => new CommonModelRemove()
            {
                Id = x.Id,
                Name = "Hóa đơn: " + x.Id.ToString(),
            }).FirstOrDefault();
            return View("_AdminModalDelete", model);
        }

        // POST: Product/Delete/5
        [Route("Admin/Order/Delete/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var model = _dbContext.Orders.Find(id);
                _dbContext.Orders.Remove(model);
                _dbContext.SaveChanges();

                return Json(new ModalFormResult() { Code = 1, Message = "remove product success!", data = JsonConvert.SerializeObject(model) });
            }
            catch
            {
                return View();
            }
        }

        #region OrderItem
        [Route("Admin/Order/LoadOrderItemList/{id?}")]
        public JsonResult LoadOrderItemList(int id)
        {
            _logger.LogInformation("Load list order item");
            if (id == 0)
                return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = null });
            var model = _dbContext.OrderItems.Where(x => x.OrderId == id).Select( x=> new OrderItemViewModel()
            {
                Id=x.Id,
                ImageItem = x.Product.Image,
                NameItem = x.Product.Name,
                Material = x.Product.Tags,
                Price = x.Product.Price,
                Quantity = x.Quantity,
                Total = x.Product.Price*x.Quantity,
            }).ToList();

            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }
        [Route("Admin/Order/OrderItemEdit/{id?}")]
        public ActionResult OrderItemEdit(int id)
        {
            try
            {
                //if (id == null || id == 0)
                //{
                //    ViewBag.ListUser = _dbContext.Users.Where(x => x.Status == EntityStatus.Enabled).Select(x => new SelectListItem() { Value = x.Id, Text = x.UserName, Selected = false }).ToList();
                //    return PartialView("OrderItemEdit", new ShippingEditModel());
                //}
                //var shipping = _dbContext.Shippings.Find(id);
                //var model = new ShippingEditModel()
                //{
                //    Id = id,
                //    UserId = shipping.UserId,
                //    Address = shipping.Address,
                //    ProvinceId = shipping.ProvinceId,
                //    DistrictId = shipping.DistrictId,
                //    WardId = shipping.WardId,
                //    ZipCode = shipping.ZipCode
                //};

                //var listUser = _dbContext.Users.Where(x => x.Status == EntityStatus.Enabled).Select(x => new SelectListItem() { Value = x.Id, Text = x.UserName, Selected = false }).ToList();
                //if (model.UserId != null)
                //    foreach (var item in listUser)
                //    {
                //        if (item.Value.Equals(model.UserId))
                //            item.Selected = true;
                //    }
                //ViewBag.ListUser = listUser;

                return PartialView("OrderItemEdit", null);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                throw;
            }
        }
        [Route("Admin/Order/OrderItemEdit/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OrderItemEdit(ShippingEditModel model)
        {
            try
            {
                //var oldShipping = _dbContext.Shippings.FirstOrDefault(x => x.Id == model.Id);

                //var shipping = new Shipping()
                //{
                //    Id = model.Id,
                //    UserId = model.UserId,
                //    Address = model.Address,
                //    DistrictId = model.DistrictId,
                //    ProvinceId = model.ProvinceId,
                //    WardId = model.WardId,
                //    ZipCode = model.ZipCode
                //};
                //if (oldShipping != null)
                //{
                //    _dbContext.Entry(oldShipping).CurrentValues.SetValues(shipping);
                //}
                //else
                //{
                //    _dbContext.Shippings.Add(shipping);
                //}
                //_dbContext.SaveChanges();
                //var user = _dbContext.Users.Find(model.UserId);
                //var ward = _dbContext.LocationWards.FirstOrDefault(x => x.Code == shipping.WardId).FullName;
                //var district = _dbContext.LocationDistricts.FirstOrDefault(x => x.Code == shipping.DistrictId).FullName;
                //var province = _dbContext.LocationProvinces.FirstOrDefault(x => x.Code == shipping.ProvinceId).FullName;
                //var addressFull = model.Address + ',' + ward + ',' + district + ',' + province;
                //var convertDataView = new ShippingViewModel()
                //{
                //    Id = model.Id,
                //    Name = user.UserName,
                //    Address = addressFull,
                //    Phone = user.PhoneNumber,
                //    Status = user.Status,
                //    ZipCode = shipping.ZipCode,
                //};
                _logger.LogDebug("Edit Category success");
                return Json(new ModalFormResult() { Code = 1, Message = "Cập nhật thành công", data = null });
            }
            catch
            {
                _logger.LogDebug("Edit Category failed");
                return Json(new ModalFormResult() { Code = 0, Message = "Cập nhật thất bại" });
            }
        }
        #endregion

    }
}
