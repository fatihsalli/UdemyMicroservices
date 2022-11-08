namespace FreeCourse.Services.Catalog.Dtos
{
    public class CourseUpdateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        //Update esnasında yeni resim geleceği için ekledik.
        public string Picture { get; set; }
        //CreatedTime propertysini de kaldırdık. Kendimiz oluşturmak için.
        public FeatureDto Feature { get; set; }
        public string CategoryId { get; set; }
        //CategoryDto olarak değiştirdik.
        //public CategoryDto Category { get; set; } bunu da kaldırdık.


    }
}
