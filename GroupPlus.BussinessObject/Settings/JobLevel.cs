using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.JobLevel")]
    public  class JobLevel
    {
        public JobLevel()
        {
            StaffJobInfos = new HashSet<StaffJobInfo>();
        }
        public int JobLevelId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Level Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Level Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<StaffJobInfo> StaffJobInfos { get; set; }
    }
}
