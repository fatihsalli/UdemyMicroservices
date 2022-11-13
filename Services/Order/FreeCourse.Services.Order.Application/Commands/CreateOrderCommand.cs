using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Shared.Dtos;
using MediatR;
using System.Collections.Generic;

namespace FreeCourse.Services.Order.Application.Commands
{
    //Crud işlemleri için Commands klasöürün oluşturduk. Query ler read işlemleri için
    public class CreateOrderCommand:IRequest<Response<CreatedOrderDto>>
    {
        //Almamaız gereken parametreleri yazıyoruz.
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public AddressDto Address { get; set; }

    }
}
