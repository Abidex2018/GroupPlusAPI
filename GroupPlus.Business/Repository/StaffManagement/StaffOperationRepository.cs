using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using GroupPlus.Business.Core;
using GroupPlus.Business.Core.ItemComparers;
using GroupPlus.Business.DataManager;
using GroupPlus.Business.Repository.Common;
using GroupPlus.Business.Repository.CompanyManagement;
using GroupPlus.BusinessContract.CommonAPIs;
using GroupPlus.BusinessObject.StaffDetail;
using GroupPlus.BusinessObject.StaffManagement;
using GroupPlus.Common;
using XPLUG.WEBTOOLS;

namespace GroupPlus.Business.Repository.StaffManagement
{
    internal partial class StaffRepository
    {
        #region Staff Contact logic

        public SettingRegRespObj AddStaffContact(RegStaffContactObj regObj)
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

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffInfo = getStaffInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                //var searchObj = new StaffContactSearchObj
                //{
                //    AdminUserId = 0,
                //    LocalAreaOfResidenceId = 0,
                //    LocationLandmark = "",
                //    ResidentialAddress = "",
                //    Status = -1,
                //    StaffId =regObj.AdminUserId,
                //    StateOfResidenceId = 0,
                //    SysPathCode = "",
                //    TownOfResidence = "",
                //};

                //retrieve previous contacts and validate with the new one
                var staffId = staffInfo.StaffId;
                var staffContacts = getStaffContactInfo(staffId);
                if (staffContacts != null && staffContacts.Any())
                {
                    var defCount = staffContacts.Count(m => m.IsDefault);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Only 1 Default Contact is allowed";
                        response.Status.Message.TechnicalMessage = "Only 1 Default Contact is allowed";
                        return response;
                    }

                    if (defCount > 0 && regObj.IsDefault)
                    {
                        response.Status.Message.FriendlyMessage =
                            "There is already a Default Contact specified! Kindly change to none-default ";
                        response.Status.Message.TechnicalMessage =
                            "There is already a Default Contact specified! Kindly change to none-default ";
                        return response;
                    }

                    var myList = new List<AddressHelper>();
                    staffContacts.ForEachx(m =>
                    {
                        myList.Add(new AddressHelper {Address = m.ResidentialAddress, Id = m.StaffContactId});
                    });

                    myList.Add(new AddressHelper {Address = regObj.ResidentialAddress, Id = myList.Count + 1});
                    var distinctAdd = myList.Distinct(new AddressComparer());

