using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    public class Category
    {
        //MongoDb tarafında ID olarak alınması için "[BsonId]" ifadesini ekliyoruz.
        //MongoDb tarafında string olarak verilen değer BsonRepresentation(ObjectId) ile objectId'ye çevrilecek.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }




    }
}
