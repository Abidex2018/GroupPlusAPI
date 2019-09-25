using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.ProfessionalMembershipType")]
    public class ProfessionalMembershipType
    {
        public ProfessionalMembershipType()
        {
            ProfessionalMemberships = new HashSet<ProfessionalMembership>();
        }
        public int ProfessionalMembershipTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Professional Membership Type Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Professional Membership Type Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        public ItemStatus Status { get; set; }

        public ICollection<ProfessionalMembership> ProfessionalMemberships { get; set; }
    }
}
