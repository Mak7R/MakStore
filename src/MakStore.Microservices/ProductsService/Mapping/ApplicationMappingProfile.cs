using AutoMapper;
using ProductsService.Dtos;
using ProductsService.Mediator.Commands.CreateProductCommand;
using ProductsService.Mediator.Commands.DeleteProductCommand;
using ProductsService.Mediator.Commands.UpdateProductCommand;
using ProductsService.Models;

namespace ProductsService.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateProductMaps();
    }

    private void CreateProductMaps()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();
        CreateMap<DeleteProductCommand, Product>();
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}