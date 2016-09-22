using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class VacationConfiguration
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int VacationYear { get; set; }
        public decimal VacationDays { get; set; } //Total vacation days for the year
        public decimal RemainingDays { get; set; }
        public int EarnedDays { get; set; }
        public MyAppUser User { get; set; }
    }
}