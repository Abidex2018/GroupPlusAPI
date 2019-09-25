using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.SalaryLevel")]
    public class SalaryLevel
    {
        public SalaryLevel()
        {
            StaffJobInfos = new HashSet<StaffJobInfo>();
        }
        public int SalaryLevelId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Salary Level is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Salary Level must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<StaffJobInfo> StaffJobInfos { get; set; }
    }
}
