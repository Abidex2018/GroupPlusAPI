using System.Collections.Generic;
using GroupPlus.Common;

namespace GroupPlus.Business.Core.ItemComparers
{
    public class AddressComparer : IEqualityComparer<AddressHelper>

    {
        public bool Equals(AddressHelper x, AddressHelper y)

        {
            if (x == null || y == null) return true;

            return x.Address.ToLower().Trim().GetHashCode() == y.Address.ToLower().Trim().GetHashCode();
        }


        public int GetHashCode(AddressHelper x)

        {
            return x.Address.GetHashCode();
        }
    }

    public class PensionNumberComparer : IEqualityComparer<PensionNumberHelper>

    {
        public bool Equals(PensionNumberHelper x, PensionNumberHelper y)

        {
            if (x == null || y == null) return true;

            return x.PensionNumber.ToLower().Trim().GetHashCode() == y.PensionNumber.ToLower().Trim().GetHashCode();
        }


        public int GetHashCode(PensionNumberHelper x)

        {
            return x.PensionNumber.GetHashCode();
        }
    }

    public class InsuaranceComparer : IEqualityComparer<InsuranceHelper>

    {
        public bool Equals(InsuranceHelper x, InsuranceHelper y)

        {
            if (x == null || y == null) return true;

            return x.PolicyNumber.ToLower().Trim().GetHashCode() == y.PolicyNumber.ToLower().Trim().GetHashCode();
        }


        public int GetHashCode(InsuranceHelper x)

        {
            return x.PolicyNumber.GetHashCode();
        }
    }

    public class MobileComparer : IEqualityComparer<MobileNumberHelper>

    {
        public bool Equals(MobileNumberHelper x, MobileNumberHelper y)

        {
            if (x == null || y == null) return true;

            return x.MobileNumber.ToLower().Trim().GetHashCode() == y.MobileNumber.ToLower().Trim().GetHashCode();
        }


        public int GetHashCode(MobileNumberHelper x)

        {
            return x.MobileNumber.GetHashCode();
        }
    }


    public class BankAccountComparer : IEqualityComparer<BankAccountHelper>

    {
        public bool Equals(BankAccountHelper x, BankAccountHelper y)

        {
            if (x == null || y == null) return true;

            return x.BankAccountNumber.ToLower().Trim().GetHashCode() ==
                   y.BankAccountNumber.ToLower().Trim().GetHashCode();
        }


        public int GetHashCode(BankAccountHelper x)

        {
            return x.Id.GetHashCode();
        }
    }
}