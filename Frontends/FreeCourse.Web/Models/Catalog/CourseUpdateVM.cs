using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FreeCourse.Web.Models.Catalog
{
    public class CourseUpdateVM
    {
        public string Id { get; set; }

        [Display(Name = "Course Name")]
        public string Name { get; set; }

        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Course Description")]
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public FeatureVM Feature { get; set; }

        [Display(Name = "Course Category")]
        public string CategoryId { get; set; }

        [Display(Name = "Course Photo")]
        public IFormFile PhotoFormFile { get; set; }

    }
}
