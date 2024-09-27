using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HKMain.Areas.Admin.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace HKMain.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator,Manager")]
    public class MediaController : Controller
    {
        private readonly string mediaUrl;
        private readonly string mediaPath;

        private readonly ILogger _logger;
        private readonly AppDBContext _dbContext;
        public MediaController(
            ILogger<MediaController> logger,
            AppDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;

            mediaUrl = AppSettings.Strings["MediaUrl"] ?? "/media";
            mediaPath = AppSettings.Strings["MediaPath"] ?? "./wwwroot/media";
        }

        #region Media
        [Route("Admin/thu-vien")]
        [Route("Admin/thu-vien/FileBrowser")]
        [Route("Admin/thu-vien/FileBrowser/{id?}")]
        public IActionResult FileBrowser(PagedList<MediaFile> model)
        {
            model.PageSize = 10;

            var filterQuery = _dbContext.MediaFiles.Where(x => model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var item in model.Content)
                item.FileLink = Path.Combine(mediaUrl, item.FullPath);

            return View(model);
        }

        [Route("Admin/thu-vien")]
        [Route("Admin/thu-vien/FileManager")]
        [Route("Admin/thu-vien/FileManager/{id?}")]
        public IActionResult FileManager(PagedList<MediaFile> model)
        {
            _logger.LogInformation("Page Album general");
            model.PageSize = 20;

            var filterQuery = _dbContext.MediaFiles.Where(x => model.Search == null || x.FileName.Contains(model.Search) || x.MediaAlbum.FullName.Contains(model.Search));
            var selectQuery = filterQuery.OrderByDescending(x => x.Id).Skip((model.CurPage - 1) * model.PageSize).Take(model.PageSize);

            model.TotalRows = filterQuery.Count();
            model.Content = selectQuery.ToList();

            foreach (var item in model.Content)
                item.FileLink = Path.Combine(mediaUrl, item.FullPath);


            var first = _dbContext.MediaAlbums.Where(x => x.Id < 4);
            var AllAlbum = new List<MediaAlbum>();
            var last = _dbContext.MediaAlbums.Where(x => x.Id > 3).OrderByDescending(x => x.Id).ToList();
            AllAlbum.AddRange(first);
            AllAlbum.AddRange(last);
            ViewBag.Album = new SelectList(AllAlbum, "Id", "ShortName");

            return View(model);
        }

        [Route("Admin/Media/FileUpload/{id?}")]
        [HttpPost]
        public async Task<IActionResult> FileUpload(int? id, IEnumerable<IFormFile> files)
        {
            _logger.LogInformation("File upload");
            MediaAlbum album = GetDefaultAlbum(id);
            if (album == null)
            {
                _logger.LogWarning("Could not create Default album");
                return BadRequest("Could not create Default Album!");
            }

            return await DoUploadFile(album, files, _dbContext);
        }

        [Route("Admin/Media/FileUploadCrop/{id?}")]
        [HttpPost]
        public async Task<IActionResult> FileUploadCrop(FileUploadModel model)
        {
            MediaAlbum album = GetDefaultAlbum(model.Id);
            if (album == null)
            {

                _logger.LogWarning("File upload crop create default album fail");
                return BadRequest("Could not create Default Album!");
            }

            var fileData = model.FileData;
            var pos = fileData.IndexOf(";base64,");
            if (pos <= 0)
            {
                _logger.LogWarning("File upload crop invalid filedata");
                return BadRequest("Invalid FileData");
            }

            fileData = fileData.Substring(pos + 8);
            var fileBytes = Convert.FromBase64String(fileData);

            return await DoUploadFileCrop(album, model.FileName, fileBytes, _dbContext);
        }

        [Route("Admin/Media/FileRemove/{id?}")]
        [HttpPost]
        public IActionResult FileRemove(int? key)
        {
            MediaFile model = _dbContext.MediaFiles.FirstOrDefault(x => x.Id == key);
            if (model == null)
            {
                _logger.LogDebug("File remove not found");
                return BadRequest("File not found!");
            }

            try
            {
                _dbContext.MediaFiles.Remove(model);
                _dbContext.SaveChanges();

                _logger.LogDebug("File remove success");
                string fileName = Path.Combine(mediaPath, model.FullPath);
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);

                return Json(new ModalFormResult() { Code = 1, Message = "File deleted!" });
            }
            catch (Exception ex)
            {
                _logger.LogDebug("File remove fail");
                return BadRequest(ex.Message);

            }
        }

        private async Task<IActionResult> DoUploadFile(MediaAlbum album, IEnumerable<IFormFile> files, AppDBContext dbContext)
        {
            try
            {
                string albumDir = Path.Combine(mediaPath, album.ShortName);
                if (!Directory.Exists(albumDir))
                    Directory.CreateDirectory(albumDir);

                List<MediaFile> newFiles = new List<MediaFile>();
                foreach (var file in files)
                {
                    string fileName = file.FileName.ToLower();
                    string fileExt = Path.GetExtension(fileName);

                    while (true)
                    {
                        fileName = Common.Random_Mix(6).ToLower() + fileExt;
                        string filePath = Path.Combine(albumDir, fileName);
                        if (!System.IO.File.Exists(filePath))
                        {
                            using (var stream = new FileStream(filePath, FileMode.Create))
                                await file.CopyToAsync(stream);

                            break;
                        }
                    }

                    MediaFile newFile = new MediaFile()
                    {
                        AlbumId = album.Id,
                        FileName = file.FileName.ToLower(),
                        FullPath = Path.Combine(album.ShortName, fileName),
                        FileSize = file.Length,
                        CreateTime = DateTime.Now,
                    };

                    newFiles.Add(newFile);
                    dbContext.MediaFiles.Add(newFile);
                }

                _logger.LogDebug("File upload success");
                dbContext.SaveChanges();
                return new JsonResult(new FileUploadResult
                {
                    initialPreview = newFiles.Select(x => Path.Combine(mediaUrl, x.FullPath)).ToArray(),
                    initialPreviewConfig = newFiles.Select(x => new { key = x.Id, caption = x.FileName, size = x.FileSize, showDrag = false }).ToArray(),
                });
            }
            catch (Exception ex)
            {
                _logger.LogDebug("File upload fail");
                return BadRequest(ex.Message);
            }
        }

        private async Task<IActionResult> DoUploadFileCrop(MediaAlbum album, string fileName, byte[] fileData, AppDBContext dbContext)
        {
            try
            {
                string albumDir = Path.Combine(mediaPath, album.ShortName);
                if (!Directory.Exists(albumDir))
                    Directory.CreateDirectory(albumDir);

                string fileExt = string.IsNullOrEmpty(fileName) ? ".jpg" : Path.GetExtension(fileName);

                string newName = "";
                while (true)
                {
                    newName = Common.Random_Mix(6).ToLower() + fileExt;
                    string filePath = Path.Combine(albumDir, newName);
                    if (!System.IO.File.Exists(filePath))
                    {
                        await System.IO.File.WriteAllBytesAsync(filePath, fileData);
                        break;
                    }
                }

                MediaFile newFile = new MediaFile()
                {
                    AlbumId = album.Id,
                    FullPath = Path.Combine(album.ShortName, newName),
                    FileSize = fileData.Length,
                    CreateTime = DateTime.Now
                };

                dbContext.MediaFiles.Add(newFile);
                dbContext.SaveChanges();

                _logger.LogDebug("File upload crop success");
                var newFiles = new List<MediaFile> { newFile };
                return new JsonResult(new FileUploadResult
                {
                    initialPreview = newFiles.Select(x => Path.Combine(mediaUrl, x.FullPath)).ToArray(),
                    initialPreviewConfig = newFiles.Select(x => new { key = x.Id, caption = x.FileName, size = x.FileSize, showDrag = false }).ToArray(),
                });
            }
            catch (Exception ex)
            {
                _logger.LogDebug("File upload crop fail");
                return BadRequest(ex.Message);
            }
        }

        #endregion

        //[HttpPost]
        //public async Task<IActionResult> FileUploadBlog(int? id, IEnumerable<IFormFile> files)
        //{
        //    ShopItem model = _dbContext.ShopItems.Find(id);
        //    if (model == null)
        //        return BadRequest();

        //    if (model.MediaAlbum == null)
        //    {
        //        try
        //        {
        //            model.MediaAlbum = new MediaAlbum()
        //            {
        //                UserId = 1,
        //                ShortName = string.Format("Blog{0:D4}", model.Id),
        //                FullName = "Album " + model.Name,
        //                CreateTime = DateTime.Now,
        //            };

        //            _dbContext.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return await DoUploadFile(model.MediaAlbum, files, _dbContext);
        //}

        //[HttpPost]
        //public async Task<IActionResult> FileUploadProduct(int? id, IEnumerable<IFormFile> files)
        //{
        //    ShopItem model = _dbContext.ShopItems.Find(id);
        //    if (model == null)
        //        return BadRequest();

        //    if (model.MediaAlbum == null)
        //    {
        //        try
        //        {
        //            model.MediaAlbum = new MediaAlbum()
        //            {
        //                UserId = 1,
        //                ShortName = string.Format("Item{0:D4}", model.Id),
        //                FullName = "Album " + model.Name,
        //                CreateTime = DateTime.Now,
        //            };

        //            _dbContext.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return await DoUploadFile(model.MediaAlbum, files, _dbContext);
        //}

        #region Album

        [Route("Admin/thu-vien/danh-sach-album")]
        public IActionResult AlbumList()
        {
            _logger.LogInformation("Page list album");
            //ViewBag.UrlCRUD = JsonConvert.DeserializeObject("[{\"load\":\"/Admin/Media/LoadAlbumList\"},{\"create\":\"/Admin/Media/AlbumCreate\"},{\"update\":\"/Admin/Media/AlbumUpdate/\"},{\"delete\":\"/Admin/Media/AlbumDelete/\"}]");
            //ViewBag.Columns = JsonConvert.DeserializeObject("[{\"data\":\"\"},{\"data\":\"Id\"},{\"data\":\"ShortName\"},{\"data\":\"FullName\"},{\"data\":\"Description\"},{\"data\":\"action\"}]");
            return View();
        }
        [Route("Admin/Media/LoadAlbumList")]
        public JsonResult LoadAlbumList()
        {
            _logger.LogInformation("Load list album");
            var model = _dbContext.MediaAlbums.ToList();
            return Json(new ModalFormResult() { Code = 1, Message = "Load data", data = JsonConvert.SerializeObject(model) });
        }

        public IActionResult AlbumCreate()
        {
            _logger.LogInformation("Page create album");
            return View("AlbumEdit", new MediaAlbum());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AlbumCreate(MediaAlbum model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.UserId = this.User.GetUserId();
                    model.AppUser = _dbContext.Users.Find(this.User.GetUserId());
                    model.MediaFiles = null;
                    model.CreateTime = DateTime.Now;
                    _dbContext.MediaAlbums.Add(model);
                    _dbContext.SaveChanges();

                    _logger.LogDebug("Create album success");
                    var newTaxo = new Taxonomy()
                    {
                        Name = model.ShortName,
                        Type = TaxoType.Album
                    };
                    _dbContext.Taxonomies.Add(newTaxo);
                    _dbContext.SaveChanges(true);
                    _logger.LogDebug("Create taxo album success");

                    return Json(new ModalFormResult() { Code = 1, Message = "Thêm thành công", data = JsonConvert.SerializeObject(model) });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Create new album fail");
                    if (ex.InnerException != null && ex.InnerException.ToString().Contains("Duplicate entry"))
                        ModelState.AddModelError("", "Album không được trùng Code");
                    else
                        ModelState.AddModelError("", ex.Message);
                }
            }
            return View("AlbumEdit", model);
        }

        public IActionResult AlbumEdit(int? id)
        {
            MediaAlbum model = _dbContext.MediaAlbums.FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                _logger.LogWarning("Page update album fail");
                return BadRequest();
            }

            _logger.LogInformation("Page update album");
            return View("AlbumEdit", model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AlbumEdit(MediaAlbum model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldName = _dbContext.MediaAlbums.AsNoTracking().Where(x => x.Id == model.Id).FirstOrDefault().ShortName;

                    //update mediaalbum
                    model.UserId = this.User.GetUserId();
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    _logger.LogDebug("Update album");

                    //update relationship taxonomy_mediaalbum
                    var taxo = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Album && x.Name.Equals(oldName)).FirstOrDefault();

                    var taxoNew = taxo;
                    taxoNew.Name = model.ShortName;
                    _dbContext.Entry(taxoNew).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    _logger.LogDebug("Update taxonomy album");

                    //update direct image in mediafile
                    model.MediaFiles = _dbContext.MediaFiles.Where(x => x.AlbumId == model.Id).ToList();
                    if (model.MediaFiles != null)
                    {
                        foreach (var image in model.MediaFiles)
                        {
                            string filename = Path.GetFileName(image.FullPath);
                            string dirNew = Path.Combine(model.ShortName, filename);
                            image.FullPath = dirNew;
                        }
                        _dbContext.MediaFiles.UpdateRange(model.MediaFiles);
                        _dbContext.SaveChanges();
                    }
                    _logger.LogDebug("Update mediafile album");
                    string albumDir = Path.Combine(mediaPath, oldName);
                    string albumDirNew = Path.Combine(mediaPath, model.ShortName);
                    if (Directory.Exists(albumDir) && !albumDirNew.Equals(albumDir))
                        Directory.Move(albumDir, albumDirNew);
                    _logger.LogDebug("Update folder directory album");

                    _logger.LogDebug("Update album success");
                    return Json(new ModalFormResult() { Code = 1, Message = "Cập nhật " + model.ShortName + " thành công", data = JsonConvert.SerializeObject(model) });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Update album fail");
                    if (ex.InnerException != null && ex.InnerException.ToString().Contains("Duplicate entry"))
                        ModelState.AddModelError("", "Album không được trùng Code");
                    else
                        ModelState.AddModelError("", ex.Message);
                }
            }

            return View("AlbumEdit", model);
        }

        [Route("Admin/Media/AlbumDelete/{id?}")]
        public IActionResult AlbumDelete(int? id)
        {
            var model = _dbContext.MediaAlbums.Where(s => s.Id == id).Select(x => new CommonModelRemove()
            {
                Id = x.Id,
                Name = x.ShortName
            }).FirstOrDefault();
            _logger.LogInformation("Delete album");
            return View("_AdminModalDelete", model);
        }
        [Route("Admin/Media/AlbumDelete/{id?}")]
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult AlbumDelete(int id)
        {
            try
            {
                var albumAlbum = _dbContext.MediaAlbums;
                if (albumAlbum.Count() == 1)
                    return Json(new ModalFormResult() { Code = 0, Message = "Không được xóa hết album (Album đầu là album mặc định)", data = null });

                MediaAlbum model = albumAlbum.Find(id);

                _dbContext.MediaAlbums.Remove(model);
                _dbContext.SaveChanges();
                _logger.LogDebug("Delete album");

                var images = _dbContext.MediaFiles.Where(x => x.AlbumId == id).ToList();
                if (images.Count > 0)
                {
                    _dbContext.MediaFiles.RemoveRange(images);
                    _dbContext.SaveChanges();
                }
                _logger.LogDebug("Delete media album");
                var taxo = _dbContext.Taxonomies.Where(x => x.Type == TaxoType.Album && x.Name.Equals(model.ShortName)).FirstOrDefault();
                if (taxo != null)
                {
                    _dbContext.Taxonomies.Remove(taxo);
                    _dbContext.SaveChanges();
                    _logger.LogDebug("Delete taxonomy album");
                }

                string albumDir = Path.Combine(mediaPath, model.ShortName);
                if (Directory.Exists(albumDir))
                    Directory.Delete(albumDir, true);
                _logger.LogDebug("Delete folder directory album");

                var title = new string[] { "STT", "Code", "Tên Album", "Miêu tả", "Thao tác" };
                //var dataLoad = _dbContext.MediaAlbums.Select(x => new MediaAlbumTaxoModel { Title = title, NameAlbum = x.ShortName, FullName = x.FullName, Description = x.Description, Id = x.Id }).ToList();
                var dataLoad = JsonConvert.SerializeObject(_dbContext.MediaAlbums.Select(x => new { x.Id, x.ShortName, x.FullName, x.Description }));

                _logger.LogWarning("Delete album success");
                return Json(new ModalFormResult() { Code = 1, Message = "Xóa Album " + model.ShortName + " thành công", data = dataLoad });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Delete album fail");
                return Json(new ModalFormResult() { Code = 0, Message = "Xóa Album failed", data = null });
            }
            return Json(new ModalFormResult() { Code = 0, Message = "Xóa Album failed", data = null });
        }
        #endregion

        #region Helper
        private MediaAlbum GetDefaultAlbum(int? id)
        {
            try
            {
                MediaAlbum album = _dbContext.MediaAlbums.FirstOrDefault(x => x.Id == id);
                if (album == null) album = _dbContext.MediaAlbums.FirstOrDefault(x => x.ShortName == "Default");
                if (album == null)
                {
                    album = new MediaAlbum()
                    {
                        Id = 1,
                        ShortName = "Default",
                        FullName = "Default Album",
                        Description = "Album mặc định",
                        UserId = this.User.GetUserId(),
                        CreateTime = DateTime.Now,
                    };

                    _dbContext.MediaAlbums.Add(album);
                    _dbContext.SaveChanges();
                }

                _logger.LogDebug("Create default album success");
                return album;
            }
            catch
            {
                _logger.LogDebug("Create default album fail");
                return null;
            }
        }

        #endregion
    }
}
