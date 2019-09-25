using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.StaffManagement
{
    [Table("GPlus.StaffAccessRole")]
    public class StaffAccessRole
    {
        public int StaffAccessRoleId { get; set; }
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffAccessId { get; set; }
        
        [CheckNumber(0, ErrorMessage = "Staff Id is required")]
        public int StaffRoleId { get; set; }

        public  virtual  StaffRole StaffRole { get; set; }

        public  virtual  StaffAccess StaffAccess { get; set; }
    }
}
