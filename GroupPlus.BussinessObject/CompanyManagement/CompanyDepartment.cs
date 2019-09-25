using System.ComponentModel.DataAnnotations.Schema;

namespace GroupPlus.BussinessObject.CompanyManagement
{
    [Table("GPlus.CompanyDepartment")]
    public class CompanyDepartment
    {
       
        public int CompanyDepartmentId { get; set; }

        public int CompanyId { get; set; }

        public  int DepartmentId { get; set; }
 
        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
    }
}
