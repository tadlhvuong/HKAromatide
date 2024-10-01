using HKMain.Models;
using HKShared.Data;
using HKShared.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace HKMain.ViewComponents
{
    [ViewComponent(Name = "MegaMenu")]
    public class MegaMenuViewComponentsViewComponent : ViewComponent
    {
        private readonly AppDBContext _dbContext;
        private readonly string mediaUrl;
        private readonly string mediaPath;
        public MegaMenuViewComponentsViewComponent(AppDBContext applicationDbContext)
        {
            _dbContext = applicationDbContext;

            mediaUrl = AppSettings.Strings["MediaUrl"] ?? "/media/";
            mediaPath = AppSettings.Strings["MediaPath"] ?? "./wwwroot/media";
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var megaMenu = (from p in _dbContext.Products
                            select new MegaMenuModel()
                            {
                                Id = p.Id,
                                Image = Path.Combine(mediaUrl + p.Image),
                                Name = p.Name,
                                Price = p.FormatedCurrentPrice,
                                SalePrice = p.FormatedRegularPrice,
                                RedirectUrl = "san-pham/chi-tiet-san-pham?id=" + p.Id,
                                Taxo = (from pt in _dbContext.ProductTaxos
                                        join t in _dbContext.Taxonomies on pt.TaxoId equals t.Id
                                        where pt.ItemId == p.Id && pt.ItemType == TaxoType.Category && t.ParentId == null
                                        select t.Name).ToArray()
                            });
            return View("Index", megaMenu.ToList());
        }
    }
}
