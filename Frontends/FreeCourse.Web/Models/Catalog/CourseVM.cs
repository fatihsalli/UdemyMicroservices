using System;

namespace FreeCourse.Web.Models.Catalog
{
    //Catalog.Api içerisinden CourseDto daki propertylerin aynısını aldık.
    public class CourseVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public DateTime CreatedTime { get; set; }
        public FeatureVM Feature { get; set; }
        public string CategoryId { get; set; }
        public CategoryVM Category { get; set; }

    }
}
