namespace FreeCourse.Web.Models.Catalog
{
    public class CourseCreateVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public FeatureVM Feature { get; set; }
        public string CategoryId { get; set; }

    }
}
