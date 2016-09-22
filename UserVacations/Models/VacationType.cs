using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class VacationType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public string ColumnRelated { get; set; }
        public decimal Value { get; set; }
        public string Action { get; set; }
        public virtual ICollection<EmployeeVacation> Vacations { get; set; }
        public virtual ICollection<Office> Offices { get; set; }

        public VacationType()
        {
            Offices = new HashSet<Office>();
        }
    }
}