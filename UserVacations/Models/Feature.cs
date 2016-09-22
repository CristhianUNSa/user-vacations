using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class Feature
    {
        public int Id { get; set; }
        public string FeatureName { get; set; }
        [DisplayName("Specification Link")]
        public string SpecificationLink { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}