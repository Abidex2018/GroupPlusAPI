using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.LeaveRequest")]
    public class LeaveRequest
    {
        public long LeaveRequestId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }


        [CheckNumber(0, ErrorMessage = "Department Id is required")]
        public int DepartmentId { get; set; }

        [CheckNumber(0, ErrorMessage = "Company Id is required")]
        public int CompanyId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Title is required")]
        [StringLength(300, MinimumLength = 8, ErrorMessage = "Leave Title  must be between 8 and 300 characters")]
        public string LeaveTitle { get; set; }

        public int LeaveType { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Registration Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Registration Date must be between 10 and 35 characters")]
        public string TimeStampRegistered { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed Start Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed Start Date")]
        public string ProposedStartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed End Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Proposed End Date")]
        public string ProposedEndDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Purpose of leave is too short or too long")]
        public string Purpose { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(300)]
        public string OtherRemarks { get; set; }

        public LeaveRequestStatus Status { get; set; }

        public virtual Staff Staff { get; set; }
       
    }
}
