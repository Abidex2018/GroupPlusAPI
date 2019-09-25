using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffDetail
{
    [Table("GPlus.EmergencyContact")]
    public class EmergencyContact
    {
        public int EmergencyContactId { get; set; }
        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Last Name  must be between 3 and 200 characters")]
        public string LastName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "First Name  must be between 3 and 200 characters")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true)]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Middle Name  must be between 3 and 200 characters")]
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

        public bool IsDefault { get; set; }

        public int StateOfLocationId { get; set; }
        public int LocalAreaOfLocationId { get; set; }

       public virtual Staff Staff { get; set; }
    }
}
