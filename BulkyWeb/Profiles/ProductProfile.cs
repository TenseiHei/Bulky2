using AutoMapper;
using Bulky.Models;
using Bulky.Utility.DTO.Products;

namespace BulkyWeb.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
