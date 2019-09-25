using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.TerminationReason")]
    public class TerminationReason
    {
        
        public int TerminationReasonId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Reason Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Termination Reason Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
    }
}
