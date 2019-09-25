using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Workflow
{
    public class WorkflowSetup
    {
        public int WorkflowSetupId { get; set; }
        public WorkflowItem Item { get; set; }
        public int StaffId { get; set; }
        public int InitiatorId { get; set; }
        public WorkflowInitiatorType InitiatorType { get; set; }

        public int WorkflowOrderId { get; set; }
        public long WorkflowSourceId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description is required")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Description is too short or too long (5 - 150)")]
        public string Description { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStampInitiated { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time modified is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time modified must be between 10 and 35 characters")]
        public string LastTimeStampModified { get; set; }

        public WorkflowStatus Status { get; set; }
    }
}
