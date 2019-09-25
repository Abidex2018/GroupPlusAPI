using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.CompanyManagement
{
    [Table("GPlus.Department")]
    public class Department
    {
        public int DepartmentId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Department Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Department Name  must be between 3 and 200 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
    }
}
