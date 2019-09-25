using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.Settings;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{

    [Table("GPlus.StaffNextOfKin")]
    public class StaffNextOfKin
    {
        public int StaffNextOfKinId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }

        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 50 characters")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(2050, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 50 characters")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 50 characters")]
        public string MiddleName { get; set; }
        
        public Gender Gender { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string MobileNumber { get; set; }


        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Next of Kin Email Address is required")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        public NextOfKinRelationship Relationship { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Number is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Mobile Number  must be between 3 and 200 characters")]
        public string Landphone { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Register Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Register Date must be between 10 and 35 characters")]
        public string TimeStampRegister { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
