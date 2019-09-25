using System.ComponentModel.DataAnnotations.Schema;
using XPLUG.WEBTOOLS;

namespace GroupPlus.BusinessObject.CompanyManagement
{
    [Table("GPlus.SerialNumberKeeper")]
    public class SerialNumberKeeper
    {
        public long SerialNumberKeeperId { get; set; }

        public string RegisteredTimeStamp => DateMap.CurrentTimeStamp();
    }
}
