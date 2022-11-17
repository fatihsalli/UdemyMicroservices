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

        //Açıklama 100 karakterden büyük olması durumu için oluşturduk.
        public string ShortDescription
        {
            get => Description.Length > 100 ? Description.Substring(0, 100) + "..." : Description;
        }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public string StockPictureUrl { get; set; }
        public DateTime CreatedTime { get; set; }
        public FeatureVM Feature { get; set; }
        public string CategoryId { get; set; }
        public CategoryVM Category { get; set; }

    }
}
