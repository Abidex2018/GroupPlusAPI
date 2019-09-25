using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GroupPlus.Business.Core;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Infrastructure;
using GroupPlus.Business.Infrastructure.Contract;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.Settings;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.Common
{
    internal class KPIndexRepository
    {
        private readonly IPlugRepository<KPIndex> _repository;
        private readonly PlugUoWork _uoWork;

        public KPIndexRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<KPIndex>(_uoWork);
        }

        public KPIndex GetkPIndex(int kPIndexId)
        {
            try
            {
                return GetkPIndexs().Find(k => k.KPIndexId == kPIndexId) ?? new KPIndex();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new KPIndex();
            }
        }

        public List<KPIndex> GetkPIndexs()
        {
            try
            {
                if (!(CacheManager.GetCache("ccKPIndexList") is List<KPIndex> settings) || settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.KPIndexId);
                    if (!myItemList.Any()) return new List<KPIndex>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<KPIndex>();
                    CacheManager.SetCache("ccKPIndexList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<KPIndex>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccKPIndexList");
                GetkPIndexs();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddkPIndex(RegKPIndexObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                    return response;

                if (IskPIndexDuplicate(regObj.Name, 1, ref response)) return response;
                if (regObj.MinRating > regObj.MaxRating)
                {
                    response.Status.Message.FriendlyMessage = "Minimum Rating cannot be higher than Maximum Rating";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                if (regObj.MinRating == regObj.MaxRating)
                {
                    response.Status.Message.FriendlyMessage = "Minimum/Maximum Rating cannot be equal to each other";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                var kPIndex = new KPIndex
                {
                    Name = regObj.Name,
                    Indicator = regObj.Indicator,
                    MinRating = regObj.MinRating,
                    MaxRating = regObj.MaxRating,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(kPIndex);

                _uoWork.SaveChanges();

                if (added.KPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.KPIndexId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj UpdatekPIndex(EditKPIndexObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode,
                    HelperMethods.getSeniorAccountant(), ref response.Status.Message))
                    return response;

                var thiskPIndex = GetkPIndexInfo(regObj.KPIndexId);

                if (thiskPIndex == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Key Performance Indicator Information found for the specified KPIndex Id";
                    response.Status.Message.TechnicalMessage = "No Key Performance Indicator Information found!";
                    return response;
                }

                if (IskPIndexDuplicate(regObj.Name, 2, ref response)) return response;

                if (regObj.MinRating > regObj.MaxRating)
                {
                    response.Status.Message.FriendlyMessage = "Minimum Rating cannot be higher than Maximum Rating";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                if (regObj.MinRating == regObj.MaxRating)
                {
                    response.Status.Message.FriendlyMessage = "Minimum/Maximum Rating cannot be equal to each other";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                thiskPIndex.Name = regObj.Name;
                thiskPIndex.Indicator = regObj.Indicator;
                thiskPIndex.MinRating = regObj.MinRating;
                thiskPIndex.MaxRating = regObj.MaxRating;
                thiskPIndex.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thiskPIndex);
                _uoWork.SaveChanges();

                if (added.KPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.KPIndexId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public SettingRegRespObj DeletekPIndex(DeleteKPIndexObj regObj)
        {
            var response = new SettingRegRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (regObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(regObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }
                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                if (!HelperMethods.IsUserValid(regObj.AdminUserId, regObj.SysPathCode, HelperMethods.getStaffRoles(),
                    ref response.Status.Message))
                    return response;
                var thiskPIndex = GetkPIndexInfo(regObj.KPIndexId);

                if (thiskPIndex == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Key Performance Indicator Information found for the specified KPIndex Id";
                    response.Status.Message.TechnicalMessage = "No Key Performance Indicator Information found!";
                    return response;
                }
                thiskPIndex.Name = thiskPIndex.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thiskPIndex.Status = ItemStatus.Deleted;
                var added = _repository.Update(thiskPIndex);
                _uoWork.SaveChanges();

                if (added.KPIndexId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.KPIndexId;
            }
            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
            return response;
        }

        public KPIndexRespObj LoadkPIndexs(CommonSettingSearchObj searchObj)
        {
            var response = new KPIndexRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                if (searchObj.Equals(null))
                {
                    response.Status.Message.FriendlyMessage = "Error Occurred! Unable to proceed with your request";
                    response.Status.Message.TechnicalMessage = "Registration Object is empty / invalid";
                    return response;
                }

                if (!EntityValidatorHelper.Validate(searchObj, out var valResults))
                {
                    var errorDetail = new StringBuilder();
                    if (!valResults.IsNullOrEmpty())
                    {
                        errorDetail.AppendLine("Following error occurred:");
                        valResults.ForEachx(m => errorDetail.AppendLine(m.ErrorMessage));
                    }

                    else
                    {
                        errorDetail.AppendLine(
                            "Validation error occurred! Please check all supplied parameters and try again");
                    }
                    response.Status.Message.FriendlyMessage = errorDetail.ToString();
                    response.Status.Message.TechnicalMessage = errorDetail.ToString();
                    response.Status.IsSuccessful = false;
                    return response;
                }

                //if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode, HelperMethods.getAllRoles(), ref response.Status.Message))
                //{
                //    return response;
                //}
                var thiskPIndex = GetkPIndexs();
                if (!thiskPIndex.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Key Performance Indicator Information found!";
                    response.Status.Message.TechnicalMessage = "No Key Performance Indicator  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thiskPIndex = thiskPIndex.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var kPIndexItems = new List<KPIndexObj>();
                thiskPIndex.ForEachx(m =>
                {
                    kPIndexItems.Add(new KPIndexObj
                    {
                        KPIndexId = m.KPIndexId,
                        Name = m.Name,
                        Indicator = m.Indicator,
                        MaxRating = m.MaxRating,
                        MinRating = m.MinRating,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.KPIndexs = kPIndexItems;
                return response;
            }

            catch (DbEntityValidationException ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                response.Status.Message.FriendlyMessage = "Error Occurred! Please try again later";
                response.Status.Message.TechnicalMessage = "Error: " + ex.GetBaseException().Message;
                response.Status.IsSuccessful = false;
                return response;
            }
        }

        private bool IskPIndexDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"KPIndexId\"), 0) FROM  \"GPlus\".\"KPIndex\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
                var check = _repository.RepositoryContext().Database.SqlQuery<long>(sql1).ToList();

                if (check.IsNullOrEmpty())
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to complete your request due to validation error. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check for duplicate";
                    return true;
                }

                if (check.Count != 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Unable to complete your request due to validation error. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to check for duplicate";
                    return true;
                }

                if (check[0] < 1)
                    return false;

                if (check[0] > 0)
                    if (callType != 2 || check[0] > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error! KPIndex Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! KPIndex Name already exist";
                        return true;
                    }
                return false;
            }
            catch (Exception ex)
            {
                response.Status.Message.FriendlyMessage =
                    "Unable to complete your request due to validation error. Please try again later";
                response.Status.Message.TechnicalMessage = "Duplicate Check Error: " + ex.Message;
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return true;
            }
        }

        private KPIndex GetkPIndexInfo(int kPIndexId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"KPIndex\" WHERE  \"KPIndexId\" = {kPIndexId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<KPIndex>(sql1).ToList();
                if (!agentInfos.Any() || agentInfos.Count != 1)
                    return null;
                return agentInfos[0];
            }

            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }
    }
}