using AutoMapper;
using ProductAPI.Models;
using ProductAPI.Protos;

namespace ProductAPI.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        // Product -> ProductMessage
        CreateMap<Product, ProductMessage>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.ToString())
            );

        // CreateProductRequest -> Product
        CreateMap<CreateProductRequest, Product>();

        // UpdateProductRequest -> Product>()
        CreateMap<UpdateProductRequest, Product>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => Guid.Parse(src.Id))
            );
    }
}