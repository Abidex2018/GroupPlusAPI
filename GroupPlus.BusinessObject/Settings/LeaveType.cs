using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.LeaveType")]
    public class LeaveType
    {
        public LeaveType()
        {
            LeaveRequests = new HashSet<LeaveRequest>();
        }
        public int LeaveTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Type is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Leave Type must be between 2 and 50 characters")]
        public string Name { get; set; }

        [CheckNumber(0,ErrorMessage = "Invalid Minimum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Minimum Days is required")]
        public int MinDays { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Maximum Days")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Maximum Days is required")]
        public int MaxDays { get; set; }
        public ItemStatus Status { get; set; }
        public ICollection<LeaveRequest> LeaveRequests { get; set; }
    }
}
