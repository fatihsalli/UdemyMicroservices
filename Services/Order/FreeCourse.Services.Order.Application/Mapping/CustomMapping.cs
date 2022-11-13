using AutoMapper;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Domain.OrderAggreagate;

namespace FreeCourse.Services.Order.Application.Mapping
{
    //DI Container olmadığı için Class Library olduğu için static bir "ObjectMapper" kullanarak kendimiz ayağa kaldırdık.
    public class CustomMapping : Profile
    {
        public CustomMapping()
        {
            CreateMap<Order.Domain.OrderAggreagate.Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
        }

    }
}
