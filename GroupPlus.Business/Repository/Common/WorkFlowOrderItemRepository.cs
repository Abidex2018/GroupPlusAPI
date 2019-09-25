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
    //class WorkflowOrderItemItemRepository
    //{
    //}

    internal class WorkflowOrderItemRepository
    {
        private readonly IPlugRepository<WorkflowOrderItem> _repository;
        private readonly PlugUoWork _uoWork;

        public WorkflowOrderItemRepository()
        {
            _uoWork = new PlugUoWork();
            _repository = new PlugRepository<WorkflowOrderItem>(_uoWork);
        }

        public WorkflowOrderItem GetWorkflowOrderItem(int workflowOrderItemId)
        {
            try
            {
                return GetWorkflowOrderItems().Find(k => k.WorkflowOrderItemId == workflowOrderItemId) ??
                       new WorkflowOrderItem();
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new WorkflowOrderItem();
            }
        }

        public List<WorkflowOrderItem> GetWorkflowOrderItems()
        {
            try
            {
                if (!(CacheManager.GetCache("ccWorkflowOrderItemList") is List<WorkflowOrderItem> settings) ||
                    settings.IsNullOrEmpty())
                {
                    var myItemList = _repository.GetAll().OrderBy(m => m.WorkflowOrderItemId);
                    if (!myItemList.Any()) return new List<WorkflowOrderItem>();
                    settings = myItemList.ToList();
                    if (settings.IsNullOrEmpty())
                        return new List<WorkflowOrderItem>();
                    CacheManager.SetCache("ccWorkflowOrderItemList", settings, DateTime.Now.AddYears(1));
                }
                return settings;
            }
            catch (Exception ex)
            {
                ErrorManager.LogApplicationError(ex.StackTrace, ex.Source, ex.Message);
                return new List<WorkflowOrderItem>();
            }
        }

        internal void resetCache()
        {
            try
            {
                HelperMethods.clearCache("ccWorkflowOrderItemList");
                GetWorkflowOrderItems();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public SettingRegRespObj AddWorkflowOrderItem(RegWorkFlowOrderItemObj regObj)
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

                if (IsWorkflowOrderItemDuplicate(regObj.Name, regObj.WorkflowOrderId, regObj.Order, 1, ref response))
                    return response;

                var workflowOrderItem = new WorkflowOrderItem
                {
                    Name = regObj.Name,
                    WorkflowOrderId = regObj.WorkflowOrderId,
                    Order = regObj.Order,
                    Status = (ItemStatus) regObj.Status
                };

                var added = _repository.Add(workflowOrderItem);

                _uoWork.SaveChanges();

                if (added.WorkflowOrderItemId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderItemId;
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

        public SettingRegRespObj UpdateWorkflowOrderItem(EditWorkFlowOrderItemObj regObj)
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

                var thisWorkflowOrderItem = getWorkflowOrderItemInfo(regObj.WorkflowOrderItemId);

                if (thisWorkflowOrderItem == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No WorkflowOrderItem Information found for the specified WorkflowOrderItem Id";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrderItem Information found!";
                    return response;
                }

                if (IsWorkflowOrderItemDuplicate(regObj.Name, regObj.WorkflowOrderId, regObj.Order, 1, ref response))
                    return response;

                thisWorkflowOrderItem.Name = regObj.Name;
                thisWorkflowOrderItem.WorkflowOrderId = regObj.WorkflowOrderId;
                thisWorkflowOrderItem.Order = regObj.Order;
                thisWorkflowOrderItem.Status = (ItemStatus) regObj.Status;

                var added = _repository.Update(thisWorkflowOrderItem);
                _uoWork.SaveChanges();

                if (added.WorkflowOrderItemId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderItemId;
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

        public SettingRegRespObj DeleteWorkflowOrderItem(DeleteWorkFlowOrderItemObj regObj)
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
                var thisWorkflowOrderItem = getWorkflowOrderItemInfo(regObj.WorkflowOrderItemId);

                if (thisWorkflowOrderItem == null)
                {
                    response.Status.Message.FriendlyMessage =
                        "No WorkflowOrderItem Information found for the specified WorkflowOrderItem Id";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrderItem Information found!";
                    return response;
                }
                thisWorkflowOrderItem.Name =
                    thisWorkflowOrderItem.Name + "_Deleted_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                thisWorkflowOrderItem.Status = ItemStatus.Deleted;
                var added = _repository.Update(thisWorkflowOrderItem);
                _uoWork.SaveChanges();

                if (added.WorkflowOrderItemId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                resetCache();
                response.Status.IsSuccessful = true;
                response.SettingId = added.WorkflowOrderItemId;
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

        public WorkFlowOrderItemRespObj LoadWorkflowOrderItems(CommonSettingSearchObj searchObj)
        {
            var response = new WorkFlowOrderItemRespObj
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
                var thisWorkflowOrderItems = GetWorkflowOrderItems();
                if (!thisWorkflowOrderItems.Any())
                {
                    response.Status.Message.FriendlyMessage = "No WorkflowOrderItem Information found!";
                    response.Status.Message.TechnicalMessage = "No WorkflowOrderItem  Information found!";
                    return response;
                }

                if (searchObj.Status > -1)
                    thisWorkflowOrderItems =
                        thisWorkflowOrderItems.FindAll(p => p.Status == (ItemStatus) searchObj.Status);

                var workflowOrderItemItems = new List<WorkFlowOrderItemObj>();
                thisWorkflowOrderItems.ForEachx(m =>
                {
                    workflowOrderItemItems.Add(new WorkFlowOrderItemObj
                    {
                        WorkflowOrderItemId = m.WorkflowOrderItemId,
                        Name = m.Name,
                        WorkflowOrderId = m.WorkflowOrderId,
                        WorkflowOrderTitle = new WorkFlowOrderRepository().GetWorkflowOrder(m.WorkflowOrderId).Title,
                        Order = m.Order,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " ")
                    });
                });

                response.Status.IsSuccessful = true;
                response.WorkFlowOrderItems = workflowOrderItemItems;
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

        private bool IsWorkflowOrderItemDuplicate(string name, int workFlowOrderId, int order, int callType,
            ref SettingRegRespObj response)
        {
            try
            {
                var sql1 =
                    $"Select coalesce(Count(\"WorkflowOrderItemId\"), 0) FROM  \"GPlus\".\"WorkflowOrderItem\"  WHERE lower(\"Name\") = lower('{name.Replace("'", "''")}') AND \"WorkflowOrderId\" = {workFlowOrderId} AND \"Order\" = {order}";
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
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error!WorkflowOrderItem Name already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! WorkflowOrderItem Name already exist";
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

        private WorkflowOrderItem getWorkflowOrderItemInfo(int WorkflowOrderItemId)
        {
            try
            {
                var sql1 =
                    $"SELECT *  FROM  \"GPlus\".\"WorkflowOrderItem\" WHERE  \"WorkflowOrderItemId\" = {WorkflowOrderItemId};";
                var agentInfos = _repository.RepositoryContext().Database.SqlQuery<WorkflowOrderItem>(sql1).ToList();
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