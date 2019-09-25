// ReSharper disable InconsistentNaming
namespace GroupPlus.Common
{
    public enum ItemStatus { Deleted = -100, Inactive = 0, Active}

    public enum StaffStatus { Deleted = -100,Inactive=0, Active = 1, Suspended, Terminated }
    public enum ApprovalStatus { Deleted = -100, Pending = 0,Registered, Approved ,Denied}
    public enum LeaveStatus { Deleted = -100, Registered = 1, LM_Approved, HOD_Approved, LM_Denied, HOD_Denied, Approved, Denied, Started, Completed }


    public enum LeaveRequestStatus { Deleted = -100, Registering = 1, Registered }

    public enum CompanyType { Regular=1, Group, Subsidiary}

    public enum BloodGroup { A = 1, AB, O, O_Positive, O_Negative }
    public enum Genotype { AA = 1, AS, SS }
  

    public enum EmploymentType { Contract = 1, Permanent, Intern }
    public enum NextOfKinRelationship { Father = 1, Mother, Wife, Husband, Sibling, Child, Uncle, Aunt, Cousing, Nephew, Nice, Friend, Others }

    public enum MaritalStatus { Single = 1, Married , Divorce }
    public enum Currency { Naira = 1, Dollar}

    public enum Month
    {
        Jan = 1, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
    }
    public enum MonthOfCert
    {
        January = 1, February, March, April, May, June, July, August, September, October, November, December
    }

    public enum MemoType { Query = 1, Instruction, Warning }
    public enum CommentType { Satisfactory = 1, Incomplete_Profile, Warning }

    public enum Gender { Male = 1, Female }

    public enum WorkflowItem { Staff_Leave = 1, Staff_Appraisal, Staff_Promotion, Staff_Memo, Staff_Loan }
    public enum WorkflowInitiatorType { Staff = 1, Line_Manager, HR, }
    public enum WorkflowStatus { Stopped =0, Initiated = 1, Approval_In_Progress, On_Hold, Completed }
    public enum WorkflowApprovalType { Line_Manager = 1, Team_Lead, Unit_Head, Divisional_Head, Account_Head, IT_Head, HR_Head }
}
