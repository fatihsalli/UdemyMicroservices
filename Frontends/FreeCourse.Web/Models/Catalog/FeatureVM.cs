﻿using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalog
{
    public class FeatureVM
    {
        [Display(Name = "Kurs Süre")]
        [Required]
        public int Duration { get; set; }

    }
}