                    if (staffContacts.Count + 1 != distinctAdd.Count())
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! At least one Residential Address already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! At least one Residential Address already exist";
                        return response;
                    }
                }

                var contactInfo = new StaffContact
                {
                    StaffId = regObj.AdminUserId,
                    LocalAreaOfResidenceId = regObj.LocalAreaOfResidenceId,
                    ResidentialAddress = regObj.ResidentialAddress,
                    StateOfResidenceId = regObj.StateOfResidenceId,
                    LocationLandmark = regObj.LocationLandmark,
                    TownOfResidence = regObj.TownOfResidence,
                    Status = (ItemStatus) regObj.Status,
                    IsDefault = regObj.IsDefault,
                    TimeStamRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _staffContactRepository.Add(contactInfo);
                _uoWork.SaveChanges();
                if (added.StaffContactId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffContactId;
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

        public SettingRegRespObj UpdateStaffContact(EditStaffContactObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                //var searchObj = new StaffContactSearchObj
                //{
                //    AdminUserId = 0,
                //    LocalAreaOfResidenceId = 0,
                //    LocationLandmark = "",
                //    ResidentialAddress = "",
                //    Status = -1,
                //    StaffId = regObj.AdminUserId,
                //    StaffContactId=0,
                //    StateOfResidenceId = 0,
                //    SysPathCode = "",
                //    TownOfResidence = "",
                //};

                //retrieve previous contacts and validate with the new one
                var staffId = staffInfo.StaffId;
                var staffContacts = getStaffContactInfo(staffId);

                if (staffContacts == null || !staffContacts.Any())
                {
                    response.Status.Message.FriendlyMessage = "No previous Staff Contact Information found";
                    response.Status.Message.TechnicalMessage = "No previous Staff Contact Information found";
                    return response;
                }
                var thisCurrentContact = staffContacts.Find(m => m.StaffContactId == regObj.StaffContactId);
                if (thisCurrentContact.StaffContactId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Contact Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Contact Information not found";
                    return response;
                }

                var defItems = staffContacts.FindAll(m => m.IsDefault);
                if (defItems.Count > 1)
                {
                    response.Status.Message.FriendlyMessage = "Only 1 Default Contact is allowed";
                    response.Status.Message.TechnicalMessage = "Only 1 Default Contact is allowed";
                    return response;
                }

                if (defItems.Count > 0 && regObj.IsDefault && defItems[0].StaffContactId != regObj.StaffContactId)
                {
                    response.Status.Message.FriendlyMessage =
                        "There is already a Default Contact specified! Kindly change to none-default ";
                    response.Status.Message.TechnicalMessage =
                        "There is already a Default Contact specified! Kindly change to none-default ";
                    return response;
                }

                //if (staffContacts.Count > 0)
                //{
                //var myList = new List<AddressHelper>();
                //staffContacts.ForEachx(m =>
                //{
                //    myList.Add(new AddressHelper { Address = m.ResidentialAddress, Id = m.StaffContactId });
                //});

                //myList.Add(new AddressHelper { Address = regObj.ResidentialAddress, Id = myList.Count });
                //var distinctAdd = myList.Distinct(new AddressComparer());

                //if (staffContacts.Count + 1  != distinctAdd.Count())
                //{
                //    response.Status.Message.FriendlyMessage =
                //        "Duplicate Error! At least one Residential Address already exist";
                //    response.Status.Message.TechnicalMessage =
                //        "Duplicate Error! At least one Residential Address already exist";
                //    return response;
                //}

                //}
                thisCurrentContact.StaffId = regObj.AdminUserId;
                thisCurrentContact.ResidentialAddress = regObj.ResidentialAddress;
                thisCurrentContact.TownOfResidence = regObj.TownOfResidence;
                thisCurrentContact.LocalAreaOfResidenceId = regObj.LocalAreaOfResidenceId;
                thisCurrentContact.IsDefault = regObj.IsDefault;
                thisCurrentContact.LocationLandmark = regObj.LocationLandmark;
                thisCurrentContact.StateOfResidenceId = regObj.StateOfResidenceId;
                thisCurrentContact.Status = (ItemStatus) regObj.Status;

                var retVal = _staffContactRepository.Update(thisCurrentContact);
                _uoWork.SaveChanges();
                if (retVal.StaffContactId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.StaffContactId;
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

        public StaffContactRespObj LoadStaffContact(StaffContactSearchObj searchObj)
        {
            var response = new StaffContactRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisContactStaffs = getStaffContactInfos(searchObj);

                if (thisContactStaffs == null || !thisContactStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffContactItems = new List<StaffContactObj>();

                thisContactStaffs.ForEachx(m =>
                {
                    staffContactItems.Add(new StaffContactObj
                    {
                        StaffContactId = m.StaffContactId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        ResidentialAddress = m.ResidentialAddress,
                        TownOfResidence = m.TownOfResidence,
                        LocationLandmark = m.LocationLandmark,
                        IsDefault = m.IsDefault,
                        StateOfResidenceId = m.StateOfResidenceId,
                        LocalAreaOfResidenceId = m.LocalAreaOfResidenceId,
                        TimeStamRegistered = m.TimeStamRegistered,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString()
                    });
                });

                response.Status.IsSuccessful = true;
                response.Staffs = staffContactItems;
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

        public StaffContactRespObj LoadStaffContactByAdmin(StaffContactSearchObj searchObj)
        {
            var response = new StaffContactRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisContactStaffs = getStaffContactInfos(searchObj);

                if (thisContactStaffs == null || !thisContactStaffs.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffContactItems = new List<StaffContactObj>();

                thisContactStaffs.ForEachx(m =>
                {
                    staffContactItems.Add(new StaffContactObj
                    {
                        StaffContactId = m.StaffContactId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        ResidentialAddress = m.ResidentialAddress,
                        TownOfResidence = m.TownOfResidence,
                        LocationLandmark = m.LocationLandmark,
                        IsDefault = m.IsDefault,
                        StateOfResidenceId = m.StateOfResidenceId,
                        LocalAreaOfResidenceId = m.LocalAreaOfResidenceId,
                        TimeStamRegistered = m.TimeStamRegistered,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString()
                    });
                });

                response.Status.IsSuccessful = true;
                response.Staffs = staffContactItems;
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

        #endregion

        #region Emergency Contact logic

        public SettingRegRespObj AddEmergencyContact(RegEmergencyContactObj regObj)
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

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                //var staffEmerContactInfo = GetEmergencyContactInfo(regObj.AdminUserId);

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }

                var searchObj = new EmergencyContactSearchObj
                {
                    EmergencyContactId = 0,
                    StaffId = regObj.AdminUserId,
                    AdminUserId = 0,
                    MobileNumber = "",
                    ResidentialAddress = "",
                    LastName = "",
                    FirstName = "",
                    StateOfLocationId = 0,
                    LocalAreaOfLocationId = 0
                };

                //retrieve previous emergency contacts and validate with the new one
                var staffEmerContacts = GetStaffEmergencyContactsInfos(searchObj);
                if (staffEmerContacts != null && staffEmerContacts.Any())
                {
                    var defCount = staffEmerContacts.Count(m => m.IsDefault);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Only 1 Default Emergency Contact is allowed";
                        response.Status.Message.TechnicalMessage = "Only 1 Default Contact is allowed";
                        return response;
                    }

                    if (defCount > 0 && regObj.IsDefault)
                    {
                        response.Status.Message.FriendlyMessage =
                            "There is already a Default Emergency Contact specified! Kindly change to none-default ";
                        response.Status.Message.TechnicalMessage =
                            "There is already a Default Emergency Contact specified! Kindly change to none-default ";
                        return response;
                    }

                    var myList = new List<AddressHelper>();
                    var myList2 = new List<MobileNumberHelper>();
                    staffEmerContacts.ForEachx(m =>
                    {
                        myList.Add(new AddressHelper {Address = m.ResidentialAddress, Id = m.EmergencyContactId});
                        myList2.Add(new MobileNumberHelper {MobileNumber = m.MobileNumber, Id = m.EmergencyContactId});
                    });

                    myList.Add(new AddressHelper {Address = regObj.ResidentialAddress, Id = myList.Count + 1});
                    myList2.Add(new MobileNumberHelper {MobileNumber = regObj.MobileNumber, Id = myList2.Count + 1});

                    var distinctAdd = myList.Distinct(new AddressComparer());

                    var distinctAdd2 = myList2.Distinct(new MobileComparer());

                    if (staffEmerContacts.Count + 1 != distinctAdd.Count() ||
                        staffEmerContacts.Count + 1 != distinctAdd2.Count())
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! At least one Residential Address Or Mobile number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! At least one Residential Address Or Mobile number already exist";
                        return response;
                    }
                }

                var emergencyContactInfo = new EmergencyContact
                {
                    StaffId = staffInfo.StaffId,
                    FirstName = regObj.FirstName,
                    LastName = regObj.LastName,
                    MiddleName = regObj.MiddleName,
                    Gender = (Gender) regObj.Gender,
                    ResidentialAddress = regObj.ResidentialAddress,
                    StateOfLocationId = regObj.StateOfLocationId,
                    LocalAreaOfLocationId = regObj.LocalAreaOfLocationId,
                    MobileNumber = regObj.MobileNumber,
                    IsDefault = regObj.IsDefault
                };

                var added = _emergencyContactRepository.Add(emergencyContactInfo);
                _uoWork.SaveChanges();
                if (added.EmergencyContactId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.EmergencyContactId;
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

        public SettingRegRespObj UpdateEmergencyContact(EditEmergencyContactObj regObj)
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

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }
                //var searchObj = new EmergencyContactSearchObj()
                //{
                //    EmergencyContactId = 0,
                //    StaffId = regObj.StaffId,
                //    AdminUserId = 0,
                //    Status = -10,
                //    MobileNumber = "",
                //    ResidentialAddress = "",
                //    LastName = "",
                //    FirstName = "",
                //    StateOfLocationId = 0,
                //    LocalAreaOfLocationId = 0,


                //};

                //retrieve previous emergency contacts and validate with the new one
                var staffEmerContacts = getEmergencyContactInfo(staffInfo.StaffId);
                if (staffEmerContacts == null || !staffEmerContacts.Any())
                {
                    response.Status.Message.FriendlyMessage = "No previous Staff Emergency Contact Information found";
                    response.Status.Message.TechnicalMessage = "No previous Staff Emergency Contact Information found";
                    return response;
                }
                var thisCurrentEmerContact =
                    staffEmerContacts.Find(m =>
                        m.EmergencyContactId == regObj.EmergencyContactId && m.StaffId == regObj.StaffId);

                if (thisCurrentEmerContact.EmergencyContactId < 1 && thisCurrentEmerContact.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Emergency Contact Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Emergency Contact Information not found";
                    return response;
                }

                var defItems = staffEmerContacts.FindAll(m => m.IsDefault);
                if (defItems.Count > 1)
                {
                    response.Status.Message.FriendlyMessage = "Only 1 Default Emergency Contact is allowed";
                    response.Status.Message.TechnicalMessage = "Only 1 Default Emergency Contact is allowed";
                    return response;
                }

                if (defItems.Count > 0 && regObj.IsDefault &&
                    defItems[0].EmergencyContactId != regObj.EmergencyContactId)
                {
                    response.Status.Message.FriendlyMessage =
                        "There is already a Default Emergency Contact specified! Kindly change to none-default ";
                    response.Status.Message.TechnicalMessage =
                        "There is already a Default Emergency Contact specified! Kindly change to none-default ";
                    return response;
                }


                //if (staffEmerContacts.Count > 1)
                //{
                //    var myList = new List<AddressHelper>();
                //    var myList2 = new List<MobileNumberHelper>();

                //    staffEmerContacts.ForEachx(m =>
                //    {
                //        myList.Add(new AddressHelper { Address = m.ResidentialAddress, Id = m.EmergencyContactId });
                //        myList2.Add(new MobileNumberHelper { MobileNumber = m.MobileNumber, Id = m.EmergencyContactId });
                //    });

                //    myList.Add(new AddressHelper { Address = regObj.ResidentialAddress, Id = myList.Count + 1 });
                //    myList2.Add(new MobileNumberHelper { MobileNumber = regObj.MobileNumber, Id = myList2.Count + 1 });

                //    var distinctAdd = myList.Distinct(new AddressComparer());

                //    var distinctAdd2 = myList2.Distinct(new MobileComparer());

                //    if (staffEmerContacts.Count + 1 != distinctAdd.Count() ||
                //        staffEmerContacts.Count + 1 != distinctAdd2.Count())
                //    {
                //        response.Status.Message.FriendlyMessage =
                //            "Duplicate Error! At least one Residential Address Or Mobile number already exist";
                //        response.Status.Message.TechnicalMessage =
                //            "Duplicate Error! At least one Residential Address Or Mobile number already exist";
                //        return response;
                //    }


                thisCurrentEmerContact.StaffId = staffInfo.StaffId;
                thisCurrentEmerContact.LastName = regObj.LastName;
                thisCurrentEmerContact.FirstName = regObj.FirstName;
                thisCurrentEmerContact.MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName;
                thisCurrentEmerContact.LocalAreaOfLocationId = regObj.LocalAreaOfLocationId;
                thisCurrentEmerContact.StateOfLocationId = regObj.StateOfLocationId;
                thisCurrentEmerContact.MobileNumber = regObj.MobileNumber;
                thisCurrentEmerContact.Gender = (Gender) regObj.Gender;
                //}
                //else
                //{
                //    thisCurrentEmerContact.StaffId = staffInfo.StaffId;
                //    thisCurrentEmerContact.LastName = regObj.LastName;
                //    thisCurrentEmerContact.FirstName = regObj.FirstName;
                //    thisCurrentEmerContact.MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName;
                //    thisCurrentEmerContact.LocalAreaOfLocationId = regObj.LocalAreaOfLocationId;
                //    thisCurrentEmerContact.StateOfLocationId = regObj.StateOfLocationId;
                //    thisCurrentEmerContact.MobileNumber = regObj.MobileNumber;
                //    thisCurrentEmerContact.Gender = (Gender)regObj.Gender;
                //}


                var added = _emergencyContactRepository.Update(thisCurrentEmerContact);
                _uoWork.SaveChanges();
                if (added.EmergencyContactId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.EmergencyContactId;
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

        public EmergencyContactRespObj LoadEmergencyContact(EmergencyContactSearchObj searchObj)
        {
            var response = new EmergencyContactRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var thisEmergencyContact = GetStaffEmergencyContactsInfos(searchObj);
                if (thisEmergencyContact == null || !thisEmergencyContact.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Emergency Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Emergency Contact  Information found!";
                    return response;
                }


                var emergencyContactItems = new List<EmergencyContactObj>();

                thisEmergencyContact.ForEachx(m =>
                {
                    emergencyContactItems.Add(new EmergencyContactObj
                    {
                        EmergencyContactId = m.EmergencyContactId,
                        StaffId = m.StaffId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Gender = (int) m.Gender,
                        GenderLabel = m.Gender.ToString().Replace("_", " "),
                        ResidentialAddress = m.ResidentialAddress,
                        MobileNumber = m.MobileNumber,
                        StateOfOriginId = m.StateOfLocationId,
                        LocalAreaId = m.LocalAreaOfLocationId,
                        IsDefault = m.IsDefault
                    });
                });

                response.Status.IsSuccessful = true;
                response.EmergencyContacts = emergencyContactItems;
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


        public EmergencyContactRespObj LoadEmergencyContactByAdmin(EmergencyContactSearchObj searchObj)
        {
            var response = new EmergencyContactRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisEmergencyContact = GetStaffEmergencyContactsInfos(searchObj);
                if (thisEmergencyContact == null || !thisEmergencyContact.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Emergency Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Emergency Contact  Information found!";
                    return response;
                }


                var emergencyContactItems = new List<EmergencyContactObj>();

                thisEmergencyContact.ForEachx(m =>
                {
                    emergencyContactItems.Add(new EmergencyContactObj
                    {
                        EmergencyContactId = m.EmergencyContactId,
                        StaffId = m.StaffId,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Gender = (int) m.Gender,
                        GenderLabel = m.Gender.ToString().Replace("_", " "),
                        ResidentialAddress = m.ResidentialAddress,
                        MobileNumber = m.MobileNumber,
                        StateOfOriginId = m.StateOfLocationId,
                        LocalAreaId = m.LocalAreaOfLocationId,
                        IsDefault = m.IsDefault
                    });
                });

                response.Status.IsSuccessful = true;
                response.EmergencyContacts = emergencyContactItems;
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

        #endregion

        #region StaffMedical

        public SettingRegRespObj AddStaffMedical(RegStaffMedicalObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var staffMedical = new StaffMedical
                {
                    StaffId = staffInfo.StaffId,
                    BloodGroup = (BloodGroup) regObj.BloodGroup,
                    Genotype = (Genotype) regObj.Genotype,
                    KnownAilment = regObj.KnownAilment,
                    MedicalFitnessReport = regObj.MedicalFitnessReport,
                    TimeStampRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _staffMedicalRepository.Add(staffMedical);

                _uoWork.SaveChanges();

                if (added.StaffMedicalId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffMedicalId;
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

        public SettingRegRespObj UpdateStaffMedical(EditStaffMedicalObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var searchObj = new StaffMedicalSearchObj
                {
                    AdminUserId = 0,
                    StaffId = regObj.AdminUserId,
                    StaffMedicalId = 0,
                    SysPathCode = ""
                };

                //retrieve previous contacts and validate with the new one
                var thisStaffMedical = getStaffMedicals(searchObj);


                var thisCurrentStaffMedical = thisStaffMedical.Find(m => m.StaffId == staffInfo.StaffId);
                if (thisCurrentStaffMedical.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Medical Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Medical Information not found";
                    return response;
                }


                thisCurrentStaffMedical.BloodGroup = (BloodGroup) regObj.BloodGroup;
                thisCurrentStaffMedical.Genotype = (Genotype) regObj.Genotype;
                thisCurrentStaffMedical.KnownAilment = regObj.KnownAilment;
                thisCurrentStaffMedical.MedicalFitnessReport = regObj.MedicalFitnessReport;


                var updateItem = _staffMedicalRepository.Update(thisCurrentStaffMedical);
                _uoWork.SaveChanges();

                if (updateItem.StaffMedicalId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = updateItem.StaffMedicalId;
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

        public StaffMedicalRespObj LoadStaffMedical(StaffMedicalSearchObj searchObj)
        {
            var response = new StaffMedicalRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisStaffMedicals = getStaffMedicals(searchObj);
                if (thisStaffMedicals == null || !thisStaffMedicals.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Medical Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Medical Information found!";
                    return response;
                }


                var staffMedicalItems = new List<StaffMedicalObj>();
                thisStaffMedicals.ForEachx(m =>
                    staffMedicalItems.Add(new StaffMedicalObj
                    {
                        StaffId = m.StaffId,
                        StaffMedicalId = m.StaffMedicalId,
                        BloodGroup = (int) m.BloodGroup,
                        BloodGroupLabel = m.BloodGroup.ToString().Replace("_", " "),
                        Genotype = (int) m.Genotype,
                        GenotypeLabel = m.Genotype.ToString().Replace("_", " "),
                        KnownAilment = m.KnownAilment,
                        MedicalFitnessReport = m.MedicalFitnessReport,
                        TimeStampRegistered = m.TimeStampRegistered
                    }));


                response.Status.IsSuccessful = true;
                response.StaffMedicals = staffMedicalItems;
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

        public StaffMedicalRespObj LoadStaffMedicalByAdmin(StaffMedicalSearchObj searchObj)
        {
            var response = new StaffMedicalRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisStaffMedicals = getStaffMedicals(searchObj);
                if (thisStaffMedicals == null || !thisStaffMedicals.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Medical Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Medical Information found!";
                    return response;
                }


                var staffMedicalItems = new List<StaffMedicalObj>();
                thisStaffMedicals.ForEachx(m =>
                    staffMedicalItems.Add(new StaffMedicalObj
                    {
                        StaffId = m.StaffId,
                        StaffMedicalId = m.StaffMedicalId,
                        BloodGroup = (int) m.BloodGroup,
                        BloodGroupLabel = m.BloodGroup.ToString().Replace("_", " "),
                        Genotype = (int) m.Genotype,
                        GenotypeLabel = m.Genotype.ToString().Replace("_", " "),
                        KnownAilment = m.KnownAilment,
                        MedicalFitnessReport = m.MedicalFitnessReport,
                        TimeStampRegistered = m.TimeStampRegistered
                    }));


                response.Status.IsSuccessful = true;
                response.StaffMedicals = staffMedicalItems;
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

        #endregion


        #region Staff Account Number

        public SettingRegRespObj AddStaffBankAccount(RegStaffBankAccountObj regObj)
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

                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffBankAccountInfo = GetStaffBankAccountInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }

                //var searchObj = new StaffBankAccountSearchObj
                //{
                //    AdminUserId = 0,
                //    Status = -1,
                //    StaffId = regObj.StaffId,
                //    AccountName = "",
                //    StaffBankAccountId = 0,
                //    SysPathCode = "",
                //    BankId = 0
                //};
                var staffId = staffInfo.StaffId;

                //retrieve previous contacts and validate with the new one
                var staffAccInfos = getStaffBankAccInfo(staffId);


                if (staffAccInfos != null)
                {
                    var defCount = staffAccInfos.Count(m => m.IsDefault);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Only 1 Default Bank Account is allowed";
                        response.Status.Message.TechnicalMessage = "Only 1 Default Bank Account is allowed";
                        return response;
                    }

                    if (defCount > 0 && regObj.IsDefault)
                    {
                        response.Status.Message.FriendlyMessage =
                            "There is already a Default Bank Account specified! Kindly change to none-default ";
                        response.Status.Message.TechnicalMessage =
                            "There is already a Default Bank Account specified! Kindly change to none-default ";
                        return response;
                    }

                    var myList = new List<BankAccountHelper>();
                    staffAccInfos.ForEachx(m =>
                    {
                        myList.Add(new BankAccountHelper
                        {
                            BankAccountNumber = m.AccountNumber,
                            Id = m.StaffBankAccountId
                        });
                    });

                    myList.Add(new BankAccountHelper {BankAccountNumber = regObj.AccountNumber, Id = myList.Count + 1});
                    var distinctAdd = myList.Distinct(new BankAccountComparer());

                    if (staffAccInfos.Count + 1 != distinctAdd.Count())
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! At least one Bank Account Number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! At least one Bank Account Number already exist";
                        return response;
                    }
                }

                var bankAccountInfo = new StaffBankAccount
                {
                    StaffId = regObj.StaffId,
                    AccountName = regObj.AccountName,
                    AccountNumber = regObj.AccountNumber,
                    BankId = regObj.BankId,
                    Status = (ItemStatus) regObj.Status,
                    IsDefault = regObj.IsDefault,
                    TimeStamRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _staffBankAccRepository.Add(bankAccountInfo);
                _uoWork.SaveChanges();
                if (added.StaffBankAccountId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffBankAccountId;
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

        public SettingRegRespObj UpdateStaffBankAccount(EditStaffBankAccountObj regObj)
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
                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                //var searchObj = new StaffBankAccountSearchObj
                //{
                //    AdminUserId = 0,
                //    Status = -1,
                //    StaffId = regObj.AdminUserId,
                //    AccountName = "",
                //    StaffBankAccountId = 0,
                //    SysPathCode = "",
                //    BankId = 0
                //};
                var staffId = staffInfo.StaffId;

                //retrieve previous contacts and validate with the new one
                var staffAccInfos = getStaffBankAccInfo(staffId);
                if (staffAccInfos == null || !staffAccInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No previous Staff Bank account Information found";
                    response.Status.Message.TechnicalMessage = "No previous Staff Bank account Information found";
                    return response;
                }
                var thisCurrentBankAccount =
                    staffAccInfos.Find(m => m.StaffBankAccountId == regObj.StaffBankAccountId);

                if (thisCurrentBankAccount.StaffBankAccountId < 1 && thisCurrentBankAccount.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Bank account Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Bank account Information not found";
                    return response;
                }

                var defItems = staffAccInfos.FindAll(m => m.IsDefault);
                if (defItems.Count > 1)
                {
                    response.Status.Message.FriendlyMessage = "Only 1 Default Bank account is allowed";
                    response.Status.Message.TechnicalMessage = "Only 1 Default Bank account is allowed";
                    return response;
                }

                if (defItems.Count > 0 && regObj.IsDefault &&
                    defItems[0].StaffBankAccountId != regObj.StaffBankAccountId)
                {
                    response.Status.Message.FriendlyMessage =
                        "There is already a Default Bank account specified! Kindly change to none-default ";
                    response.Status.Message.TechnicalMessage =
                        "There is already a Default Bank account specified! Kindly change to none-default ";
                    return response;
                }


                if (staffAccInfos.Count > 1)
                {
                    var myList = new List<BankAccountHelper>();


                    staffAccInfos.ForEachx(m =>
                    {
                        myList.Add(new BankAccountHelper
                        {
                            BankAccountNumber = m.AccountNumber,
                            Id = m.StaffBankAccountId
                        });
                    });

                    myList.Add(new BankAccountHelper {BankAccountNumber = regObj.AccountNumber, Id = myList.Count + 1});


                    var distinctAdd = myList.Distinct(new BankAccountComparer());


                    if (staffAccInfos.Count + 1 != distinctAdd.Count())
                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error! At least one Bank account Number  already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error! At least one Bank account Number already exist";
                        return response;
                    }


                    thisCurrentBankAccount.StaffId = staffInfo.StaffId;
                    thisCurrentBankAccount.AccountName = regObj.AccountName;
                    thisCurrentBankAccount.AccountNumber = regObj.AccountNumber;
                    thisCurrentBankAccount.BankId = regObj.BankId;
                    thisCurrentBankAccount.IsDefault = regObj.IsDefault;
                    thisCurrentBankAccount.Status = (ItemStatus) regObj.Status;
                }
                else
                {
                    thisCurrentBankAccount.StaffId = staffInfo.StaffId;
                    thisCurrentBankAccount.AccountName = regObj.AccountName;
                    thisCurrentBankAccount.AccountNumber = regObj.AccountNumber;
                    thisCurrentBankAccount.BankId = regObj.BankId;
                    thisCurrentBankAccount.IsDefault = regObj.IsDefault;
                    thisCurrentBankAccount.Status = (ItemStatus) regObj.Status;
                }

                var added = _staffBankAccRepository.Update(thisCurrentBankAccount);
                _uoWork.SaveChanges();
                if (added.StaffBankAccountId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffBankAccountId;
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

        public StaffBankAccountRespObj LoadStaffBankAccount(StaffBankAccountSearchObj searchObj)
        {
            var response = new StaffBankAccountRespObj
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

                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thiStaffAccInfos = getStaffBankAccInfos(searchObj);
                if (thiStaffAccInfos == null || !thiStaffAccInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Bank Account Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Bank Account  Information found!";
                    return response;
                }


                var staffBankAccItems = new List<StaffBankAccountObj>();

                thiStaffAccInfos.ForEachx(m =>
                {
                    staffBankAccItems.Add(new StaffBankAccountObj
                    {
                        StaffBankAccountId = m.StaffBankAccountId,
                        StaffId = m.StaffId,
                        BankId = m.BankId,
                        BankName = new BankRepository().GetBank(m.BankId).Name,
                        AccountName = m.AccountName,
                        AccountNumber = m.AccountNumber,
                        IsDefault = m.IsDefault,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStamRegistered = m.TimeStamRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffBankAccounts = staffBankAccItems;
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


        public StaffBankAccountRespObj LoadStaffBankAccountByAdmin(StaffBankAccountSearchObj searchObj)
        {
            var response = new StaffBankAccountRespObj
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

                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thiStaffAccInfos = getStaffBankAccInfos(searchObj);
                if (thiStaffAccInfos == null || !thiStaffAccInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Bank Account Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Bank Account  Information found!";
                    return response;
                }


                var staffBankAccItems = new List<StaffBankAccountObj>();

                thiStaffAccInfos.ForEachx(m =>
                {
                    staffBankAccItems.Add(new StaffBankAccountObj
                    {
                        StaffBankAccountId = m.StaffBankAccountId,
                        StaffId = m.StaffId,
                        BankId = m.BankId,
                        BankName = new BankRepository().GetBank(m.BankId).Name,
                        AccountName = m.AccountName,
                        AccountNumber = m.AccountNumber,
                        IsDefault = m.IsDefault,
                        Status = (int) m.Status,
                        StatusLabel = m.Status.ToString().Replace("_", " "),
                        TimeStamRegistered = m.TimeStamRegistered
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffBankAccounts = staffBankAccItems;
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

        #endregion

        #region Staff_Next_Of_Kin

        public SettingRegRespObj AddStaffNextOfKin(RegStaffNextOfKinObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                if (IsStaffNextOfKinDuplicate(regObj.LastName, regObj.FirstName, regObj.MiddleName, regObj.Email,
                    regObj.MobileNumber, regObj.Landphone, 1, ref response))
                    return response;

                var nextOfKin = new StaffNextOfKin
                {
                    StaffId = regObj.AdminUserId,
                    FirstName = regObj.FirstName,
                    LastName = regObj.LastName,
                    MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName,
                    Gender = (Gender) regObj.Gender,
                    MaritalStatus = (MaritalStatus) regObj.MaritalStatus,
                    Relationship = (NextOfKinRelationship) regObj.Relationship,
                    Email = regObj.Email,
                    MobileNumber = regObj.MobileNumber,
                    Landphone = regObj.Landphone,
                    ResidentialAddress = regObj.ResidentialAddress,
                    LocalAreaOfLocationId = regObj.LocalAreaOfLocationId,
                    StateOfLocationId = regObj.StateOfLocationId,
                    TimeStampRegister = DateMap.CurrentTimeStamp()
                };

                var added = _staffNextOfKinRepository.Add(nextOfKin);
                _uoWork.SaveChanges();
                if (added.StaffNextOfKinId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffNextOfKinId;
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

        public SettingRegRespObj UpdateStaffNextOfKin(EditStaffNextOfKinObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                if (IsStaffNextOfKinDuplicate(regObj.LastName, regObj.FirstName, regObj.MiddleName, regObj.Email,
                    regObj.MobileNumber, regObj.Landphone, 2, ref response))
                    return response;

                var thisNextOfKin = getStaffNextInfoInfo(staffInfo.StaffId);


                thisNextOfKin.FirstName = regObj.FirstName;
                thisNextOfKin.LastName = regObj.LastName;
                thisNextOfKin.MiddleName = string.IsNullOrEmpty(regObj.MiddleName) ? "" : regObj.MiddleName;
                thisNextOfKin.Gender = (Gender) regObj.Gender;
                thisNextOfKin.MaritalStatus = (MaritalStatus) regObj.MaritalStatus;
                thisNextOfKin.Relationship = (NextOfKinRelationship) regObj.Relationship;
                thisNextOfKin.Email = regObj.Email;
                thisNextOfKin.MobileNumber = regObj.MobileNumber;
                thisNextOfKin.Landphone = regObj.Landphone;
                thisNextOfKin.ResidentialAddress = regObj.ResidentialAddress;
                thisNextOfKin.LocalAreaOfLocationId = regObj.LocalAreaOfLocationId;
                thisNextOfKin.StateOfLocationId = regObj.StateOfLocationId;


                var retVal = _staffNextOfKinRepository.Update(thisNextOfKin);
                _uoWork.SaveChanges();
                if (retVal.StaffNextOfKinId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.StaffNextOfKinId;
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

        public StaffNextOfKinRespObj LoadStaffNextOfKin(StaffNextOfKinSearchObj searchObj)
        {
            var response = new StaffNextOfKinRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisStaffNextOfKins = getStaffNextOfKinInfos(searchObj);

                if (thisStaffNextOfKins == null || !thisStaffNextOfKins.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffNextOfKinItems = new List<StaffNextOfKinObj>();

                thisStaffNextOfKins.ForEachx(m =>
                {
                    staffNextOfKinItems.Add(new StaffNextOfKinObj
                    {
                        StaffNextOfKinId = m.StaffNextOfKinId,
                        StaffId = m.StaffId,
                        StaffName = new StaffRepository().getStaffInfo(m.StaffId).FirstName + " " +
                                    new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Gender = (int) m.Gender,
                        GenderLabel = m.Gender.ToString(),
                        Email = m.Email,
                        MobileNumber = m.MobileNumber,
                        Landphone = m.Landphone,
                        MaritalStatus = (int) m.MaritalStatus,
                        MaritalStatusLabel = m.MaritalStatus.ToString(),
                        Relationship = (int) m.Relationship,
                        RelationshipLabel = m.Relationship.ToString(),
                        ResidentialAddress = m.ResidentialAddress,
                        StateOfLocationId = m.StateOfLocationId,
                 
                        LocalAreaOfLocationId = m.LocalAreaOfLocationId,
                    
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffNextOfKins = staffNextOfKinItems;
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

        public StaffNextOfKinRespObj LoadStaffNextOfKinByAdmin(StaffNextOfKinSearchObj searchObj)
        {
            var response = new StaffNextOfKinRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisStaffNextOfKins = getStaffNextOfKinInfos(searchObj);

                if (thisStaffNextOfKins == null || !thisStaffNextOfKins.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffNextOfKinItems = new List<StaffNextOfKinObj>();

                thisStaffNextOfKins.ForEachx(m =>
                {
                    staffNextOfKinItems.Add(new StaffNextOfKinObj
                    {
                        StaffNextOfKinId = m.StaffNextOfKinId,
                        StaffId = m.StaffId,
                        StaffName = new StaffRepository().getStaffInfo(m.StaffId).FirstName + " " +
                                    new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        Gender = (int) m.Gender,
                        GenderLabel = m.Gender.ToString(),
                        Email = m.Email,
                        MobileNumber = m.MobileNumber,
                        Landphone = m.Landphone,
                        MaritalStatus = (int) m.MaritalStatus,
                        MaritalStatusLabel = m.MaritalStatus.ToString(),
                        Relationship = (int) m.Relationship,
                        RelationshipLabel = m.Relationship.ToString(),
                        ResidentialAddress = m.ResidentialAddress,
                        StateOfLocationId = m.StateOfLocationId,
                      
                        LocalAreaOfLocationId = m.LocalAreaOfLocationId,
                      
                        TimeStampRegister = m.TimeStampRegister
                    });
                });

                response.Status.IsSuccessful = true;
                response.StaffNextOfKins = staffNextOfKinItems;
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

        #endregion

        #region Educational Qualification

        public SettingRegRespObj AddEducationalQualification(RegEducationalQualificationObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffInfo = getHigherEducationInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }

                if (regObj.StartYear > regObj.EndYear)
                {
                    response.Status.Message.FriendlyMessage = "Start year cannot be greater than end year";
                    response.Status.Message.TechnicalMessage = "Start year cannot be greater than end year";
                    return response;
                }

                // IsStaffEducationalQualificationDuplicate
                if (IsStaffEducationalQualificationDuplicate(staffInfo.StaffId, regObj.InstitutionId,
                    regObj.QualificationId, regObj.DisciplineId, regObj.CourseOfStudyId, regObj.StartYear,
                    regObj.EndYear, 1, ref response))
                    return response;
                var staffQualification = new HigherEducation
                {
                    StaffId = staffInfo.StaffId,
                    InstitutionId = regObj.InstitutionId,
                    QualificationId = regObj.QualificationId,
                    DisciplineId = regObj.DisciplineId,
                    ClassOfAwardId = regObj.ClassOfAwardId,
                    CourseOfStudyId = regObj.CourseOfStudyId,
                    CGPA = regObj.CGPA,
                    StartYear = regObj.StartYear,
                    EndYear = regObj.EndYear,
                    GradeScale = regObj.GradeScale,
                    SpecifiedDiscipline = new DisciplineRepository().GetDiscipline(regObj.DisciplineId).Name,
                    SpecifiedInstitution = new InstitutionRepository().GetInstitution(regObj.InstitutionId).Name,
                    SpecifiedCourseOfStudy =
                        new CourseOfStudyRepository().GetCourseOfStudy(regObj.CourseOfStudyId).Name,
                    TimeStampRegistered = DateMap.CurrentTimeStamp(),
                    TimeStampLastEdited = DateMap.CurrentTimeStamp()
                };

                var added = _staffHigherEducationRepository.Add(staffQualification);
                _uoWork.SaveChanges();
                if (added.HigherEducationId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.HigherEducationId;
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

        public SettingRegRespObj UpdateEducationalQualification(EditEducationalQualificationObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var searchObj = new EducationalQualificationSearchObj
                {
                    AdminUserId = 0,
                    InstitutionId = 0,
                    QualificationId = 0,
                    CourseOfStudyId = 0,
                    ClassOfAwardId = 0,
                    StaffId = regObj.AdminUserId,
                    HigherEducationId = 0,
                    SysPathCode = "",
                    DisciplineId = 0
                };

                //retrieve previous contacts and validate with the new one
                var staffEducationalQualification = getHigherEducationInfos(searchObj);
                if (staffEducationalQualification == null || !staffEducationalQualification.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "No previous Staff Educational Qualification Information found";
                    response.Status.Message.TechnicalMessage =
                        "No previous Staff Educational Qualification Information found";
                    return response;
                }
                var thisCurrentStaffQuEducation =
                    staffEducationalQualification.Find(m => m.HigherEducationId == regObj.HigherEducationId);
                if (thisCurrentStaffQuEducation.HigherEducationId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "This Staff Educational Qualification Information not found";
                    response.Status.Message.TechnicalMessage =
                        "This Staff Educational Qualification Information not found";
                    return response;
                }

                if (IsStaffEducationalQualificationDuplicate(staffInfo.StaffId, regObj.InstitutionId,
                    regObj.QualificationId, regObj.DisciplineId, regObj.CourseOfStudyId, regObj.StartYear,
                    regObj.EndYear, 2, ref response))
                    return response;

                thisCurrentStaffQuEducation.InstitutionId = regObj.InstitutionId;
                thisCurrentStaffQuEducation.QualificationId = regObj.QualificationId;
                thisCurrentStaffQuEducation.DisciplineId = regObj.DisciplineId;
                thisCurrentStaffQuEducation.ClassOfAwardId = regObj.ClassOfAwardId;
                thisCurrentStaffQuEducation.CourseOfStudyId = regObj.CourseOfStudyId;
                thisCurrentStaffQuEducation.CGPA = regObj.CGPA;
                thisCurrentStaffQuEducation.StartYear = regObj.StartYear;
                thisCurrentStaffQuEducation.EndYear = regObj.EndYear;
                thisCurrentStaffQuEducation.GradeScale = regObj.GradeScale;
                thisCurrentStaffQuEducation.SpecifiedDiscipline =
                    new DisciplineRepository().GetDiscipline(regObj.DisciplineId).Name;
                thisCurrentStaffQuEducation.SpecifiedInstitution =
                    new InstitutionRepository().GetInstitution(regObj.InstitutionId).Name;
                thisCurrentStaffQuEducation.SpecifiedCourseOfStudy =
                    new CourseOfStudyRepository().GetCourseOfStudy(regObj.CourseOfStudyId).Name;
                thisCurrentStaffQuEducation.TimeStampLastEdited = DateMap.CurrentTimeStamp();

                var retVal = _staffHigherEducationRepository.Update(thisCurrentStaffQuEducation);
                _uoWork.SaveChanges();
                if (retVal.HigherEducationId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.HigherEducationId;
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

        public EducationalQualificationRespObj LoadEducationalQualification(EducationalQualificationSearchObj searchObj)
        {
            var response = new EducationalQualificationRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisStaffEducationInfoss = getHigherEducationInfos(searchObj);

                if (thisStaffEducationInfoss == null || !thisStaffEducationInfoss.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffEducationalQualifications = new List<EducationalQualificationObj>();

                thisStaffEducationInfoss.ForEachx(m =>
                {
                    staffEducationalQualifications.Add(new EducationalQualificationObj
                    {
                        HigherEducationId = m.HigherEducationId,
                        StaffId = m.StaffId,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,

                        InstitutionId = m.InstitutionId,
                        SpecifiedInstitution = new InstitutionRepository().GetInstitution(m.InstitutionId).Name,

                        CourseOfStudyId = m.CourseOfStudyId,
                        SpecifiedCourseOfStudy = new CourseOfStudyRepository().GetCourseOfStudy(m.CourseOfStudyId).Name,

                        QualificationId = m.QualificationId,
                        QualificationLabel = new QualificationRepository().GetQualification(m.QualificationId).Name,

                        DisciplineId = m.DisciplineId,
                        SpecifiedDiscipline = new DisciplineRepository().GetDiscipline(m.DisciplineId).Name,

                        ClassOfAwardId = m.ClassOfAwardId,
                        ClassOfAwardLabel = new ClassOfAwardRepository().GetClassOfAward(m.ClassOfAwardId).Name,

                        CGPA = m.CGPA,
                        GradeScale = m.GradeScale,
                        StartYear = m.StartYear,
                        EndYear = m.EndYear,
                        TimeStampRegistered = m.TimeStampRegistered,
                        TimeStampLastEdited = m.TimeStampLastEdited
                    });
                });

                response.Status.IsSuccessful = true;
                response.EducationalQualifications = staffEducationalQualifications;
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


        public EducationalQualificationRespObj LoadEducationalQualificationByAdmin(
            EducationalQualificationSearchObj searchObj)
        {
            var response = new EducationalQualificationRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisStaffEducationInfoss = getHigherEducationInfos(searchObj);

                if (thisStaffEducationInfoss == null || !thisStaffEducationInfoss.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Contact Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Contact  Information found!";
                    return response;
                }


                var staffEducationalQualifications = new List<EducationalQualificationObj>();

                thisStaffEducationInfoss.ForEachx(m =>
                {
                    staffEducationalQualifications.Add(new EducationalQualificationObj
                    {
                        HigherEducationId = m.HigherEducationId,
                        StaffId = m.StaffId,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,

                        InstitutionId = m.InstitutionId,
                        SpecifiedInstitution = new InstitutionRepository().GetInstitution(m.InstitutionId).Name,

                        CourseOfStudyId = m.CourseOfStudyId,
                        SpecifiedCourseOfStudy = new CourseOfStudyRepository().GetCourseOfStudy(m.CourseOfStudyId).Name,

                        QualificationId = m.QualificationId,
                        QualificationLabel = new QualificationRepository().GetQualification(m.QualificationId).Name,

                        DisciplineId = m.DisciplineId,
                        SpecifiedDiscipline = new DisciplineRepository().GetDiscipline(m.DisciplineId).Name,

                        ClassOfAwardId = m.ClassOfAwardId,
                        ClassOfAwardLabel = new ClassOfAwardRepository().GetClassOfAward(m.ClassOfAwardId).Name,

                        CGPA = m.CGPA,
                        GradeScale = m.GradeScale,
                        StartYear = m.StartYear,
                        EndYear = m.EndYear,
                        TimeStampRegistered = m.TimeStampRegistered,
                        TimeStampLastEdited = m.TimeStampLastEdited
                    });
                });

                response.Status.IsSuccessful = true;
                response.EducationalQualifications = staffEducationalQualifications;
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

        #endregion

        #region ProfessionalMembership Body

        public SettingRegRespObj AddProfessionalMembership(RegProfessionalMemberShipObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                //var staffInfo = GetStaffProfessionalMembershipInfo(regObj.AdminUserId);
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }

                //var searchObj = new ProfessionalMemberShipSearchObj
                //{
                //    AdminUserId = 0,
                //    ProfessionalMembershipId = 0,
                //    ProfessionalMembershipTypeId = 0,
                //    StaffId = regObj.AdminUserId,
                //    ProfessionalBodyId = 0,
                //    SysPathCode = "",

                //};
                var staffId = staffInfo.StaffId;
                //retrieve previous contacts and validate with the new one
                var staffProfessionalMembership = GetStaffProfmeMembershipsInfo(staffId);
                if (staffProfessionalMembership != null && staffProfessionalMembership.Any())
                {
                    var defCount =
                        staffProfessionalMembership.Count(m =>
                            m.StaffId == staffInfo.StaffId &&
                            m.ProfessionalMembershipTypeId == regObj.ProfessionalMembershipTypeId &&
                            m.ProfessionalBodyId == regObj.ProfessionalBodyId);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Professional Membership already exist";
                        response.Status.Message.TechnicalMessage = "Professional Membership already exist";
                        return response;
                    }
                }
                var professionalBody = new ProfessionalMembership
                {
                    StaffId = staffInfo.StaffId,
                    ProfessionalMembershipTypeId = regObj.ProfessionalMembershipTypeId,
                    ProfessionalBodyId = regObj.ProfessionalBodyId,
                    YearJoined = regObj.YearJoined,
                    TimeStampRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _staffProfessionalMembershipRepository.Add(professionalBody);
                _uoWork.SaveChanges();
                if (added.ProfessionalMembershipId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = added.ProfessionalMembershipId;
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

        public SettingRegRespObj UpdateProfessionalMembership(EditProfessionalMemberShipObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                //var searchObj = new ProfessionalMemberShipSearchObj
                //{
                //    AdminUserId = 0,
                //    ProfessionalMembershipId = 0,
                //    ProfessionalMembershipTypeId = 0,
                //    StaffId = staffInfo.StaffId,
                //    ProfessionalBodyId = 0,
                //    SysPathCode = "",
                //};

                //retrieve previous contacts and validate with the new one
                var staffId = staffInfo.StaffId;
                //retrieve previous contacts and validate with the new one
                var staffProfessionalMembership = GetStaffProfmeMembershipsInfo(staffId);
                if (staffProfessionalMembership == null || !staffProfessionalMembership.Any())
                {
                    response.Status.Message.FriendlyMessage =
                        "No previous Staff Professional Membership Information found";
                    response.Status.Message.TechnicalMessage =
                        "No previous Staff Professional Membership Information found";
                    return response;
                }
                var thisCurrentProfessionalMembership = staffProfessionalMembership.Find(m =>
                    m.ProfessionalMembershipId == regObj.ProfessionalMembershipId);
                if (thisCurrentProfessionalMembership.ProfessionalMembershipId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "This Staff Professional Membership Information not found";
                    response.Status.Message.TechnicalMessage =
                        "This Staff Professional Membership Information not found";
                    return response;
                }
                var defCount =
                    staffProfessionalMembership.Count(m =>
                        m.StaffId == regObj.AdminUserId &&
                        m.ProfessionalMembershipTypeId == regObj.ProfessionalMembershipTypeId &&
                        m.ProfessionalBodyId == regObj.ProfessionalBodyId);
                if (defCount > 1)
                {
                    response.Status.Message.FriendlyMessage = "Professional Membership already exist";
                    response.Status.Message.TechnicalMessage = "Professional Membership already exist";
                    return response;
                }


                thisCurrentProfessionalMembership.StaffId = staffInfo.StaffId;
                thisCurrentProfessionalMembership.ProfessionalMembershipId = regObj.ProfessionalMembershipTypeId;
                thisCurrentProfessionalMembership.ProfessionalMembershipTypeId = regObj.ProfessionalMembershipTypeId;
                thisCurrentProfessionalMembership.ProfessionalBodyId = regObj.ProfessionalBodyId;
                thisCurrentProfessionalMembership.YearJoined = regObj.YearJoined;

                var retVal = _staffProfessionalMembershipRepository.Update(thisCurrentProfessionalMembership);
                _uoWork.SaveChanges();
                if (retVal.ProfessionalMembershipId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }
                response.Status.IsSuccessful = true;
                response.SettingId = retVal.ProfessionalMembershipId;
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

        public ProfessionalMembershipRespObj LoadProfessionalMembership(ProfessionalMemberShipSearchObj searchObj)
        {
            var response = new ProfessionalMembershipRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;
                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                var thisProfessionalMembershipInfoss = getProfessionalMembershipInfos(searchObj);

                if (thisProfessionalMembershipInfoss == null || !thisProfessionalMembershipInfoss.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Professional Membership Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Professional Membership  Information found!";
                    return response;
                }


                var professionalMemberships = new List<ProfessionalMembershipObj>();

                thisProfessionalMembershipInfoss.ForEachx(m =>
                {
                    professionalMemberships.Add(new ProfessionalMembershipObj
                    {
                        ProfessionalMembershipId = m.ProfessionalMembershipId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        ProfessionalBodyId = m.ProfessionalBodyId,
                        ProfessionalBody =
                            new ProfessionalBodyRepository().GetProfessionalBody(m.ProfessionalBodyId).Name,
                        ProfessionalMembershipTypeId = m.ProfessionalMembershipTypeId,
                        ProfessionalMembershipType = new ProfessionalMemshipTypeRepository()
                            .GetProfessionalMembershipType(m.ProfessionalMembershipTypeId).Name,
                        YearJoined = m.YearJoined
                    });
                });

                response.Status.IsSuccessful = true;
                response.ProfessionalMemberships = professionalMemberships;
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


        public ProfessionalMembershipRespObj LoadProfessionalMembershipByAdmin(
            ProfessionalMemberShipSearchObj searchObj)
        {
            var response = new ProfessionalMembershipRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisProfessionalMembershipInfoss = getProfessionalMembershipInfos(searchObj);

                if (thisProfessionalMembershipInfoss == null || !thisProfessionalMembershipInfoss.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Professional Membership Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Professional Membership  Information found!";
                    return response;
                }


                var professionalMemberships = new List<ProfessionalMembershipObj>();

                thisProfessionalMembershipInfoss.ForEachx(m =>
                {
                    professionalMemberships.Add(new ProfessionalMembershipObj
                    {
                        ProfessionalMembershipId = m.ProfessionalMembershipId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        ProfessionalBodyId = m.ProfessionalBodyId,
                        ProfessionalBody =
                            new ProfessionalBodyRepository().GetProfessionalBody(m.ProfessionalBodyId).Name,
                        ProfessionalMembershipTypeId = m.ProfessionalMembershipTypeId,
                        ProfessionalMembershipType = new ProfessionalMemshipTypeRepository()
                            .GetProfessionalMembershipType(m.ProfessionalMembershipTypeId).Name,
                        YearJoined = m.YearJoined
                    });
                });

                response.Status.IsSuccessful = true;
                response.ProfessionalMemberships = professionalMemberships;
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

        #endregion

        #region Staff Insurance

        public SettingRegRespObj AddStaffInsurance(RegStaffInsuranceObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                //var searchObj = new StaffInsuranceSearchObj
                //{
                //    AdminUserId = 0,
                //    Status = -1,
                //    StaffId = regObj.AdminUserId,
                //    StaffInsuranceId = 0,
                //    SysPathCode = "",

                //};
                var staffId = staffInfo.StaffId;
                //retrieve previous contacts and validate with the new one
                var staffInsurances = getStafInsuranceInfo(staffId);
                if (staffInsurances != null && staffInsurances.Any())
                {
                    var defCount =
                        staffInsurances.Count(m =>
                            m.StaffId == staffInfo.StaffId && m.InsurancePolicyTypeId == regObj.InsurancePolicyTypeId);
                    if (defCount > 1)
                    {
                        response.Status.Message.FriendlyMessage = "Staff Insurance Infomation already exist";
                        response.Status.Message.TechnicalMessage = "Staff Insurance Infomation already exist";
                        return response;
                    }
                    var myList = new List<InsuranceHelper>();

                    staffInsurances.ForEachx(m =>
                    {
                        myList.Add(new InsuranceHelper {PolicyNumber = m.PolicyNumber, Id = m.StaffInsuranceId});
                    });

                    myList.Add(new InsuranceHelper {PolicyNumber = regObj.PolicyNumber, Id = myList.Count + 1});


                    var distinctAdd = myList.Distinct(new InsuaranceComparer());


                    if (staffInsurances.Count + 1 != distinctAdd.Count())

                    {
                        response.Status.Message.FriendlyMessage =
                            "Duplicate Error!  The Policy Number already exist";
                        response.Status.Message.TechnicalMessage =
                            "Duplicate Error!  The Policy Number already exist";
                        return response;
                    }
                }

                var staffInsurance = new StaffInsurance
                {
                    StaffId = regObj.AdminUserId,
                    InsurancePolicyTypeId = regObj.InsurancePolicyTypeId,
                    Insurer = regObj.Insurer,
                    PolicyNumber = regObj.PolicyNumber,
                    CompanyContibution = regObj.CompanyContibution,
                    PersonalContibution = regObj.PersonalContibution,
                    CommencementDate = string.IsNullOrEmpty(regObj.CommencementDate) ? "" : regObj.CommencementDate,
                    TerminationDate = string.IsNullOrEmpty(regObj.TerminationDate) ? "" : regObj.TerminationDate,
                    Status = (ItemStatus) regObj.Status,
                    TimeStamRegistered = DateMap.CurrentTimeStamp()
                };

                var added = _staffInsuranceRepository.Add(staffInsurance);

                _uoWork.SaveChanges();

                if (added.StaffInsuranceId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffInsuranceId;
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

        public SettingRegRespObj UpdateStaffInsurance(EditStaffInsuranceObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }
                //var searchObj = new StaffInsuranceSearchObj
                //{
                //    AdminUserId = 0,
                //    Status = -1,
                //    StaffId = regObj.AdminUserId,
                //    StaffInsuranceId = 0,
                //    SysPathCode = "",

                //};
                var staffId = staffInfo.StaffId;
                //retrieve previous contacts and validate with the new one
                var staffInsurances = getStafInsuranceInfo(staffId);

                if (staffInsurances == null || !staffInsurances.Any())
                {
                    response.Status.Message.FriendlyMessage = "This Staff Insurance Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Insurance Information not found";
                    return response;
                }

                var defCount =
                    staffInsurances.Count(m =>
                        m.StaffId == staffInfo.StaffId && m.InsurancePolicyTypeId == regObj.InsurancePolicyTypeId);
                if (defCount > 1)
                {
                    response.Status.Message.FriendlyMessage = "Staff Insurance Infomation already exist";
                    response.Status.Message.TechnicalMessage = "Staff Insurance Infomation already exist";
                    return response;
                }

                var thisCurrentStaffInsurance = staffInsurances.Find(m => m.StaffId == regObj.StaffId);
                if (thisCurrentStaffInsurance.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Insurance Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Insurance Information not found";
                    return response;
                }

                var myList = new List<InsuranceHelper>();

                staffInsurances.ForEachx(m =>
                {
                    myList.Add(new InsuranceHelper {PolicyNumber = m.PolicyNumber, Id = m.StaffInsuranceId});
                });

                myList.Add(new InsuranceHelper {PolicyNumber = regObj.PolicyNumber, Id = myList.Count + 1});


                var distinctAdd = myList.Distinct(new InsuaranceComparer());


                if (staffInsurances.Count + 1 != distinctAdd.Count())

                {
                    response.Status.Message.FriendlyMessage =
                        "Duplicate Error!  The Pension Number already exist";
                    response.Status.Message.TechnicalMessage =
                        "Duplicate Error!  The Pension Number already exist";
                    return response;
                }

                thisCurrentStaffInsurance.InsurancePolicyTypeId = regObj.InsurancePolicyTypeId;
                thisCurrentStaffInsurance.Insurer = regObj.Insurer;
                thisCurrentStaffInsurance.PolicyNumber = regObj.PolicyNumber;
                thisCurrentStaffInsurance.CompanyContibution = regObj.CompanyContibution;
                thisCurrentStaffInsurance.PersonalContibution = regObj.PersonalContibution;
                thisCurrentStaffInsurance.CommencementDate =
                    string.IsNullOrEmpty(regObj.CommencementDate) ? "" : regObj.CommencementDate;
                thisCurrentStaffInsurance.TerminationDate =
                    string.IsNullOrEmpty(regObj.TerminationDate) ? "" : regObj.TerminationDate;
                thisCurrentStaffInsurance.Status = (ItemStatus) regObj.Status;
                var added = _staffInsuranceRepository.Update(thisCurrentStaffInsurance);

                _uoWork.SaveChanges();

                if (added.StaffInsuranceId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffInsuranceId;
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

        public StaffInsuranceRespObj LoadStaffInsurances(StaffInsuranceSearchObj searchObj)
        {
            var response = new StaffInsuranceRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP,
                    false, out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Invalid Staff Information";
                    response.Status.Message.TechnicalMessage = "Invalid Staff Information";
                    return response;
                }

                var thisStaffInsurances = getStaffInsuranceInfos(searchObj);
                if (thisStaffInsurances == null || !thisStaffInsurances.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Insurances Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Insurances Information found!";
                    return response;
                }


                var staffInsuranceItems = new List<StaffInsuranceObj>();
                thisStaffInsurances.ForEachx(m => staffInsuranceItems.Add(new StaffInsuranceObj
                {
                    StaffInsuranceId = m.StaffInsuranceId,
                    StaffId = m.StaffId,
                    FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                    LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                    MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                    InsurancePolicyTypeId = m.InsurancePolicyTypeId,
                    InsurancePolicyType = new InsurancePolicyTypeRepository()
                        .GetInsurancePolicyType(m.InsurancePolicyTypeId).Name,
                    PolicyNumber = m.PolicyNumber,
                    Insurer = m.Insurer,
                    CommencementDate = m.CommencementDate,
                    TerminationDate = m.TerminationDate,
                    PersonalContibution = m.PersonalContibution,
                    CompanyContibution = m.CompanyContibution,
                    Status = (int) m.Status,
                    StatusLabel = m.Status.ToString().Replace("_", " ")
                }));


                response.Status.IsSuccessful = true;
                response.StaffInsurances = staffInsuranceItems;
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


        public StaffInsuranceRespObj LoadStaffInsurancesByAdmin(StaffInsuranceSearchObj searchObj)
        {
            var response = new StaffInsuranceRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;

                var thisStaffInsurances = getStaffInsuranceInfos(searchObj);
                if (thisStaffInsurances == null || !thisStaffInsurances.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Insurances Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Insurances Information found!";
                    return response;
                }


                var staffInsuranceItems = new List<StaffInsuranceObj>();
                thisStaffInsurances.ForEachx(m => staffInsuranceItems.Add(new StaffInsuranceObj
                {
                    StaffInsuranceId = m.StaffInsuranceId,
                    StaffId = m.StaffId,
                    FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                    LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                    MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                    InsurancePolicyTypeId = m.InsurancePolicyTypeId,
                    InsurancePolicyType = new InsurancePolicyTypeRepository()
                        .GetInsurancePolicyType(m.InsurancePolicyTypeId).Name,
                    PolicyNumber = m.PolicyNumber,
                    Insurer = m.Insurer,
                    CommencementDate = m.CommencementDate,
                    TerminationDate = m.TerminationDate,
                    PersonalContibution = m.PersonalContibution,
                    CompanyContibution = m.CompanyContibution,
                    Status = (int) m.Status,
                    StatusLabel = m.Status.ToString().Replace("_", " ")
                }));


                response.Status.IsSuccessful = true;
                response.StaffInsurances = staffInsuranceItems;
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

        #endregion

        #region StaffJobInfo

        public SettingRegRespObj AddStaffJobInfo(RegStaffJobInfoObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;

                if (staffInfo == null || staffInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "Unable to retrieve Staff Information";
                    response.Status.Message.TechnicalMessage = "Unable to retrieve Staff Information";
                    return response;
                }
                if (IsStaffJobInfoDuplicate(staffInfo.StaffId, 1, ref response))
                    return response;

                var staffJobInfo = new StaffJobInfo
                {
                    StaffId = staffInfo.StaffId,
                    EntranceCompanyId = regObj.EntranceCompanyId,
                    CurrentCompanyId = regObj.CurrentCompanyId,
                    EntranceDepartmentId = regObj.EntranceDepartmentId,
                    CurrentDepartmentId = regObj.CurrentDepartmentId,
                    JobTypeId = regObj.JobTypeId,
                    JobPositionId = regObj.JobPositionId,
                    JobLevelId = regObj.JobLevelId,
                    JobSpecializationId = regObj.JobSpecializationId,
                    JobTitle = regObj.JobTitle,
                    JobDescription = regObj.JobDescription,
                    TimeStampRegistered = DateMap.CurrentTimeStamp(),
                    SalaryGradeId = regObj.SalaryGradeId,
                    SalaryLevelId = regObj.SalaryLevelId,
                    LineManagerId = regObj.LineManagerId,
                    TeamLeadId = regObj.TeamLeadId
                };

                var added = _staffJobInfoRepository.Add(staffJobInfo);

                _uoWork.SaveChanges();

                if (added.StaffJobInfoId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = added.StaffJobInfoId;
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

        public SettingRegRespObj UpdateStaffJobInfo(EditStaffJobInfoObj regObj)
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


                if (!HelperMethods.IsStaffUserValid(regObj.AdminUserId, regObj.SysPathCode, regObj.LoginIP, false,
                    out var staffInfo, ref response.Status.Message))
                    return response;


                var searchObj = new StaffJobInfoSearchObj
                {
                    AdminUserId = 0,
                    StaffId = regObj.AdminUserId,
                    StaffJobInfoId = 0,
                    SysPathCode = ""
                };

                //retrieve previous contacts and validate with the new one
                var thisStaffInfos = getStaffJobInfos(searchObj);


                var thisCurrentStaffJobInfo = thisStaffInfos.Find(m => m.StaffId == staffInfo.StaffId);
                if (thisCurrentStaffJobInfo.StaffId < 1)
                {
                    response.Status.Message.FriendlyMessage = "This Staff Job Information not found";
                    response.Status.Message.TechnicalMessage = "This Staff Job Information not found";
                    return response;
                }


                thisCurrentStaffJobInfo.EntranceCompanyId = regObj.EntranceCompanyId;
                thisCurrentStaffJobInfo.CurrentCompanyId = regObj.CurrentCompanyId;
                thisCurrentStaffJobInfo.EntranceDepartmentId = regObj.EntranceDepartmentId;
                thisCurrentStaffJobInfo.CurrentDepartmentId = regObj.CurrentDepartmentId;
                thisCurrentStaffJobInfo.JobTypeId = regObj.JobTypeId;
                thisCurrentStaffJobInfo.JobPositionId = regObj.JobPositionId;
                thisCurrentStaffJobInfo.JobLevelId = regObj.JobLevelId;
                thisCurrentStaffJobInfo.JobSpecializationId = regObj.JobSpecializationId;
                thisCurrentStaffJobInfo.JobTitle = regObj.JobTitle;
                thisCurrentStaffJobInfo.JobDescription = regObj.JobDescription;
                thisCurrentStaffJobInfo.SalaryGradeId = regObj.SalaryGradeId;
                thisCurrentStaffJobInfo.SalaryLevelId = regObj.SalaryLevelId;
                thisCurrentStaffJobInfo.LineManagerId = regObj.LineManagerId;
                thisCurrentStaffJobInfo.TeamLeadId = regObj.TeamLeadId;

                var updateItem = _staffJobInfoRepository.Update(thisCurrentStaffJobInfo);
                _uoWork.SaveChanges();

                if (updateItem.StaffJobInfoId < 1)
                {
                    response.Status.Message.FriendlyMessage =
                        "Error Occurred! Unable to complete your request. Please try again later";
                    response.Status.Message.TechnicalMessage = "Unable to save to database";
                    return response;
                }

                response.Status.IsSuccessful = true;
                response.SettingId = updateItem.StaffJobInfoId;
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

        public StaffJobInfoRespObj LoadStaffJobInfos(StaffJobInfoSearchObj searchObj)
        {
            var response = new StaffJobInfoRespObj
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


                if (!HelperMethods.IsStaffUserValid(searchObj.AdminUserId, searchObj.SysPathCode, searchObj.LoginIP, false, out var staffInfo, ref response.Status.Message))return response;
                var thisStaffJobInfos = getStaffJobInfos(searchObj);
                if (thisStaffJobInfos == null || !thisStaffJobInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Job Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Job Information found!";
                    return response;
                }


                var staffJobInfoItems = new List<StaffJobInfoObj>();
                thisStaffJobInfos.ForEachx(m =>
                    staffJobInfoItems.Add(new StaffJobInfoObj
                    {
                        StaffJobInfoId = m.StaffJobInfoId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        EntranceCompanyId = m.EntranceCompanyId,
                        EntranceCompany = new CompanyRepository().GetCompany(m.EntranceCompanyId).Name,
                        CurrentCompanyId = m.CurrentCompanyId,
                        CurrentCompany = new CompanyRepository().GetCompany(m.CurrentCompanyId).Name,
                        EntranceDepartmentId = m.EntranceDepartmentId,
                        EntranceDepartment = new DepartmentRepository().GetDepartment(m.EntranceDepartmentId).Name,
                        CurrentDepartmentId = m.CurrentDepartmentId,
                        CurrentDepartment = new DepartmentRepository().GetDepartment(m.CurrentDepartmentId).Name,
                        JobTypeId = m.JobTypeId,
                        JobType = new JobTypeRepository().GetJobType(m.JobTypeId).Name,
                        JobLevelId = m.JobLevelId,
                        JobLevel = new JobLevelRepository().GetJobLevel(m.JobLevelId).Name,
                        JobPositionId = m.JobPositionId,
                        JobPosition = new JobPositionRepository().GetJobPosition(m.JobPositionId).Name,
                        JobSpecializationId = m.JobSpecializationId,
                        JobSpecialization =
                            new JobSpecializationRepository().GetJobSpecialization(m.JobSpecializationId).Name,
                        JobTitle = m.JobTitle,
                        JobDescription = m.JobDescription,
                        SalaryGradeId = m.SalaryGradeId,
                        SalaryGrade = new SalaryGradeRepository().GetSalaryGrade(m.SalaryGradeId).Name,
                        SalaryLevelId = m.SalaryLevelId,
                        SalaryLevel = new SalaryLevelRepository().GetSalaryLevel(m.SalaryLevelId).Name,
                        TeamLeadId = m.TeamLeadId,
                        TeamLead = new StaffRepository().getStaffInfo(m.TeamLeadId).FirstName ,
                        LineManagerId = m.LineManagerId,
                        LineManager = new StaffRepository().getStaffInfo(m.LineManagerId).FirstName,
                        TimeStampRegistered = m.TimeStampRegistered
                    }));


                response.Status.IsSuccessful = true;
                response.StaffJobInfos = staffJobInfoItems;
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


        public StaffJobInfoRespObj LoadStaffJobInfosByAdmin(StaffJobInfoSearchObj searchObj)
        {
            var response = new StaffJobInfoRespObj
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


                if (!HelperMethods.IsUserValid(searchObj.AdminUserId, searchObj.SysPathCode,
                    HelperMethods.getStaffRoles(), ref response.Status.Message))
                    return response;
                var thisStaffJobInfos = getStaffJobInfos(searchObj);
                if (thisStaffJobInfos == null || !thisStaffJobInfos.Any())
                {
                    response.Status.Message.FriendlyMessage = "No Staff Job Information found!";
                    response.Status.Message.TechnicalMessage = "No Staff Job Information found!";
                    return response;
                }


                var staffJobInfoItems = new List<StaffJobInfoObj>();
                thisStaffJobInfos.ForEachx(m =>
                    staffJobInfoItems.Add(new StaffJobInfoObj
                    {
                        StaffJobInfoId = m.StaffJobInfoId,
                        StaffId = m.StaffId,
                        FirstName = new StaffRepository().getStaffInfo(m.StaffId).FirstName,
                        LastName = new StaffRepository().getStaffInfo(m.StaffId).LastName,
                        MiddleName = new StaffRepository().getStaffInfo(m.StaffId).MiddleName,
                        EntranceCompanyId = m.EntranceCompanyId,
                        EntranceCompany = new CompanyRepository().GetCompany(m.EntranceCompanyId).Name,
                        CurrentCompanyId = m.CurrentCompanyId,
                        CurrentCompany = new CompanyRepository().GetCompany(m.CurrentCompanyId).Name,
                        EntranceDepartmentId = m.EntranceDepartmentId,
                        EntranceDepartment = new DepartmentRepository().GetDepartment(m.EntranceDepartmentId).Name,
                        CurrentDepartmentId = m.CurrentDepartmentId,
                        CurrentDepartment = new DepartmentRepository().GetDepartment(m.CurrentDepartmentId).Name,
                        JobTypeId = m.JobTypeId,
                        JobType = new JobTypeRepository().GetJobType(m.JobTypeId).Name,
                        JobLevelId = m.JobLevelId,
                        JobLevel = new JobLevelRepository().GetJobLevel(m.JobLevelId).Name,
                        JobPositionId = m.JobPositionId,
                        JobPosition = new JobPositionRepository().GetJobPosition(m.JobPositionId).Name,
                        JobSpecializationId = m.JobSpecializationId,
                        JobSpecialization =
                            new JobSpecializationRepository().GetJobSpecialization(m.JobSpecializationId).Name,
                        JobTitle = m.JobTitle,
                        JobDescription = m.JobDescription,
                        SalaryGradeId = m.SalaryGradeId,
                        SalaryGrade = new SalaryGradeRepository().GetSalaryGrade(m.SalaryGradeId).Name,
                        SalaryLevelId = m.SalaryLevelId,
                        SalaryLevel = new SalaryLevelRepository().GetSalaryLevel(m.SalaryLevelId).Name,
                        TeamLeadId = m.TeamLeadId,
                        TeamLead = new StaffRepository().getStaffInfo(m.TeamLeadId).FirstName,
                        LineManagerId = m.LineManagerId,
                        LineManager = new StaffRepository().getStaffInfo(m.LineManagerId).FirstName,
                        TimeStampRegistered = m.TimeStampRegistered
                    }));


                response.Status.IsSuccessful = true;
                response.StaffJobInfos = staffJobInfoItems;
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

        #endregion
    }
}