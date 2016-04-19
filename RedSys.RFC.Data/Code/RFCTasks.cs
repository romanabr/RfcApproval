using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.Const;
using RedSys.RFC.Data.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Code
{
    public class RFCTasks
    {
        private SPWeb currentWeb;
        public List<SPUser> Users;
        private string UserType;
        private SPListItem currentListItem;

        public RFCTasks(SPWeb web, SPListItem rfcListItem)
        {
            currentWeb = web;
            Users = new List<SPUser>();
            UserType = string.Empty;
            currentListItem = rfcListItem;
        }

        public int Count()
        {
            int retInt = 0;
            SPList list = currentWeb.GetListExt(RFCLists.KeApproveTaskList.CustomUrl);
            SPQuery query = new SPQuery();
            query.ViewFields = "<FieldRef Name='ID' />";
            query.Query = string.Format("<Where><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><Eq><FieldRef Name='RFCUserType' /><Value Type='Text'>{1}</Value></Eq></And></Where>", currentListItem.ID, RFCUserTypeConst.APPROVER);
            SPListItemCollection lic = list.GetItems(query);
            if (lic != null)
                retInt = lic.Count;
            return retInt;
        }

        

        public void Approve(string username)
        {
            SPList list = currentWeb.GetListExt(RFCLists.KeApproveTaskList.CustomUrl);
            SPQuery query = new SPQuery();
            
            query.Query = string.Format("<Where><And><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><Eq><FieldRef Name='RFCKeType' /><Value Type='Text'>{1}</Value></Eq></And><Eq><FieldRef Name='RFCKeApprove' /><Value Type='Text'>{2}</Value></Eq></And></Where>", currentListItem.ID, RFCUserTypeConst.APPROVER, RFCTaskStatus.ONWORK);
            SPListItemCollection lic = list.GetItems(query);
            if (lic == null) return;
            foreach(SPListItem item in lic)
            {
                item[RFCFields.RFCKeApprove.InternalName] = RFCTaskStatus.APPROVEMANAGER;
                item[RFCFields.RFCKeApproveDate.InternalName] = DateTime.Now;
                item[RFCFields.RFCKeComment.InternalName] = "Согласовно принудительно менеджером " + username;
                item.Update();
            }
               
        }

        public RFCTasks(SPWeb web, List<SPUser> users, string userType, SPListItem rfcListItem)
        {
            currentWeb = web;
            Users = users;
            UserType = userType;
            currentListItem = rfcListItem;

        }
        public RFCTasks(SPWeb web, SPUser user, string userType, SPListItem rfcListItem)
        {
            currentWeb = web;
            Users = new List<SPUser> { user };
            UserType = userType;
            currentListItem = rfcListItem;
        }

        public RFCTasks(SPWeb web, string userType, SPListItem rfcListItem)
        {
            currentWeb = web;
            Users = new List<SPUser>();
            UserType = userType;
            currentListItem = rfcListItem;
        }

        public void CreateTasks()
        {
            SPList rfcKEList = currentWeb.GetListExt(RFCLists.RfcKeList.CustomUrl);
            SPQuery query = new SPQuery();
            query.Query = string.Format("<Where><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq></Where>", currentListItem.ID);
            SPListItemCollection rfKEListItems = rfcKEList.GetItems(query);
            if (rfKEListItems != null && rfKEListItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<Where><In><FieldRef Name='{0}' LookupId='True' /><Values>", RFCFields.KeToKeLink.InternalName);
                Dictionary<string, List<SPUser>> userDict = new Dictionary<string, List<SPUser>>();
                foreach (SPListItem keItems in rfKEListItems)
                {
                   SPFieldLookupValue kekeID = keItems.GetFieldValueLookup(RFCFields.KeToKeLink.InternalName);
                    sb.AppendFormat("<Value Type='Integer'>{0}</Value>", kekeID.LookupId);
                    userDict.Add(kekeID.ToString(), new List<SPUser>());
                }
                sb.Append("</Values></In></Where>");

                SPList userList = currentWeb.GetListExt(RFCLists.KEResponsibleList.CustomUrl);
                SPQuery userQuery = new SPQuery { Query = sb.ToString() };
                SPListItemCollection userCollection = userList.GetItems(userQuery);
                if (userCollection != null && userCollection.Count > 0)
                {
                    foreach (SPListItem userItem in userCollection)
                    {
                        SPUser user = userItem.GetFieldValueUser(RFCFields.KeManager.InternalName);
                        SPFieldLookupValue keLookup = userItem.GetFieldValueLookup(RFCFields.KeToKeLink.InternalName);
                        if (user != null)
                        {
                            Users.Add(user);
                            userDict[keLookup.ToString()].Add(user);
                        }
                    }
                    foreach(var x in userDict)
                         CreateRFCUsers(x);
                }
            }
        }

        public static List<SPUser> GetKEUsers(SPListItem item)
        {
            SPList list = item.Web.GetListExt(RFCLists.RfcUserList.CustomUrl);
            List<SPUser> retList = new List<SPUser>();
            SPQuery query = new SPQuery();
            query.Query = string.Format("<Where><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq></Where>", item.ID);
            SPListItemCollection existUser = list.GetItems(query);
            if (existUser == null || existUser.Count == 0)
            {
                foreach (SPListItem exist in existUser)
                {
                    SPUser user = exist.GetFieldValueUser(RFCFields.RFCUser.InternalName);
                    if (user != null)
                        retList.Add(user);
                }
            }
            return retList;
        }

        private void CreateRFCUsers(KeyValuePair<string,List<SPUser>> keusers)
        {
            SPList taskList = currentWeb.GetListExt(RFCLists.KeApproveTaskList.CustomUrl);
            SPFieldLookupValue keyLookupValue = new SPFieldLookupValue(keusers.Key  );
            foreach (SPUser user in keusers.Value)
            {
                SPQuery query = new SPQuery();
                query.Query = string.Format("<Where><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><And><Eq><FieldRef Name='KeManager' LookupId='True' /><Value Type='Integer'>{1}</Value></Eq><Eq><FieldRef Name='KeKeLink' LookupId='True' /><Value Type='Integer'>{2}</Value></Eq></And></And></Where>", currentListItem.ID, user.ID,keyLookupValue.LookupId);
                SPListItemCollection existUser = taskList.GetItems(query);
                if (existUser == null || existUser.Count == 0)
                {
                    SPListItem createUser = taskList.AddItem();
                    createUser[SPBuiltInFieldId.Title] = currentListItem.ID + "-" + user.ID;
                    createUser[RFCFields.KeManager.InternalName] = new SPFieldUserValue(currentWeb, user.ID, user.Name);
                    createUser[RFCFields.RFCUserType.InternalName] = RFCUserTypeConst.APPROVER;
                    createUser[RFCFields.RfcToKeLink.InternalName] = new SPFieldLookupValue(currentListItem.ID, currentListItem.Title);
                    createUser[RFCFields.KeToKeLink.InternalName] =keyLookupValue;
                    createUser.Update();
                }
            }
        }

        public void DeleteTasks()
        {
            SPList list = currentWeb.GetListExt(RFCLists.RfcUserList.CustomUrl);
            SPQuery query = new SPQuery();
            foreach (SPUser user in Users)
            {
                query.Query = string.Format("<Where><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><Eq><FieldRef Name='RFCUser' LookupId='True' /><Value Type='Integer'>{1}</Value></Eq></And></Where>", currentListItem.ID, user.ID);

                SPListItemCollection existUser = list.GetItems(query);
                if (existUser == null || existUser.Count > 0)
                {
                    foreach (SPListItem userItem in existUser)
                    {
                        userItem.Delete();
                    }
                }
            }
        }

       
        public void CompleteCurrentUserTask(SPUser currentUser,bool result, string comment)
        {
            if (currentUser == null) return;

            SPList taskList = currentWeb.GetListExt(RFCLists.KeApproveTaskList.CustomUrl);
           
                SPQuery query = new SPQuery();
                query.Query = string.Format("<Where><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><Eq><FieldRef Name='KeManager' LookupId='True' /><Value Type='Integer'>{1}</Value></Eq></And></Where>", currentListItem.ID, currentUser.ID);
                SPListItemCollection existUser = taskList.GetItems(query);
                if (existUser != null && existUser.Count != 0)
                {
                foreach (SPListItem listItem in existUser)
                {

                    listItem[RFCFields.RFCKeApprove.InternalName] = result == true ? RFCTaskStatus.APPROVE : RFCTaskStatus.DECLINE;
                    listItem[RFCFields.RFCKeComment.InternalName] = comment;
                    listItem[RFCFields.RFCKeApproveDate.InternalName] = DateTime.Now;
                    
                    listItem.Update();
                }
                }
            
        }
    }
}
