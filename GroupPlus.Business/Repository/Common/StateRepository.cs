﻿using System;
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
    internal class StateRepository
    {
        private readonly IPlugRepository<State> _repository;
        private readonly PlugUoWork _uoWork;

        public StateRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<State>(_uoWork);
        }

        public State GetState(int stateId)
        {
            try
            {
                return GetStates().Find(k => k.StateId == stateId) ?? new State();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new State();
            }
        }

        public List<State> GetStates()
        {
            try
            {
                if (!(CacheManager.GetCache("ccStateList") is List<State> settings) || settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.StateId);
                    if (!myItemList.Any()) return new List<State>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<State>();
                    CacheManager.SetCache("ccStateList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<State>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccStateList");
                GetStates();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddState(RegStateObj regObj)
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

                if (IsStateDuplicate(regObj.Name, 1, ref response)) return response;

                var state = new State
                {
                    Name = regObj.Name,
                    CountryId = regObj.CountryId,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(state);

                _uoWork.SaveChanges();

                if (added.StateId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.StateId;
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

        public SettingRegRespObj UpdateState(EditStateObj regObj)
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

                var thisState = getStateInfo(regObj.StateId);

                if (thisState == null)
                {
                    response.Status.Message.FriendlyMessage = "No State Information found for the specified State Id";
                    response.Status.Message.TechnicalMessage = "No State Information found!";
                    return response;
                }

                if (IsStateDuplicate(regObj.Name, 2, ref response)) return response;
                thisState.Name = regObj.Name;
                thisState.CountryId = regObj.CountryId;
                thisState.Status = (ItemStatus) regObj.Status;
                var added = _repository.Update(thisState);
                _uoWork.SaveChanges();

                if (added.StateId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.StateId;
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

        public SettingRegRespObj DeleteState(DeleteStateObj regObj)
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
                var thisState = getStateInfo(regObj.StateId);

                if (thisState == null)
                {
                    response.Status.Message.FriendlyMessage = "No State Information found for the specified State Id";
                    response.Status.Message.TechnicalMessage = "No State Information found!";
                    return response;
                }
                thisState.Name = thisState.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisState.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisState);
                _uoWork.SaveChanges();

                if (added.StateId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.StateId;
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

        public StateRespObj LoadStates()
        {
            var response = new StateRespObj
            {
                Status = new APIResponseStatus
                {
                    IsSuccessful = false,
                    Message = new APIResponseMessage()
                }
            };

            try
            {
                var thisStates = GetStates();
                if (!thisStates.Any())
                {
                    response.Status.Message.FriendlyMessage = "No State Information found!";
                    response.Status.Message.TechnicalMessage = "No State  Information found!";
                    return response;
                }
                var stateItems = new List<StateObj>();
                thisStates.ForEachx(m =>
                {
                    stateItems.Add(new StateObj
                    {
                        StateId = m.StateId,
                        Name = m.Name,
                        CountryId = m.CountryId,
                        CountryLabel = m.Country.Name,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.States = stateItems;
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

        private bool IsStateDuplicate(string name, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"StateId\"), 0) FROM  \"GPlus\".\"State\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}')";
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
                        response.Status.Message.FriendlyMessage = "Duplicate Error! State Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! State Name already exist";
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

        private State getStateInfo(int stateId)
        {
            try
            {
                var sql1 = $"SELECT *  FROM  \"GPlus\".\"State\" WHERE  \"StateId\" = {stateId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<State>(sql1).ToList();
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