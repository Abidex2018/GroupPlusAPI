using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Workflow
{
    public class WorkflowOrder
    {
        public WorkflowOrder()
        {
            WorkflowOrderItems = new HashSet<WorkflowOrderItem>();
        }
        public int WorkflowOrderId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title is too short or too long (5 - 100)")]
        public string Title { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStampRegistered { get; set; }

        public ItemStatus Status { get; set; }
        public ICollection<WorkflowOrderItem> WorkflowOrderItems { get; set; }
    }
}
