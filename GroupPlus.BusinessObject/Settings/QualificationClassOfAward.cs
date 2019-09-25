using System.ComponentModel.DataAnnotations.Schema;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.Common;

namespace GroupPlus.BusinessObject.Settings
{
    [Table("GPlus.QualificationClassOfAward")]
    public class QualificationClassOfAward
    {
        public int QualificationClassOfAwardId { get; set; }
        public int QualificationId { get; set; }
        public int ClassOfAwardId { get; set; }
        public ItemStatus Status { get; set; }
        public virtual Qualification Qualification { get; set; }
    }
}
