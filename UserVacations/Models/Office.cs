using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class Office
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<VacationType> VacationTypes { get; set; }

        public Office()
        {
            VacationTypes = new HashSet<VacationType>();
        }
    }
}