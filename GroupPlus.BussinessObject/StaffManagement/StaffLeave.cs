using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

// ReSharper disable InconsistentNaming

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffLeave")]
    public class StaffLeave
    {
        public long StaffLeaveId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Leave Request Id is required")]
        public long LeaveRequestId { get; set; }

        public int LeaveTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Request Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Request Date must be between 10 and 35 characters")]
        public string TimeStampRequested { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed Start Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Proposed Start Date must be between 10 and 35 characters")]
        public string ProposedStartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Proposed Start Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Proposed Start Date must be between 10 and 35 characters")]
        public string ProposedEndDate { get; set; }

        public int HODApprovedBy { get; set; }

        public int LMApprovedBy { get; set; }

        public int HRApprovedBy { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Comment is required")]
        [StringLength(500)]
        public string HODComment { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Comment is required")]
        [StringLength(500)]
        public string LMComment { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "HR's Comment is required")]
        [StringLength(500)]
        public string HRComment { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved Start Date is required")]
        [StringLength(10)]
        public string HODApprovedStartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Approved End Date is required")]
        [StringLength(10)]
        public string HODApprovedEndDate { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved Start Date is required")]
        [StringLength(10)]
        public string LMApprovedStartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Approved End Date is required")]
        [StringLength(10)]
        public string LMApprovedEndDate { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved Start Date is required")]
        [StringLength(10)]
        public string HRApprovedStartDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Approved End Date is required")]
        [StringLength(10)]
        public string HRApprovedEndDate { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Line Manager's Date - Time Approved is required")]
        [StringLength(35)]
        public string LMApprovedTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HOD's Date - Time Approved is required")]
        [StringLength(35)]
        public string HODApprovedTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "HR's Date - Time Approved is required")]
        [StringLength(35)]
        public string HRApprovedTimeStamp { get; set; }

        public LeaveStatus Status { get; set; }
        public virtual LeaveRequest LeaveRequest { get; set; }
    }
}
