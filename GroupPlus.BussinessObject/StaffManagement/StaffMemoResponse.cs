using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffMemoResponse")]
    public class StaffMemoResponse
    {
        public int StaffMemoResponseId { get; set; }

        public int StaffMemoId { get; set; }

        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Responese is required")]
        [StringLength(1000, MinimumLength = 8, ErrorMessage = "Memo Responese  must be between 8 and 500 characters")]
        public string MemoResponse { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Register Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Register Date must be between 10 and 35 characters")]
        public string TimeStampRegister { get; set; }

        public int IssuerId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Issuer's Remarks is required")]
        [StringLength(500)]
        public string IssuerRemarks { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Issuer's Remarks Date - Time is required")]
        [StringLength(35)]
        public string IssuerRemarkTimeStamp { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Management's Remarks is required")]
        [StringLength(500)]
        public string ManagementRemarks { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Issuer's Remarks Date - Time is required")]
        [StringLength(35)]
        public string ManagementRemarkTimeStamp { get; set; }

        public int ManagementRemarksBy { get; set; }

      
        

        public virtual StaffMemo StaffMemo { get; set; }
    }
}
