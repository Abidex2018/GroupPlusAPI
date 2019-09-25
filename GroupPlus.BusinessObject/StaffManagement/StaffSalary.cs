using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;


namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffSalary")]
    public class StaffSalary
    {
        public int StaffSalaryId { get; set; }
        public int StaffId { get; set; }
        public int StaffJobInfoId { get; set; }
        public Currency Currency { get; set; }
        public decimal BasicAllowance { get; set; }
        public decimal HousingAllowance { get; set; }
        public decimal EducationAllowance { get; set; }
        public decimal FurnitureAllowance { get; set; }
        public decimal WardrobeAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal LeaveAllowance { get; set; }
        public decimal EntertainmentAllowance { get; set; }
        
        public decimal PensionDeduction { get; set; }
        public decimal PayeDeduction { get; set; }
        public decimal InsuranceDeduction { get; set; }

        public decimal TotalPayment { get; set; }
        public decimal TotalDeduction { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStamRegistered { get; set; }

        public ItemStatus Status { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
