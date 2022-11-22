using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumer
{

    //Mesajı dinelemek için oluşturduğumuz class
    public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
    {
        private readonly OrderDbContext _orderDbContext;
        public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
        {
            var newAddress = new Domain.OrderAggreagate.Address(context.Message.Province, context.Message.District, context.Message.Street, context.Message.ZipCode, context.Message.Line);

            Domain.OrderAggreagate.Order order = new Domain.OrderAggreagate.Order(newAddress, context.Message.BuyerId);

            context.Message.OrderItems.ForEach(item =>
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.PictureUrl, item.Price);
            });

            await _orderDbContext.Orders.AddAsync(order);

            await _orderDbContext.SaveChangesAsync();
        }
    }
}
