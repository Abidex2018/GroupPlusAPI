using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.SalaryGrade")]
    public class SalaryGrade
    {
        public SalaryGrade()
        {
            StaffJobInfoes = new HashSet<StaffJobInfo>();
        }
        public int SalaryGradeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Grade is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Grade must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<StaffJobInfo> StaffJobInfoes { get; set; }
    }
}
