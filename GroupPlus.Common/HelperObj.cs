namespace GroupPlus.Common
{
    public class AddressHelper
    {
        public int Id { get; set; }
        public string Address { get; set; }
    }

    public class PensionNumberHelper
    {
        public int Id { get; set; }
        public string PensionNumber { get; set; }
    }
    public class InsuranceHelper
    {
        public int Id { get; set; }
        public string PolicyNumber { get; set; }
    }
    public class MobileNumberHelper
    {
        public int Id { get; set; }
        public string MobileNumber { get; set; }
    }

   
    public class BankAccountHelper
    {
        public int Id { get; set; }
        public string BankAccountNumber { get; set; }

    }

    public class LeaveRequestHelper
    {
        public int Id { get; set; }
        public int LeaveType { get; set; }

        public string ProposedStartDate { get; set; }

        public string ProposedEndDate { get; set; }
    }

}
