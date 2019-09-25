using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.JobType")]
    public class JobType
    {
        public JobType()
        {
            StaffJobInfos = new HashSet<StaffJobInfo>();
        }
        public int JobTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Job Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<StaffJobInfo> StaffJobInfos { get; set; }
    }
}
