using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class EmployeeVacation
    {
        public int Id { get; set; }
        public DateTime VacationFrom { get; set; }
        public DateTime VacationTo { get; set; }
        public string StatusVacation { get; set; }
        public int VacationYearTaken { get; set; }
        public string VacationNote { get; set; }
        public int UserID { get; set; }
        public int TypeId { get; set; }

        public virtual MyAppUser User { get; set; }
        public virtual VacationType VacationType { get; set; }

    }
}