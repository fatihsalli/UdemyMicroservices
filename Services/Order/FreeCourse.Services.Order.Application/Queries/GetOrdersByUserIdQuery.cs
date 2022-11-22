using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Shared.Dtos;
using MediatR;
using System.Collections.Generic;

namespace FreeCourse.Services.Order.Application.Queries
{
    //Controller tarafında "GetOrdersByUserIdQuery" nesne örneği oluşturup UserId kısmını doldurup MediatR'a gönderdiğimizde Handle edecek sınıfı kendi bulacak.
    public class GetOrdersByUserIdQuery : IRequest<Response<List<OrderDto>>>
    {
        public string UserId { get; set; }



    }
}
