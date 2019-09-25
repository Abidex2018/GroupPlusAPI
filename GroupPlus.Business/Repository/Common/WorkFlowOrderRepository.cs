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
using GroupPlus.BussinessObject.Workflow;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.Common
{
    //class WorkFlowOrderRepository
    //{
    //}

    internal class WorkFlowOrderRepository
    {
        private readonly IPlugRepository<WorkflowOrder> _repository;
        private readonly PlugUoWork _uoWork;

        public WorkFlowOrderRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<WorkflowOrder>(_uoWork);
        }

        public WorkflowOrder GetWorkflowOrder(int workflowOrderId)
        {
            try
            {
                return GetWorkflowOrders().Find(k => k.WorkflowOrderId == workflowOrderId) ?? new WorkflowOrder();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new WorkflowOrder();
            }
        }

        public List<WorkflowOrder> GetWorkflowOrders()
        {
            try
            {
                if (!(CacheManager.GetCache("ccWorkflowOrderList") is List<WorkflowOrder> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.WorkflowOrderId);
                    if (!myItemList.Any()) return new List<WorkflowOrder>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<WorkflowOrder>();
                    CacheManager.SetCache("ccWorkflowOrderList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<WorkflowOrder>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccWorkflowOrderList");
                GetWorkflowOrders();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddWorkflowOrder(RegWorkFlowOrderObj regObj)
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

                if (IsWorkflowOrderDuplicate(regObj.Title, 1, ref response))
                    return response;

                var workflowOrder = new WorkflowOrder
                {
                    Title = regObj.Title,
                    Status = (ItemStatus) regObj.Status,
                    TimeStampRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _repository.Add(workflowOrder);

                _uoWork.SaveChanges();

                if (added.WorkflowOrderId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderId;
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

        public SettingRegRespObj UpdateWorkflowOrder(EditWorkFlowOrderObj regObj)
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

                var thisWorkflowOrder = getWorkflowOrderInfo(regObj.WorkflowOrderId);

                if (thisWorkflowOrder == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No WorkflowOrder Information found for the specified WorkflowOrder Id";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrder Information found!";
                    return response;
                }

                if (IsWorkflowOrderDuplicate(regObj.Title, 2, ref response))
                    return response;

                thisWorkflowOrder.Title = regObj.Title;
                thisWorkflowOrder.Status = (ItemStatus) regObj.Status;

                var added = _repository.Update(thisWorkflowOrder);
                _uoWork.SaveChanges();

                if (added.WorkflowOrderId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderId;
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

        public SettingRegRespObj DeleteWorkflowOrder(DeleteWorkFlowOrderObj regObj)
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
                var thisWorkflowOrder = getWorkflowOrderInfo(regObj.WorkflowOrderId);

                if (thisWorkflowOrder == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No WorkflowOrder Information found for the specified WorkflowOrder Id";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrder Information found!";
                    return response;
                }
                thisWorkflowOrder.Title =
                    thisWorkflowOrder.Title + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisWorkflowOrder.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisWorkflowOrder);
                _uoWork.SaveChanges();

                if (added.WorkflowOrderId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderId;
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

        public WorkFlowOrderRespObj LoadWorkflowOrders(CommonSettingSearchObj searchObj)
        {
            var response = new WorkFlowOrderRespObj
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

                //if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                //    HelperMethods.getAllRoles(), ref response.Status.Message))
                //{
                //    return response;
                //}
                var thisWorkflowOrders = GetWorkflowOrders();
                if (!thisWorkflowOrders.Any())
                {
                    response.Status.Message.FriendlyMessage = "No WorkflowOrder Information found!";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrder  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisWorkflowOrders = thisWorkflowOrders.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var workflowOrderItems = new List<WorkFlowOrderObj>();
                thisWorkflowOrders.ForEachx(m =>
                {
                    workflowOrderItems.Add(new WorkFlowOrderObj
                    {
                        WorkflowOrderId = m.WorkflowOrderId,
                        Title = m.Title,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStampRegistered = m.TimeStampRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.WorkFlowOrders = workflowOrderItems;
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

        private bool IsWorkflowOrderDuplicate(string title, int callType, ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"WorkflowOrderId\"), 0) FROM  \"GPlus\".\"WorkflowOrder\"  WHERE lower(\"Title\") = lower('{title.Replace("'", "''")}')";
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
                {
                    if (callType != 2 || check[0] > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Duplicate Error!WorkflowOrder Name already exist";
                        response.Status.Message.TechnicalMessage = "Duplicate Error! WorkflowOrder Name already exist";
                        return true;
                    }

                    return false;
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

        private WorkflowOrder getWorkflowOrderInfo(int workflowOrderId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"WorkflowOrder\" WHERE  \"WorkflowOrderId\" = {workflowOrderId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<WorkflowOrder>(sql1).ToList();
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