using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HKMain.Areas.Admin.Models;
using HKMain.Controllers;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using HKMain.Controllers;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, Manager")]
    public class ProductController : Controller
    {
        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;
        private readonly MediaController _mediaController;

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

        #region Product

        // GET: Load data Product
        [Route("Admin/Product/LoadProductList")]
        public JsonResult LoadProductList()
        {
            _logger.LogInformation("Load list product");
            try
            {
                var pt = _dbContext.ProductTaxos.ToList();
                var t = _dbContext.Taxonomies.ToList();
                var model = _dbContext.Products.ToList();
                List<ProductViewModel> lvm = new List<ProductViewModel>();
                foreach ( var item in model )
                {
                    ProductViewModel vm = new ProductViewModel();
                    vm.IdProduct = item.Id;
                    vm.SKU = item.SKU;
                    vm.Name = item.Name;
                    vm.Image = item.Image;
                    vm.Price = item.FormatedRegularPrice;
                    vm.SalePrice = item.FormatedSalePrice;
                    vm.Quantity = item.Quantity;
                    vm.Stock = item.Stock;
                    vm.Status = item.Status;
                    if (item.Taxonomies != null)
                    {
                        List<string> taxoString = new List<string>();
                        foreach (var taxos in item.Taxonomies)
                        {
                            var taxo = _dbContext.Taxonomies.Find(taxos.TaxoId);
                            if(taxo.Type == TaxoType.Category)
                                vm.Category = taxo.Name;
                            else if(taxo.Type == TaxoType.Collection)
                                vm.Collection = taxo.Name;
                            else
                                vm.Vendor = taxo.Name;
                        }
                    }
                    lvm.Add(vm);

                }

                return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(lvm) });
            }
            catch (Exception )
            {
                throw;
            }
        }

        // GET: Load data variant
        [Route("Admin/Product/GetVariant/{id?}")]
        public JsonResult GetVariant(int? id)
        {
            var model = _dbContext.ItemVariantsParent.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            if (model != null)
            {
                foreach (var item in model)
                {
                    var listChild = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Variants && x.ParentId == item.Id).Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
                    item.ItemsChild = listChild;
                }
            }

            var attrs = _dbContext.ProductAttribs.Where(x => x.ItemId == id).Select(x => new ProductAttrModel { Parent = x.AttrId, Child = x.AttrChildId });

            foreach (var a in attrs)
            {
                foreach (var item in model)
                {
                    if (a.Parent == item.Id) item.Selected = true;
                    foreach (var itemChild in item.ItemsChild)
                    {
                        if (a.Child == itemChild.Id) itemChild.Selected = true;
                    }
                }
            }
            return Json(new ModalFormResult() { Code = 1, Message = "Get variant success!", data = JsonConvert.SerializeObject(model), SubData = JsonConvert.SerializeObject(attrs) });
        }

        // GET: Product
        [Route("Admin/san-pham")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Product/Details/5
        [Route("Admin/san-pham/chi-tiet/{id?}")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Edit
        [Route("Admin/san-pham/chinh-sua/{id?}")]
        public ActionResult Edit(int id)
        {
            var itemVariants = _dbContext.ItemVariantsParent.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            var itemCategories = _dbContext.ItemCats.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            var itemVendors = _dbContext.ItemVens.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            var itemCollection = _dbContext.ItemCollecs.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            var itemVariantsParent = _dbContext.ItemVariantsParent.Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
            if (itemVariants != null)
            {
                foreach (var item in itemVariants)
                {
                    var listChild = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Variants && x.ParentId == item.Id).Select(x => new SelectItemModel() { Id = x.Id, Text = x.Name }).ToList();
                    item.ItemsChild = listChild;
                }
            }
            ViewBag.ItemVariants = itemVariants;

            var model = new ProductFormModel();
            var mediaFiles = new List<MediaFile>();
            try
            {
                if (id != 0)
                {
                    var product = _dbContext.Products.Include(x => x.MediaAlbum.MediaFiles).FirstOrDefault(x => x.Id == id);
                    if (product.Image != "")
                    {
                        if (product.AlbumId != 0)
                            if (product.MediaAlbum.MediaFiles.Count > 0)
                            {
                                var mf = product.MediaAlbum.MediaFiles.SingleOrDefault(x => x.FullPath.Equals(product.Image));
                                model.IdFile = (mf != null) ? mf.Id : 0;
                            }
                    }
                    model.Name = product.Name;
                    model.SKU = product.SKU;
                    model.IdAlbum = product.AlbumId;
                    model.Quantity = product.Quantity;
                    model.Preview = product.Preview;
                    model.Content = product.Content;
                    model.Price = product.Price / 1000;
                    model.SalePrice = product.SalePrice / 1000;
                    model.Stock = product.Stock;
                    model.Status = product.Status;
                    model.Tags = product.Tags;

                    var Cate = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == id && x.ItemType == TaxoType.Category);
                    var idCate = (Cate != null) ? Cate.TaxoId : 0;
                    var Collec = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == id && x.ItemType == TaxoType.Collection);
                    var idCollec = (Collec != null) ? Collec.TaxoId : 0;
                    var Vendor = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == id && x.ItemType == TaxoType.Vendor);
                    var idVendor = (Vendor != null) ? Vendor.TaxoId : 0;
                    foreach (var item in itemCategories)
                    {
                        if (item.Id == idCate)
                            item.Selected = true;
                    }
                    foreach (var item in itemCollection)
                    {
                        if (item.Id == idCollec)
                            item.Selected = true;
                    }
                    foreach (var item in itemVendors)
                    {
                        if (item.Id == idVendor)
                            item.Selected = true;
                    }

                }
                else
                {
                    var newAlbum = CreateProductAlbum();
                    model.IdAlbum = newAlbum.Id;
                }

                mediaFiles = _dbContext.MediaFiles.Where(x => x.AlbumId == model.IdAlbum).ToList();
                if (mediaFiles.Count > 0)
                    foreach (var item in mediaFiles)
                        item.FileLink = Path.Combine(mediaUrl, item.FullPath);

                ViewBag.ItemCats = itemCategories;
                ViewBag.ItemVens = itemVendors;
                ViewBag.ItemCollecs = itemCollection;
                ViewBag.ItemVariants = itemVariantsParent;
                model.MediaFile = mediaFiles;
            }
            catch (Exception)
            {
                throw;
            }


            return View(model);
        }
        // POST: Product/Edit
        [Route("Admin/san-pham/chinh-sua/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(ProductFormModel model)
        {
            try
            {
                var oldProduct = _dbContext.Products.FirstOrDefault(x => x.Id == model.Id);
                var mediaFile = "";
                if (model.IdFile != null)
                    mediaFile = _dbContext.MediaFiles.Find(model.IdFile).FullPath;

                var newProduct = new Product()
                {
                    Id = model.Id,
                    Name = model.Name,
                    AlbumId = model.IdAlbum,
                    Image = mediaFile,
                    Preview = model.Preview,
                    Content = model.Content,
                    Quantity = model.Quantity,
                    Price = model.Price * 1000,
                    SalePrice = model.SalePrice * 1000,
                    SKU = model.SKU,
                    Stock = model.Stock,
                    Status = model.Status,
                    Tax = model.Tax,
                    Tags = model.Tags,
                    CreateTime = DateTime.Now,
                    UserId = this.User.GetUserId(),
                };
                if (oldProduct != null)
                {
                    _dbContext.Entry(oldProduct).CurrentValues.SetValues(newProduct);
                }
                else
                {
                    _dbContext.Products.Add(newProduct);
                }
                _dbContext.SaveChanges();

                //category...
                var oldCate = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == model.Id && x.ItemType == TaxoType.Category);
                var ProductNew = _dbContext.Products.OrderByDescending(x => x.Id).FirstOrDefault();
                var idProductNew = (ProductNew != null) ? ProductNew.Id : 0;
                if(idProductNew != 0)
                {
                    if (oldCate != null)
                    {
                        if (oldCate.TaxoId != model.ItemCategory)
                        {
                            var newCate = new ProductTaxo()
                            {
                                Id = oldCate.Id,
                                ItemId = idProductNew,
                                TaxoId = model.ItemCategory,
                                ItemType = TaxoType.Category
                            };
                            _dbContext.Entry(oldCate).CurrentValues.SetValues(newCate);
                        }
                    }
                    else
                    {
                        if (model.ItemCategory != 0)
                        {
                            var newCate = new ProductTaxo()
                            {
                                ItemId = idProductNew,
                                TaxoId = model.ItemCategory,
                                ItemType = TaxoType.Category
                            };
                            _dbContext.ProductTaxos.Add(newCate);
                        }
                    }

                    var oldCollec = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == model.Id && x.ItemType == TaxoType.Collection);
                    if (oldCollec != null)
                    {
                        if (oldCollec.TaxoId != model.ItemCollection)
                        {
                            var newCollec = new ProductTaxo()
                            {
                                Id = oldCollec.Id,
                                ItemId = idProductNew,
                                TaxoId = model.ItemCollection,
                                ItemType = TaxoType.Collection
                            };
                            _dbContext.Entry(oldCollec).CurrentValues.SetValues(newCollec);
                        }
                    }
                    else
                    {
                        if (model.ItemCollection != 0)
                        {
                            var newCollec = new ProductTaxo()
                            {
                                ItemId = idProductNew,
                                TaxoId = model.ItemCollection,
                                ItemType = TaxoType.Collection
                            };
                            _dbContext.ProductTaxos.Add(newCollec);
                        }
                    }

                    var oldVendor = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == model.Id && x.ItemType == TaxoType.Vendor);
                    if (oldVendor != null)
                    {
                        if (oldVendor.TaxoId != model.ItemVendor)
                        {
                            var newVendor = new ProductTaxo()
                            {
                                Id = oldVendor.Id,
                                ItemId = idProductNew,
                                TaxoId = model.ItemVendor,
                                ItemType = TaxoType.Vendor
                            };
                            _dbContext.Entry(oldVendor).CurrentValues.SetValues(newVendor);
                        }
                    }
                    else
                    {
                        if (model.ItemVendor != 0)
                        {
                            var newVendor = new ProductTaxo()
                            {
                                ItemId = idProductNew,
                                TaxoId = model.ItemVendor,
                                ItemType = TaxoType.Vendor
                            };
                            _dbContext.ProductTaxos.Add(newVendor);
                        }
                    }
                    // Attrubutes
                    if (model.Attrubutes != null)
                    {
                        var attrs = JArray.Parse(model.Attrubutes);

                        foreach (var item in attrs)
                        {
                            var oldAttrs = _dbContext.ProductAttribs.Where(x => x.ItemId == idProductNew).ToList();
                            if (oldAttrs.Count > 0)
                            {
                                foreach (var attr in oldAttrs)
                                {
                                    if (attr.AttrId == item.First.Value<int>())
                                        _dbContext.ProductAttribs.Remove(attr);
                                }
                            }
                            var newAttr = new ProductAttr()
                            {
                                AttrId = item.First.Value<int>(),
                                AttrChildId = item.Last.Value<int>(),
                                ItemId = idProductNew,
                                Values = ""
                            };
                            _dbContext.ProductAttribs.Add(newAttr);
                        }
                    }
                    else
                    {
                        var oldAttrs = _dbContext.ProductAttribs.Where(x => x.ItemId == idProductNew).ToList();
                        if (oldAttrs.Count > 0)
                            _dbContext.ProductAttribs.RemoveRange(oldAttrs);
                    }
                    _dbContext.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // GET: Product/Delete
        [Route("Admin/san-pham/xoa-san-pham/{id?}")]
        public ActionResult Delete(int id)
        {
            _logger.LogInformation("Delete product:" + id);
            var model = _dbContext.Products.Where(s => s.Id == id).Select(x => new CommonModelRemove()
            {
                Id = x.Id,
                Name = x.Name
            }).FirstOrDefault();
            return View("_AdminModalDelete", model);
        }
        // POST: Product/Delete
        [Route("Admin/san-pham/xoa-san-pham/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var variant = _dbContext.ProductAttribs.FirstOrDefault(x => x.ItemId == id);
                if (variant != null)
                {
                    _dbContext.ProductAttribs.RemoveRange(variant);
                    _dbContext.SaveChanges();
                }


                var taxo = _dbContext.ProductTaxos.FirstOrDefault(x => x.ItemId == id);
                if (taxo != null)
                {
                    _dbContext.ProductTaxos.RemoveRange(taxo);
                    _dbContext.SaveChanges();
                }

                var model = _dbContext.Products.Find(id);
                _dbContext.Products.Remove(model);
                _dbContext.SaveChanges();

                return Json(new ModalFormResult() { Code = 1, Message = "remove product success!", data = JsonConvert.SerializeObject(model) });
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Category 

        [Route("Admin/Product/LoadCategoryList")]
        public JsonResult LoadCategoryList()
        {
            _logger.LogInformation("Load list category");
            var model = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Category).Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                ParentName = x.ParentName
            }).ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }

        [Route("Admin/Product/LoadCollectionList")]
        public JsonResult LoadCollectionList()
        {
            _logger.LogInformation("Load list collection");
            var model = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Collection).Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                ParentName = x.ParentName
            }).ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }

        [Route("Admin/Product/LoadVendorList")]
        public JsonResult LoadVendorList()
        {
            _logger.LogInformation("Load list vendor");
            var model = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Vendor).Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                ParentName = x.ParentName
            }).ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }
        [Route("Admin/Product/LoadVariantList")]
        public JsonResult LoadVariantList()
        {
            _logger.LogInformation("Load list vendor");
            var model = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Variants).Select(x => new CategoryViewModel()
            {
                Id = x.Id,
                Name = x.Name,
                ParentName = x.ParentName
            }).ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }
        // GET: Category/Category
        [Route("Admin/san-pham/danh-muc-san-pham")]
        public ActionResult Category()
        {
            _logger.LogInformation("Page member");
            return View();
        }
        // GET: Category/Create
        [Route("Admin/Product/CategoryCreate/{typeCate?}")]
        public ActionResult CategoryCreate(int typeCate)
        {
            _logger.LogInformation("Create category");
            var CateParent = _dbContext.Taxonomies.Where(x => x.Type == (TaxoType)typeCate && x.ParentId == null).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }).ToList();
            ViewBag.ListParent = CateParent;
            return View("CategoryEdit", new Taxonomy() { Type = (TaxoType)typeCate });
        }
        // POST: Category/Create
        [Route("Admin/Product/CategoryCreate/{typeCate?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryCreate(int typeCate, Taxonomy model)
        {
            try
            {
                _dbContext.Entry(model).State = EntityState.Added;
                _dbContext.SaveChanges();
                var modelParent = _dbContext.Taxonomies.AsNoTracking().FirstOrDefault(x => x.Id == model.ParentId);
                var mapData = new CategoryViewModel()
                {
                    Id = model.Id,
                    Name = model.Name,
                    ParentName = modelParent != null ? modelParent.Name : null
                };
                _logger.LogDebug("Create Category success");
                return Json(new ModalFormResult() { Code = 1, Message = "Thêm" + model.Name + " thành công", data = JsonConvert.SerializeObject(mapData) });
            }
            catch
            {
                _logger.LogDebug("Create Category failed");
                return Json(new ModalFormResult() { Code = 0, Message = "Thêm thất bại" });
            }
        }

        // GET: Category/Edit/5
        [Route("Admin/Product/CategoryEdit/{id?}/{typeCate?}")]
        public ActionResult CategoryEdit(int id, int typeCate)
        {   
            _logger.LogInformation("Edit category:" + id, "Type: " + typeCate);
            var cateList = _dbContext.Taxonomies.Where(x => x.Type == (TaxoType)typeCate).ToList();
            List<SelectListItem> listParent = new List<SelectListItem>();
            var modelCate = new Taxonomy();
            foreach (var cate in cateList)
            {
                if (cate.Id == id)
                {
                    modelCate = cate;
                }
                else
                {
                    if (cate.ParentId != id)
                    {
                        if(cate.ParentId == null)
                        {
                            SelectListItem item = new SelectListItem()
                            {
                                Text = cate.Name,
                                Value = cate.Id.ToString(),
                            };
                            listParent.Add(item);
                        }
                    }
                }
            }
            ViewBag.ListParent = listParent;
            return View("CategoryEdit", modelCate);
        }
        // POST: Category/Edit/5
        [Route("Admin/Product/CategoryEdit/{id?}/{typeCate?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(int id, int typeCate, Taxonomy model)
        {
            try
            {
                _dbContext.Entry(model).State = EntityState.Modified;
                _dbContext.SaveChanges();
                var delay = _dbContext.Taxonomies.Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentName = x.ParentName
                }).ToList();
                var mapData = _dbContext.Taxonomies.Select(x => new CategoryViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentName = x.ParentName
                }).ToList().Where(x => x.Id == id).SingleOrDefault();
                _logger.LogDebug("Edit Category success");
                return Json(new ModalFormResult() { Code = 1, Message = "Cập nhật " + model.Name + " thành công", data = JsonConvert.SerializeObject(mapData) });
            }
            catch
            {
                _logger.LogDebug("Edit Category failed");
                return Json(new ModalFormResult() { Code = 0, Message = "Cập nhật thất bại" });
            }
        }

        // GET: Category/Delete/5
        [Route("Admin/Product/CategoryDelete/{id?}/{typeCate?}")]
        public ActionResult CategoryDelete(int id, int typeCate)
        {
            _logger.LogInformation("Delete category:" + id + ", Type: " + typeCate);
            var model = _dbContext.Taxonomies.Where(s => s.Type == (TaxoType)typeCate && s.Id == id).Select(x => new CommonModelRemove()
            {
                Id = x.Id,
                Name = x.Name
            }).FirstOrDefault();
            return View("_AdminModalDelete", model);
        }
        // POST: Category/Delete/5
        [Route("Admin/Product/CategoryDelete/{id?}/{typeCate?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CategoryDelete(int id, int typeCate, IFormCollection collection)
        {
            try
            {
                var model = _dbContext.Taxonomies.Where(s => s.Type == (TaxoType)typeCate).ToList().Where(x => x.Id == id).FirstOrDefault();
                if (model.Children != null)
                {
                    foreach (var child in model.Children)
                    {
                        child.ParentId = null;
                    }
                }
                _dbContext.Remove(model);
                _dbContext.SaveChanges();

                _logger.LogDebug("Delete category success");
                return Json(new ModalFormResult() { Code = 1, Message = "Xóa thành công", data = null });
            }
            catch
            {
                _logger.LogDebug("Delete category failed");
                return Json(new ModalFormResult() { Code = 0, Message = "Xóa dữ liệu lỗi", data = null });
            }
        }

        #endregion

        #region Helpers

        private MediaAlbum? CreateProductAlbum()
        {
            try
            {
                var newAlbum = "Product_" + DateTime.Now.ToString("MMMM", CultureInfo.CreateSpecificCulture("en")) + DateTime.Now.Year;
                var album = _dbContext.MediaAlbums.Where(x => x.ShortName.Equals(newAlbum)).FirstOrDefault();
                if (album == null)
                {
                    album = new MediaAlbum()
                    {
                        ShortName = newAlbum,
                        FullName = "Product Album " + DateTime.Now.Month + "/" + DateTime.Now.Year,
                        Description = "Album sản phẩm lưu trữ hình ảnh upload " + DateTime.Now.Month + "/" + DateTime.Now.Year,
                        UserId = this.User.GetUserId(),
                        CreateTime = DateTime.Now,
                    };
                    _dbContext.MediaAlbums.Add(album);
                    _dbContext.SaveChanges();
                    _logger.LogDebug("Create default album success");
                    album = _dbContext.MediaAlbums.FirstOrDefault(x => x.ShortName.Equals(newAlbum));
                    return album;
                }
                else
                {
                    _logger.LogDebug("Album already exits");
                    return album;
                }
            }
            catch
            {
                _logger.LogDebug("Create album fail");
                return null;
            }
        }

        #endregion
    }
}
