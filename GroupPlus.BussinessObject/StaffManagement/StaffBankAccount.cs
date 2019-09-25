using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffBankAccount")]
    public class StaffBankAccount
    {
        public int StaffBankAccountId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Bank Id is required")]
        public int BankId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Name is required")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Account Name must be between 5 and 100 characters")]
        public string AccountName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Account Number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Account Number")]
        [Index("UQ_Acc_No", IsUnique = true)]
        public string AccountNumber { get; set; }

        public bool IsDefault { get; set; }

        public ItemStatus Status { get; set; }

        public  virtual  Staff Staff { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStamRegistered { get; set; }

    }
}
