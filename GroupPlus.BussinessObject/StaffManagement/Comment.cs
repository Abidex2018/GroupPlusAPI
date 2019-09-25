using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.Common;

namespace GroupPlus.BussinessObject.StaffManagement
{
    public class Comment
    {
       

        public int CommentId { get; set; }
        public int StaffId { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment Details is required")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Comment Details must be between 2 and 500 characters")]
        public string CommentDetails { get; set; }

        [Column(TypeName = "varchar")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Comment Date - Time is required")]
        [StringLength(35, MinimumLength = 10, ErrorMessage = "Comment Date must be between 10 and 35 characters")]
        public string TimeStampCommented { get; set; }
        public CommentType CommentType { get; set; }
      public  virtual Staff Staff { get; set; }
    }
}
