using AutoMapper.Internal.Mappers;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Application.Mapping;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Handlers
{
    //Veritabanına gidip datayı alacağım sınıfım burası olacak
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrdersByUserIdQuery, Response<List<OrderDto>>>
    {
        private readonly OrderDbContext _context;

        public GetOrderByUserIdQueryHandler(OrderDbContext context)
        {
            _context = context;
        }

        //MediatR'a bu "GetOrdersByUserIdQuery" classını gönderdiğimizde gelip bu metodu çalıştıracaktır.
        public async Task<Response<List<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
        {
            //OrderItemsları da dahil ettik.
            var orders=await _context.Orders.Include(x=> x.OrderItems).Where(x=> x.BuyerId==request.UserId).ToListAsync();

            //Maplemeden önce dolu mu boş mu diye kontrol ediyoruz. Yoksa Automapper kullanırken hata alırız.
            if (!orders.Any())
            {
                return Response<List<OrderDto>>.Success(new List<OrderDto>(), 200);
            }

            var orderDtos=ObjectMapper.Mapper.Map<List<OrderDto>>(orders);

            return Response<List<OrderDto>>.Success(orderDtos, 200);
        }
    }
}
