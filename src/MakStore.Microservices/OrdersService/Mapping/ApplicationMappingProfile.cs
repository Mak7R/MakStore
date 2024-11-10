using AutoMapper;
using MakStore.Domain.Entities;
using OrdersService.Mediator.Commands.CreateOrderCommand;

namespace OrdersService.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateProductMaps();
    }

    private void CreateProductMaps()
    {
        CreateMap<CreateOrderCommand, Order>();
    }
}