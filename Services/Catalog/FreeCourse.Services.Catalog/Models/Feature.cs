namespace FreeCourse.Services.Catalog.Models
{
    public class Feature
    {
        //MongoDb tarafında -Bire bir ilişki için; Embedded Document Pattern kullandığımız için Id alanına gerek yoktur. Course içinde tutacağız.
        public int Duration { get; set; }


    }
}
