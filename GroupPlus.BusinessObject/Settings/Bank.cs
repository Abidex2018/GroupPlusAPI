using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.Bank")]
    public class Bank
    {
        public int BankId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bank Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Bank Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }
    }
}
