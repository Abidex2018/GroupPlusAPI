using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.Settings;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffPension")]
    public class StaffPension
    {
        public int StaffPensionId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pension Number is required")]
        [StringLength(15, ErrorMessage = "Pension Number must not exceed 15 characters")]
        [Index("IX_PenKey", IsUnique = true)]
        public string PensionNumber { get; set; }

        public decimal CompanyContribution { get; set; }
        
        public decimal PersonalContribution { get; set; }

        public int PensionAdministratorId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Register Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Register Date must be between 10 and 35 characters")]
        public string TimeStampRegister { get; set; }

        public virtual PensionAdministrator PensionAdministrator { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
