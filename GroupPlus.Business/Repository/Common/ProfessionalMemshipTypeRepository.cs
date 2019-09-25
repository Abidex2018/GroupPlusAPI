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
    internal class ProfessionalMemshipTypeRepository
    {
        private readonly IPlugRepository<ProfessionalMembershipType> _repository;
        private readonly PlugUoWork _uoWork;

        public ProfessionalMemshipTypeRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<ProfessionalMembershipType>(_uoWork);
        }

        public ProfessionalMembershipType GetProfessionalMembershipType(int professionalMembershipTypeId)
        {
            try
            {
                return GetProfessionalMembershipTypes()
                           .Find(k => k.ProfessionalMembershipTypeId == professionalMembershipTypeId) ??
                       new ProfessionalMembershipType();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new ProfessionalMembershipType();
            }
        }

        public List<ProfessionalMembershipType> GetProfessionalMembershipTypes()
        {
            try
            {
                if (!(CacheManager.GetCache("ccProfessionalMembershipTypeList") is List<ProfessionalMembershipType>
                        settings) || settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.ProfessionalMembershipTypeId);
                    if (!myItemList.Any()) return new List<ProfessionalMembershipType>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<ProfessionalMembershipType>();
                    CacheManager.SetCache("ccProfessionalMembershipTypeList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<ProfessionalMembershipType>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccProfessionalMembershipTypeList");
                GetProfessionalMembershipTypes();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddProfessionalMembershipType(RegProfessionalMembershipTypeObj regObj)
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

                if (IsProfessionalMembershipTypeDuplicate(regObj.Name, 1, ref response)) return response;

                var professionalMembershipType = new ProfessionalMembershipType
                {
                    Name = regObj.Name,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(professionalMembershipType);

                _uoWork.SaveChanges();

                if (added.ProfessionalMembershipTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ProfessionalMembershipTypeId;
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

        public SettingRegRespObj UpdateProfessionalMembershipType(EditProfessionalMembershipTypeObj regObj)
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

                var thisProfessionalMembershipType =
                    getProfessionalMembershipTypeInfo(regObj.ProfessionalMembershipTypeId);

                if (thisProfessionalMembershipType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Professional Membership Type Information found for the specified ProfessionalMembershipType Id";
                    response.Status.Message.TechnicalMessage = "No Professional Membership Type Information found!";
                    return response;
                }

                if (IsProfessionalMembershipTypeDuplicate(regObj.Name, 2, ref response)) return response;
                thisProfessionalMembershipType.Name = regObj.Name;
                thisProfessionalMembershipType.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisProfessionalMembershipType);
                _uoWork.SaveChanges();

                if (added.ProfessionalMembershipTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ProfessionalMembershipTypeId;
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

        public SettingRegRespObj DeleteProfessionalMembershipType(DeleteProfessionalMembershipTypeObj regObj)
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
                var thisProfessionalMembershipType =
                    getProfessionalMembershipTypeInfo(regObj.ProfessionalMembershipTypeId);

                if (thisProfessionalMembershipType == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No Professional Membership Type Information found for the specified ProfessionalMembershipType Id";
                    response.Status.Message.TechnicalMessage = "No Professional Membership Type Information found!";
                    return response;
                }
                thisProfessionalMembershipType.Name = thisProfessionalMembershipType.Name + "_Deleted_" +
                                                      DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisProfessionalMembershipType.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisProfessionalMembershipType);
                _uoWork.SaveChanges();

                if (added.ProfessionalMembershipTypeId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.ProfessionalMembershipTypeId;
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

        public ProfessionalMembershipTypeRespObj LoadProfessionalMembershipTypes(CommonSettingSearchObj searchObj)
        {
            var response = new ProfessionalMembershipTypeRespObj
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
                var thisProfessionalMembershipTypes = GetProfessionalMembershipTypes();
                if (!thisProfessionalMembershipTypes.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Professional Membership Type Information found!";
                    response.Status.Message.TechnicalMessage = "No Professional Membership Type  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisProfessionalMembershipTypes =
                        thisProfessionalMembershipTypes.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var professionalMembershipTypeItems = new List<ProfessionalMembershipTypeObj>();
                thisProfessionalMembershipTypes.ForEachx(m =>
                {
                    professionalMembershipTypeItems.Add(new ProfessionalMembershipTypeObj
                    {
                        ProfessionalMembershipTypeId = m.ProfessionalMembershipTypeId,
                        Name = m.Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.ProfessionalMembershipTypes = professionalMembershipTypeItems;
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

        private bool IsProfessionalMembershipTypeDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"ProfessionalMembershipTypeId\"), 0) FROM  \"GPlus\".\"ProfessionalMembershipType\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
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
                            "Duplicate Error! Professional Membership Type Name already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! Professional Membership Type Name already exist";
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

        private ProfessionalMembershipType getProfessionalMembershipTypeInfo(int professionalMembershipTypeId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"ProfessionalMembershipType\" WHERE  \"ProfessionalMembershipTypeId\" = {professionalMembershipTypeId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<ProfessionalMembershipType>(sql1)
                    .ToList();
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