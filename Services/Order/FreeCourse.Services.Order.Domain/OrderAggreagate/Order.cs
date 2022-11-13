using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Order.Domain.OrderAggreagate
{
    //Entity ve AggreageteRoot (Yönetimi order üzerinden gerçekleştireceğimizi belirttik) olacak. 
    //EF Core Features
    // --Owned Types
    // --Shadow Property
    // --Backing Field
    public class Order : Entity, IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }

        //"Owned Entity Type" biz bunu tanımladığımızda EF Core'a müdahale etmez isek Order içinde Address ile ilgili sütunları oluşturur.
        public Address Address { get; private set; }

        //UserId gönderilecek.
        public string BuyerId { get; private set; }

        //Backing fields. Order üzerinden kontrolsüz şekilde kimse orderItems a data eklememesi için oluşturduk. EF core dolduracak bir alt satırda da readonly olarak dış dünyaya açacağız.
        private readonly List<OrderItem> _orderItems;

        //Kapsülleme işlemi yaptık. Sadece okunması için.
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        //Aşağıda overload ettiğimiz için boş olarak da gösteriyoruz. Migration yaparken hata aldık.
        public Order()
        {

        }

        public Order(Address address, string buyerId)
        {
            _orderItems = new List<OrderItem>();
            CreatedDate = DateTime.Now;
            Address = address;
            BuyerId = buyerId;
        }

        public void AddOrderItem(string productId, string productName, string pictureUrl, decimal price)
        {
            //Bu productId den var mı yokmu kontrol ediyoruz. Course olduğu için bir defa alınır.
            var existProduct = _orderItems.Any(x => x.ProductId == productId);

            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        //Toplam için
        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);




    }
}
