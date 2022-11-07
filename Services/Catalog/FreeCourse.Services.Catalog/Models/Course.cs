using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace FreeCourse.Services.Catalog.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        //MongoDb tarafında değerin tipini belirlemek için "BsonRepresentation(BsonType.Decimal128)" ifadesini kullandık.
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        //MongoDb tarafında string i direkt olarak kendi tanıyacak. BsonRepresentation ile tanımlama yapmamıza gerek yoktur.
        public string Description { get; set; }

        //Identity kütüphanesinde UserId değerini Guid olarak tutacağımız için string yazdık.
        public string UserId { get; set; }
        public string Picture { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }

        //MongoDb tarafında Feature ı direkt olarak Course içinde tutulacak şekilde bağlıyoruz.
        public Feature Feature { get; set; }

        //Category Id tarafında [BsonRepresentation(BsonType.ObjectId)] bu değer ile tanımladığımız için burada da belirtmeliyiz.
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        //MongoDb tarafındaki collectionlara yansıtırken bunu göz ardı et. Biz bunu program içerisinde kullanmak için tanımladık.
        [BsonIgnore]
        public Category Category { get; set; }

    }
}
