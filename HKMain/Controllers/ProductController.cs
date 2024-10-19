using DocumentFormat.OpenXml.InkML;
using HKMain.Areas.Admin.Models;
using HKMain.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HKMain.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;
        private readonly string mediaUrl;
        private readonly string mediaPath;

        public ProductController(
            ILogger<ProductController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            mediaUrl = AppSettings.Strings["MediaUrl"] ?? "/media";
            mediaPath = AppSettings.Strings["MediaPath"] ?? "./wwwroot/media";
        }
        [Route("san-pham")]
        public IActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

        [Route("san-pham/chi-tiet-san-pham/")]
        public IActionResult Details(int id)
        {
            var model = _dbContext.Products.Include(x => x.MediaAlbum).Include(x => x.MediaAlbum.MediaFiles).Single(x=> x.Id == id);
            if (model == null) { return NotFound(); }
            model.Image = (model.MediaAlbum.MediaFiles.FirstOrDefault() != null) ? model.MediaAlbum.MediaFiles.FirstOrDefault().FullPath : "~/images/image-default.png";
            if (model.FilesId != null)
            {
                model.MediaFiles = new List<MediaFile>();
                string[] idFile = model.FilesId.Split(',');
                foreach (var file in idFile)
                {
                    var mf = _dbContext.MediaFiles.Find(int.Parse(file));
                    mf.FileLink = Path.Combine(mediaUrl, mf.FullPath);
                    model.MediaFiles.Add(mf);
                }
            }

            //var FullPath = Path.Combine(mediaUrl, model.Image);
            //model.LinkImage = FullPath;
            var variants = _dbContext.ProductAttribs.Where(x => x.ItemId == id).ToList();
            var variantsKey = variants.GroupBy(
            p => p.AttrId,
            p => p.AttrChildId,
            (key, a) => new { Id = key, Attrs = a.ToList() });

            var variantsResult = new List<ShopVariantDetails>(); 
            foreach (var item in variantsKey)
            {
                var v = new ShopVariantDetails();
                var db = _dbContext.Taxonomies.Find(item.Id);
                v.Id = db.Id;
                v.Name = db.Name;
                v.VariantChild = new List<ShopVariantDetails>();
                foreach (var itemChild in item.Attrs)
                {
                    var vc = new ShopVariantDetails();
                    var dbc = _dbContext.Taxonomies.Find(itemChild);
                    vc.Id = dbc.Id;
                    vc.Name = dbc.Name;
                    v.VariantChild.Add(vc);
                }
                variantsResult.Add(v);
            }
            ViewBag.Variants = variantsResult;

            return View(model);
        }

        public JsonResult LoadProduct(int id)
        {
            var model = _dbContext.Products.Find(id);
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }

        public JsonResult AddToCart(int id, int quantity, int[] variants)
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");

            // Set default quantity 
            if (quantity == 0)
                quantity = 1;

            if (cart == null)
            {
                cart = new Cart();
                var cartItem = new CartItem();
                var shopItem = _dbContext.Products.Find(id);

                cartItem.ShopItem = shopItem;
                cartItem.Quantity = quantity;
                cartItem.Cart = cart;

                cart.Items = new List<CartItem>();
                cart.Items.Add(cartItem);

                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            else
            {
                int existIndex = -1;
                for (int i = 0; i < cart.Items.Count; i++)
                {
                    if (cart.Items.ElementAt(i).ShopItem.Id == id)
                    {
                        existIndex = i;
                        break;
                    }
                }
                if (existIndex == -1)
                {
                    var cartItem = new CartItem();
                    var shopItem = _dbContext.Products.Find(id);
                    cartItem.ShopItem = shopItem;
                    cartItem.Quantity = quantity;
                    cartItem.Cart = cart;
                    cart.Items.Add(cartItem);
                }
                else
                {
                    cart.Items.ElementAt(existIndex).Quantity = cart.Items.ElementAt(existIndex).Quantity + quantity;
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);

            }
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(cart) });
        }

        [Route("san-pham/thanh-toan")]
        [HttpGet]
        public IActionResult Checkout()
        {
            return View(new Checkout());
        }
        [Route("san-pham/thanh-toan")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> Checkout(Checkout model)
        {
            try
            {
                var order = new Order();
               
                order.Id = model.Id;
                order.AdjustPrice = 10000;
                order.ShippingFee = 100000;
                order.GrandTotalPrice = order.AdjustPrice + order.ShippingFee;
                order.GuestName = model.Name;
                order.GuestPhone = model.PhoneNumber;
                order.GuestEmail = model.Email;
                order.Address = model.Address;
                order.Note = model.Note;
                order.PaymentInfo = "COD";
                order.OrderStatus = OrderStatus.Pending;
                order.PaymentStatus = PaymentStatus.None;
                order.IsAgree = model.isAgressPrivacy;
                order.CreateTime = DateTime.Now;
                _dbContext.Orders.Add(order);
                var success = await _dbContext.SaveChangesAsync() > 0;
                if (model.Items != null && success)
                {
                    var orderItem = JsonConvert.DeserializeObject<ICollection<CheckoutItem>>(model.Items);
                    var total = 0.0;
                    if (orderItem.Count > 0)
                    {
                        foreach (var item in orderItem)
                        {
                            total += item.Product_Price;
                            var temp = new OrderItem()
                            {
                                OrderId = order.Id,
                                ItemId = item.Product_Id,
                                Quantity = item.Product_Quantity,
                                ItemAttrib = item.NameParent + ": " + item.NameChild,
                            };
                            _dbContext.OrderItems.Add(temp);
                        }
                    }
                }
                _dbContext.SaveChanges();

                return Json(new ModalFormResult() { Code = 1, Message = "Checkout successed" });
            }
            catch (Exception e)
            {
                BadRequest(e.Message);
                return Json(new ModalFormResult() { Code = 0, Message = e.Message });
            }
        }
    }
}
