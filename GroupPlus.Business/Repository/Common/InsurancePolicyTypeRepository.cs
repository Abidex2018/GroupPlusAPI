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
    internal class InsurancePolicyTypeRepository
    {
        private readonly IPlugRepository<InsurancePolicyType> _repository;
        private readonly PlugUoWork _uoWork;

        public InsurancePolicyTypeRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<InsurancePolicyType>(_uoWork);
        }

        public InsurancePolicyType GetInsurancePolicyType(int insurancePolicyTypeId)
        {
            try
            {
                return GetInsurancePolicyTypes().Find(k => k.InsurancePolicyTypeId == insurancePolicyTypeId) ??
                       new InsurancePolicyType();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new InsurancePolicyType();
            }
        }

        public List<InsurancePolicyType> GetInsurancePolicyTypes()
        {
            try
            {
                if (!(CacheManager.GetCache("ccInsurancePolicyTypeList") is List<InsurancePolicyType> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.InsurancePolicyTypeId);
                    if (!myItemList.Any()) return new List<InsurancePolicyType>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<InsurancePolicyType>();
                    CacheManager.SetCache("ccInsurancePolicyTypeList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<InsurancePolicyType>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccInsurancePolicyTypeList");
                GetInsurancePolicyTypes();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddInsurancePolicyType(RegInsurancePolicyTypeObj regObj)
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

                if (IsInsurancePolicyTypeDuplicate(regObj.Name, 1, ref response)) return response;

                var insurancePolicyType = new InsurancePolicyType
                {
                    Name = regObj.Name,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(insurancePolicyType);

                _uoWork.SaveChanges();

                if (added.InsurancePolicyTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.InsurancePolicyTypeId;
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

        public SettingRegRespObj UpdateInsurancePolicyType(EditInsurancePolicyTypeObj regObj)
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

                var thisInsurancePolicyType = getInsurancePolicyTypeInfo(regObj.InsurancePolicyTypeId);

                if (thisInsurancePolicyType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Insurance Policy Type Information found for the specified InsurancePolicyType Id";
                    response.Status.Message.TechnicalMessage = "No Insurance Policy Type Information found!";
                    return response;
                }

                if (IsInsurancePolicyTypeDuplicate(regObj.Name, 2, ref response)) return response;
                thisInsurancePolicyType.Name = regObj.Name;
                thisInsurancePolicyType.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisInsurancePolicyType);
                _uoWork.SaveChanges();

                if (added.InsurancePolicyTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.InsurancePolicyTypeId;
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

        public SettingRegRespObj DeleteInsurancePolicyType(DeleteInsurancePolicyTypeObj regObj)
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
                var thisInsurancePolicyType = getInsurancePolicyTypeInfo(regObj.InsurancePolicyTypeId);

                if (thisInsurancePolicyType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Insurance Policy Type Information found for the specified InsurancePolicyType Id";
                    response.Status.Message.TechnicalMessage = "No Insurance Policy Type Information found!";
                    return response;
                }
                thisInsurancePolicyType.Name = thisInsurancePolicyType.Name + "_Deleted_" +
                                               DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisInsurancePolicyType.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisInsurancePolicyType);
                _uoWork.SaveChanges();

                if (added.InsurancePolicyTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.InsurancePolicyTypeId;
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

        public InsurancePolicyTypeRespObj LoadInsurancePolicyTypes(CommonSettingSearchObj searchObj)
        {
            var response = new InsurancePolicyTypeRespObj
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
                var thisInsurancePolicyTypes = GetInsurancePolicyTypes();
                if (!thisInsurancePolicyTypes.Any())
                {
                    response.Status.Message.FriendlyMessage = "No InsurancePolicyType Information found!";
                    response.Status.Message.TechnicalMessage = "No InsurancePolicyType  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisInsurancePolicyTypes =
                        thisInsurancePolicyTypes.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var insurancePolicyTypeItems = new List<InsurancePolicyTypeObj>();
                thisInsurancePolicyTypes.ForEachx(m =>
                {
                    insurancePolicyTypeItems.Add(new InsurancePolicyTypeObj
                    {
                        InsurancePolicyTypeId = m.InsurancePolicyTypeId,
                        Name = m.Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.InsurancePolicyTypes = insurancePolicyTypeItems;
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

        private bool IsInsurancePolicyTypeDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"InsurancePolicyTypeId\"), 0) FROM  \"GPlus\".\"InsurancePolicyType\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
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
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! InsurancePolicyType Name already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! InsurancePolicyType Name already exist";
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

        private InsurancePolicyType getInsurancePolicyTypeInfo(int insurancePolicyTypeId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"InsurancePolicyType\" WHERE  \"InsurancePolicyTypeId\" = {insurancePolicyTypeId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<InsurancePolicyType>(sql1).ToList();
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