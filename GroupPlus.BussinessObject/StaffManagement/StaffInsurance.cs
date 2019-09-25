using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.Settings;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    [Table("GPlus.StaffInsurance")]
    public  class StaffInsurance
    {
        public int StaffInsuranceId { get; set; }

        public int InsurancePolicyTypeId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Policy Number is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Policy Number  must be between 3 and 200 characters")]
        [Index("UQ_Ins_No", IsUnique = true)]
        public string PolicyNumber { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Insurer's Name is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Insurer's Name  must be between 2 and 200 characters")]
        public string Insurer { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Commencement Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Commencement Date")]
        public string CommencementDate { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Termination Date is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Invalid Termination Date")]
        public string TerminationDate { get; set; }
        public decimal PersonalContibution { get; set; }
        public decimal CompanyContibution { get; set; }
    
        public ItemStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStamRegistered { get; set; }

        public virtual InsurancePolicyType InsurancePolicyType { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
