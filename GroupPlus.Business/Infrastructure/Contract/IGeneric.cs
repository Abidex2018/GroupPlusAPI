using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupPlus.Business.Infrastructure.Contract
{
    internal interface IReaderRepository<T> where T : class
    {
       T bindReaderToObj(DbDataReader obj);
    }
    //private static NYSCDetailObj bindReaderToObj(DbDataReader dr)
    //{
    //try
    //{
    //    var item = new NYSCDetailObj
    //    {
    //        NYSCDetailId = dr.GetFieldValue<int>(dr.GetOrdinal("NYSCDetailId")),
    //        Status = dr.GetFieldValue<int>(dr.GetOrdinal("Status")),
    //        ApplicantId = dr.GetFieldValue<int>(dr.GetOrdinal("ApplicantId")),
    //        StateOfServiceId = dr.GetFieldValue<int>(dr.GetOrdinal("StateOfServiceId")),
    //        YearOfService = dr.GetFieldValue<int>(dr.GetOrdinal("YearOfService")),
    //        ExemptionDetail = dr.IsDBNull(dr.GetOrdinal("ExemptionDetail")) ? "" : dr.GetFieldValue<string>(dr.GetOrdinal("ExemptionDetail")),
    //        DischargeCertificateNo = dr.IsDBNull(dr.GetOrdinal("DischargeCertificateNo")) ? "" : dr.GetFieldValue<string>(dr.GetOrdinal("DischargeCertificateNo")),
    //        TimeStampRegistered = dr.IsDBNull(dr.GetOrdinal("TimeStampRegistered")) ? "" : dr.GetFieldValue<string>(dr.GetOrdinal("TimeStampRegistered")),
    //        TimeStampLastEdited = dr.IsDBNull(dr.GetOrdinal("TimeStampLastEdited")) ? "" : dr.GetFieldValue<string>(dr.GetOrdinal("TimeStampLastEdited")),
    //    };
    //    item.StateOfServiceName = item.StateOfServiceId == (int)OtherSelected.NotApplicable ? "Not Applicable" : StateRepository.Instance().getStateInfo(item.StateOfServiceId).Name;
    //    item.YearLabel = item.YearOfService == (int)OtherSelected.NotApplicable ? "Not Applicable" : item.YearOfService.ToString();
    //    item.StatusLabel = ((NYSCStatus)item.Status).ToUtilString();
    //    return item;
    //}
    //catch (Exception ex)
    //{
    //    UtilTools.LogE(ex.StackTrace, ex.Source, ex.GetBaseException().Message);
    //    return new NYSCDetailObj();
    //}
    //}
}
