using System;

namespace FreeCourse.Services.Catalog.Dtos
{
    public class CourseCreateDto
    {
        //Client'ların Id göndermesine gerek yok. Id propertysini kaldırdık.
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        //CreatedTime propertysini de kaldırdık. Kendimiz oluşturmak için.
        public FeatureDto Feature { get; set; }
        public string CategoryId { get; set; }
        //CategoryDto olarak değiştirdik.
        //public CategoryDto Category { get; set; } bunu da kaldırdık.

    }
}
