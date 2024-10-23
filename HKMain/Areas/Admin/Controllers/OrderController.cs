using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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
                Date = x.CreateTime,
                NameUser = (x.AppUser.UserName != null) ? x.AppUser.UserName : x.GuestName,
                EmailUser = (x.AppUser.Email != null) ? x.AppUser.Email : x.GuestEmail,
                PaymentStatus = x.PaymentStatus,
                Status = x.OrderStatus,
                Method = x.PaymentInfo
            });
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model, Common.FormatSettingsJsonConvert()) });
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
            return View();
        }

        // GET: Product/Details/5
        [Route("Admin/don-hang/chinh-sua/{id?}")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if(id == 0) return View(new OrderEditViewModel());
            else
            {
                var model = new OrderEditViewModel();
                try
                {
                    var order = _dbContext.Orders.FirstOrDefault(x => x.Id == id);
                    model.Id = id;
                    model.NameUser = order.GuestName;
                    model.EmailUser = order.GuestEmail;
                    model.PhoneUser =  order.GuestPhone;
                    model.Date = order.CreateTime;
                    model.Note = order.Note;
                    model.Price = order.AdjustPrice;
                    model.Fee = order.ShippingFee;
                    model.Address = order.Address;
                    model.Total = order.GrandTotalPrice;
                    model.PaymentStatus = order.PaymentStatus;
                    model.Status = order.OrderStatus;
                }
                catch (Exception ex)
                {

                    throw;
                }
                return View(model);
            }
        }

        [Route("Admin/don-hang/chinh-sua/{id?}")]
        [HttpPost]
        public ActionResult Edit(OrderEditViewModel model)
        {
            var oldOrder = _dbContext.Orders.FirstOrDefault(x => x.Id == model.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    var newOrder = new HKShared.Data.Order()
                    { 
                        Id = model.Id,
                        GuestName = model.NameUser,
                        GuestEmail = model.EmailUser,
                        GuestPhone = model.PhoneUser,
                        Address = model.Address,
                        Note = model.Note,
                        AdjustPrice = model.Price,
                        ShippingFee = model.Fee,
                        GrandTotalPrice = model.Total,
                        PaymentInfo = "COD",
                        PaymentStatus = model.PaymentStatus,
                        OrderStatus = model.Status,
                        CreateTime = oldOrder.CreateTime
                    };
                    if(newOrder.OrderStatus >= OrderStatus.Processing)
                    {
                        newOrder.ShippingTime = DateTime.Now;
                    }
                    if (oldOrder != null)
                    {
                        _dbContext.Entry(oldOrder).CurrentValues.SetValues(newOrder);
                    }
                    else
                    {
                        _dbContext.Orders.Add(newOrder);
                    }
                    _dbContext.SaveChanges();
                    var newModel = new OrderEditViewModel();
                    newModel.Id = newOrder.Id;
                    newModel.NameUser = newOrder.GuestName;
                    newModel.EmailUser = newOrder.GuestEmail;
                    newModel.PhoneUser = newOrder.GuestPhone;
                    newModel.Date = newOrder.CreateTime;
                    newModel.Note = newOrder.Note;
                    newModel.Price = newOrder.AdjustPrice;
                    newModel.Fee = newOrder.ShippingFee;
                    newModel.Address = newOrder.Address;
                    newModel.Total = newOrder.GrandTotalPrice;
                    newModel.PaymentStatus = newOrder.PaymentStatus;
                    newModel.Status = newOrder.OrderStatus;
                    return View(newModel);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return View(model);
        }
        [Route("Admin/don-hang/xoa/{id?}")]
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
        [Route("Admin/don-hang/xoa/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var model = _dbContext.Orders.Find(id);
                _dbContext.Orders.Remove(model);
                _dbContext.SaveChanges();
                var modelItems = _dbContext.OrderItems.Where(x => x.OrderId == id).ToList();
                _dbContext.OrderItems.RemoveRange(modelItems);
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
                Material = x.ItemAttrib,
                Price = x.Product.FormatedCurrentPrice,
                Quantity = x.Quantity,
                Total = (x.Product.CurrentPrice * x.Quantity).ToString("#,#đ"),
            }).ToList();

            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }
        public JsonResult LoadProductSelect(int id)
        {
            _logger.LogInformation("Load list order item");
            var product = _dbContext.Products.Where(x => x.Status == ProductStatus.Published).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = false }).ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(product) });
        }
        public JsonResult LoadVariantSelect(int id)
        {
            _logger.LogInformation("Load list order item");
            var product = _dbContext.ProductAttribs.Where(x => x.AttrId == id);
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(product) });
        }
        [Route("Admin/Order/OrderItemEdit/{id?}")]
        public ActionResult OrderItemEdit(int id)
        {
            try
            {
                if (id == 0)
                {
                    ViewBag.ListProduct = _dbContext.Products.Where(x => x.Status == ProductStatus.Published).Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = false }).ToList();
                    return PartialView("OrderItemEdit", new OrderItem());
                }
                else
                {

                }
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
