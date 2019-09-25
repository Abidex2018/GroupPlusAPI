using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffLoginActivity")]
    public class StaffLoginActivity
    {
      
        public long StaffLoginActivityId { get; set; }

        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffId { get; set; }
        public bool IsLoggedIn { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string LoginAddress { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(50)]
        public string LoginToken { get; set; }
        
        [Column(TypeName = "varchar")]
        [StringLength(10)]
        public string TokenExpiryDate { get; set; }
        public bool IsTokenExpired { get; set; }

        [Column(TypeName = "varchar")]
        [StringLength(35)]
        public string LoginTimeStamp { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
