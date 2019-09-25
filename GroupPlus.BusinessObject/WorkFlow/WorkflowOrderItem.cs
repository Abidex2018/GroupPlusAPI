using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Workflow
{
    public class WorkflowOrderItem
    {
        public WorkflowOrderItem()
        {
            WorkflowLogs = new List<WorkflowLog>();
        }

        public int WorkflowOrderItemId { get; set; }

        public int WorkflowOrderId { get; set; }

        public int Order { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name is too short or too long (5 - 100)")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public virtual WorkflowOrder WorkflowOrder { get; set; }
        public ICollection<WorkflowLog> WorkflowLogs { get; set; }
    }
}
