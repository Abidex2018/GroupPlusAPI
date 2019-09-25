using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.JobSpecialization")]
    public class JobSpecialization
    {
        public JobSpecialization()
        {
            StaffJobInfos = new HashSet<StaffJobInfo>();
        }
        public int JobSpecializationId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Specialization Name is required")]
        [StringLength(350, MinimumLength = 2, ErrorMessage = "Specialization Name must be between 2 and 350 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<StaffJobInfo> StaffJobInfos { get; set; }
    }
}
