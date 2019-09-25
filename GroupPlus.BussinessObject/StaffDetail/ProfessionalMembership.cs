using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BussinessObject.Settings;
using GroupPlus.BussinessObject.StaffManagement;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffDetail
{
    [Table("GPlus.ProfessionalMembership")]
    public class ProfessionalMembership
    {
        public long ProfessionalMembershipId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Staff Information")]
        public int StaffId { get; set; }

        [CheckNumber(0, ErrorMessage = "Invalid Professional Body Information")]
        public int ProfessionalBodyId { get; set; }

        public int YearJoined { get; set; }
       
        public int ProfessionalMembershipTypeId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Time Registered is required")]
        [StringLength(35, MinimumLength = 15, ErrorMessage = "Invalid Time Registered")]
        public string TimeStampRegistered { get; set; }

       
        public virtual ProfessionalMembershipType ProfessionalMembershipType { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
