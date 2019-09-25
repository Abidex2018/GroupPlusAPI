using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.JobPosition")]
    public class JobPosition
    {
        public JobPosition()
        {
            StaffJobInfos = new HashSet<StaffJobInfo>();
        }

        public int JobPositionId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Job Position is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Job Position must be between 2 and 100 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<StaffJobInfo> StaffJobInfos { get; set; }
    }
}
