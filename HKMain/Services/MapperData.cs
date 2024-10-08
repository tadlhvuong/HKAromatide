using AutoMapper;
using HKMain.Areas.Admin.Models;
using HKShared.Data;

namespace HKMain.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CategoryViewModel, Taxonomy>();
            CreateMap<Taxonomy, Taxonomy>();
            CreateMap<OrderItem, Order>();
        }
    }
}
