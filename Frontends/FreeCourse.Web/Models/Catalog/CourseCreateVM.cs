using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog
{
    public class CourseCreateVM
    {
        [Display(Name= "Course Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Price")]
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "Course Description")]
        [Required]
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Picture { get; set; }
        public FeatureVM Feature { get; set; }

        [Display(Name = "Course Category")]
        [Required]
        public string CategoryId { get; set; }

        [Display(Name = "Course Photo")]
        public IFormFile PhotoFormFile { get; set; }

    }
}
