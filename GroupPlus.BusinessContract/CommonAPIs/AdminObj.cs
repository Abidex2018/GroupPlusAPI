using System.ComponentModel.DataAnnotations;
using GroupPlus.Common;

namespace GroupPlus.BusinessContract.CommonAPIs
{
    public class AdminObj
    {
        [CheckNumber(0, ErrorMessage ="User Id is required")]
        public int AdminUserId { get; set; }

        [Required(ErrorMessage = "System Code is required", AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 15, ErrorMessage = "Invalid System Code")]
        public string SysPathCode;

        [StringLength(50)]
        public string LoginIP;
    }
}
