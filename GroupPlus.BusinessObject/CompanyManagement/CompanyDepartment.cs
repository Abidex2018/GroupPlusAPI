using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPlus.BusinessObject.CompanyManagement
{
    [Table("GPlus.CompanyDepartment")]
    public class CompanyDepartment
    {

        public int CompanyDepartmentId { get; set; }

        public int CompanyId { get; set; }

        public int DepartmentId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
    }
}
