using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.Settings
{
    [Table("GPlus.State")]
    public class State
    {
        public int StateId { get; set; }

        public int CountryId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "State Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
        public virtual Country Country { get; set; }
    }
}
