using System;
using System.Linq;
using System.Threading;
using GroupPlus.Business.Infrastructure;
using GroupPlus.Business.Infrastructure.Contract;
using GroupPlus.Business.Repository.Common;
using GroupPlus.BusinessObject.Settings;
using XPLUG.WEBTOOLS;
using XPLUG.WEBTOOLS.Security;

namespace GroupPlus.Business.Core
{
    internal class IdGenerator
    {
// ReSharper disable InconsistentNaming
        private static IPlugRepository<Bank> _supRepository;

        private static PlugUoWork _uoWork;
        //private static string _reqPrefix = "REQ-";
        //private static string _retPrefix = "RET-";
        //private static string _supPrefix = "SUP-";

        private static void Init()
        {
            _uoWork = new PlugUoWork( /*You can specify you custom context here*/);
            _supRepository = new PlugRepository<Bank>(_uoWork);
        }


        internal static string gen(string firstName, string mobileNo, string confirmCode, out string msg)
        {
            try
            {
                var serialId = SerialNumberKeeperRepository.Generate();
                if (serialId < 1)
                {
                    msg = "Unable to generate customer key";
                    return "";
                }
                var keyHash = serialId + "_" + firstName.Replace(" ", "") + "_" + mobileNo.Trim().Replace(" ", "") +
                              "_" + confirmCode;
                var keyCode = UniqueHashing.GetStandardHash(keyHash);
                if (keyCode.ToString().Length < 8)
                    keyCode = UniqueHashing.GetStandardHash(keyHash + "" + Environment.TickCount);
                if (keyCode.ToString().Length < 8)
                {
                    msg = "Unable to generate customer key";
                    return "";
                }
                msg = "";
                return keyCode.ToString().Substring(0, 8);
            }
            catch (Exception ex)
            {
                msg = ex.GetBaseException().Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return "";
            }
        }

        internal static string RandCodeGen(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static string RandCodeGenMixed(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjklmnpq0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        internal static string OTPCodeGen(int size)
        {
            try
            {
                if (size > 8)
                    return "";
                var thisTick = Math.Abs(Environment.TickCount).ToString();
                if (thisTick.Length < 10)
                {
                    Thread.Sleep(50);
                    var thisTick2 = Math.Abs(Environment.TickCount).ToString();
                    thisTick = thisTick + "" + thisTick2;
                }

                var rnx = new Random();
                var rndId = rnx.Next(1, 6);

                switch (size)
                {
                    case 4:
                        return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2);
                    case 5:
                        return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2) + "" + thisTick.Substring(0, 1);
                    case 6:
                        switch (rndId)
                        {
                            case 1:
                                return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2) + "" +
                                       thisTick.Substring(7, 1) + "" + thisTick.Substring(1, 1);
                            case 2:
                                return thisTick.Substring(7, 2) + "" + thisTick.Substring(4, 2) + "" +
                                       thisTick.Substring(0, 1) + "" + thisTick.Substring(3, 1);
                            case 3:
                                return thisTick.Substring(5, 2) + "" + thisTick.Substring(2, 1) + "" +
                                       thisTick.Substring(8, 1) + "" + thisTick.Substring(0, 2);
                            case 4:
                                return thisTick.Substring(9, 1) + "" + thisTick.Substring(6, 2) + "" +
                                       thisTick.Substring(2, 2) + "" + thisTick.Substring(0, 1);
                            case 5:
                                return thisTick.Substring(7, 2) + "" + thisTick.Substring(4, 2) + "" +
                                       thisTick.Substring(0, 2);
                            case 6:
                                return thisTick.Substring(0, 2) + "" + thisTick.Substring(7, 2) + "" +
                                       thisTick.Substring(4, 1) + "" + thisTick.Substring(9, 1);
                            default:
                                return thisTick.Substring(3, 3) + "" + thisTick.Substring(8, 1) + "" +
                                       thisTick.Substring(0, 2);
                        }

                    case 7:
                        return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2) + "" +
                               thisTick.Substring(7, 1) + "" + thisTick.Substring(0, 2);
                    case 8:
                        return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2) + "" +
                               thisTick.Substring(6, 2) + "" + thisTick.Substring(0, 2);
                    default:
                        return thisTick.Substring(8, 2) + "" + thisTick.Substring(3, 2) + "" +
                               thisTick.Substring(7, 1) + "" + thisTick.Substring(1, 1);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return "";
            }
        }

        #region Generate Requisition Number

        //internal static string generateRequisitionNumber(out string msg)
        //{
        //    Init();
        //    try
        //    {
        //        var lastRefNo = getLastSalesRequisitionNumber(out msg);
        //        if (lastRefNo == null)
        //        {
        //            return "";
        //        }

        //        if (lastRefNo == "")
        //        {
        //            return _reqPrefix + "0000000001";
        //        }

        //        var retRef = lastRefNo.Replace(_reqPrefix, "");
        //        if (string.IsNullOrEmpty(retRef))
        //        {
        //            msg = "Unable to generate Requisition Number";
        //            return "";
        //        }
        //        // ErrorManager.LogApplicationError("Raw Val", retRef, retRef);
        //        var newRef = _reqPrefix + (int.Parse(retRef) + 1).ToString("D9");
        //        return newRef;
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = "Unable to generate Requisition Number due to error";
        //        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
        //        return "";
        //    }

        //}

        //private static string getLastSalesRequisitionNumber(out string msg)
        //{
        //    try
        //    {
        //        var sql1 = "Select * FROM  \"TradePlus\".\"SalesRequisition\"  ORDER BY \"SalesRequisitionId\" DESC LIMIT 1";
        //        var projects = _reqRepository.RepositoryContext().Database.SqlQuery<SalesRequisition>(sql1).ToList();
        //        if (!projects.Any())
        //        {
        //            msg = "";
        //            return "";
        //        }
        //        if (projects.Count != 1)
        //        {
        //            msg = "Invalid last project code";
        //            return null;
        //        }
        //        if (projects[0] == null)
        //        {
        //            msg = "Unable to retreive last Requisition Number";
        //            return null;
        //        }
        //        if (string.IsNullOrEmpty(projects[0].RequisitionNumber))
        //        {
        //            msg = "Invalid last Requisition Number";
        //            return null;
        //        }
        //        msg = "";
        //        return projects[0].RequisitionNumber;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
        //        msg = "Error occurred while generating Requisition Number! Please try again later";
        //        return "0";
        //    }
        //}

        #endregion
    }
}