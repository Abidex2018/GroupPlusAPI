using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Workflow
{
   public class WorkflowLog
    {
        public long WorkflowLogId { get; set; }

        [CheckNumber(0, ErrorMessage = "Workflow Setup is required")]
        public int WorkflowSetupId { get; set; }
        public WorkflowApprovalType ApprovalType { get; set; }

        [CheckNumber(0, ErrorMessage = "Target Staff is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Workflow Processor is required")]
        public int ProcessorId { get; set; }
       
        [CheckNumber(0, ErrorMessage = "Workflow Order Item is required")]
        public int WorkflowOrderItemId { get; set; }
        public ApprovalStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "comment is required")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "comment is too short or too long (5 - 500)")]
        public string Comment { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string LogTimeStamp { get; set; }

        public virtual WorkflowOrderItem WorkflowOrderItem { get; set; }
    }
}
