using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserVacations.Models
{
    public class AssignedOfficeData
    {
        public int VacationTypeId { get; set; }
        public string VacationTypeName { get; set; }
        public bool Assigned { get; set; }
    }
}