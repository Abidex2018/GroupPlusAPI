using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffContact")]
    public class StaffContact
    {
        public int StaffContactId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff is required")]
        public int StaffId { get; set; }
        
        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Residential Address is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Residential Address  must be between 3 and 200 characters")]
        public string ResidentialAddress { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Town of Residence is required")]
        [StringLength(200)]
        public string TownOfResidence { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = true, ErrorMessage = "Location Landmark is required")]
        [StringLength(200)]
        public string LocationLandmark { get; set; }

        [CheckNumber(0, ErrorMessage = "State Of Origin Id  is required")]
        public int StateOfResidenceId { get; set; }

        [CheckNumber(0, ErrorMessage = "LGA Of Origin Id is required")]
        public int LocalAreaOfResidenceId { get; set; }

        public bool IsDefault { get; set; }

        public ItemStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date - Time registered is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Date  Time registered must be between 10 and 35 characters")]
        public string TimeStamRegistered { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
