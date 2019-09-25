using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffMemo")]
    public class StaffMemo
    {
        public StaffMemo()
        {
            StaffMemoResponses = new HashSet<StaffMemoResponse>();
        }
        public int StaffMemoId { get; set; }

        public int StaffId { get; set; }

        public MemoType MemoType { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Leave Purpose is required")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Leave Purpose  must be between 5 and 200 characters")]
        public string Title { get; set; }

        public int RegisterBy { get; set; }

        public int ApprovedBy { get; set; }

        [Column(TypeName = "text")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Memo Detail is required")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "Memo Detail  must be between 20 and 2000 characters")]
        public string MemoDetail { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Register Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Register Date must be between 10 and 35 characters")]
        public string TimeStampRegister { get; set; }

        public bool IsReplied { get; set; }
        public ApprovalStatus Status { get; set; }

        public virtual Staff Staff { get; set; }

        public ICollection<StaffMemoResponse> StaffMemoResponses { get; set; }
    }
}
