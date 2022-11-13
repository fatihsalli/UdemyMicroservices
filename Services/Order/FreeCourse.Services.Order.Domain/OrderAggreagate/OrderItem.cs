using FreeCourse.Services.Order.Domain.Core;
using System;

namespace FreeCourse.Services.Order.Domain.OrderAggreagate
{
    //OrderItem Order classına ait olduğu için navigation property tanımlamıyoruz.
    public class OrderItem : Entity
    {
        //CourseId de yazabilirdik.
        public string ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string PictureUrl { get; private set; }
        public Decimal Price { get; private set; }

        //Domain Driven Design da bu şekilde tanımlamıyoruz. Bu classa Order üzerinden erişiyoruz. Bunun sebebi Order dışında bu class eklenmemesi içindir. Biz OrderId yazmasak da EF Core 1'e çok ilişki tanımlamak için bu property'i database tarafında oluşturacak. Buna "Shadow Property" denir. Database tarafında sütun olarak yer alıp entity tarafında karşılığı olmayan propertylerdir.
        //public int OrderId { get; set; }

        public OrderItem()
        {

        }

        //Sadece bu class üzerinden set edilebilmesi için private set yaptık.
        public OrderItem(string productId, string productName, string pictureUrl, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }

        //Domain Driven Desing da business kodu da burada yazıyoruz. Bizim normal monolithic den farklı olarak. Dışarıdan update edebilmek için bu metotu yazdık.
        public void UpdateOrderItem(string productName, string pictureUrl, decimal price)
        {
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }


    }
}
