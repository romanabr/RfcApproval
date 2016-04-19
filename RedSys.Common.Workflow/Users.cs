using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using RedSys.RFC.Core.Helper;

namespace RedSys.Common.Workflow
{
    public class Users
    {
        public static void GetUsers(SPWeb web, SPListItem item, Step cStep, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            if (cStep.Role.GetRoleVal().Count == 0) return;
            foreach (SPFieldLookupValue role in cStep.Role.GetRoleVal())
            {
                ParseRole(web, cStep, item, role, AllUsersList, CurrentUsers);
            }
        }

        public static bool ParseRole(SPWeb web, Step cStep, SPListItem item, SPFieldLookupValue role, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            bool hasUsers = cStep.IfUserNameEmpty;
            int roleId = role.LookupId;
            BranchInfo userinfo = null;
            userinfo = new BranchInfo();
            userinfo.RoleName = role.LookupValue;
            userinfo.UserStep = cStep;
            userinfo.StartDate = DateTime.Now;
            SPUser user = null;
            if (role.LookupValue == "Запустивший процесс")
            {
                try
                {
                    user = item.GetFieldValueUser(SPBuiltInFieldId.Author);
                    userinfo.User.UserName = cStep.GetDeputy(web, user);
                    userinfo.User.RealUserName = user.LoginName;
                    bool wasAgreed = false;
                    foreach (BranchInfo br in AllUsersList)
                    {
                        if (br.Approved &&
                            br.UserStep.ID == userinfo.UserStep.ID &&
                            br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                        if (userinfo.UserStep.RepeatedUserAgreement &&
                            br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                    }
                    if (!wasAgreed)
                        CurrentUsers.Add(userinfo);
                    hasUsers = true;
                }
                catch
                {
                    userinfo.User.UserName = string.Empty;
                    userinfo.User.RealUserName = string.Empty;
                }
            }
            else if (role.LookupValue == "Автор документа")
            {
                try
                {
                    user = item.GetFieldValueUser(SPBuiltInFieldId.Author);
                    userinfo.User.RealUserName = user.LoginName;
                    userinfo.User.UserName = cStep.GetDeputy(web, user);
                    bool wasAgreed = false;
                    foreach (BranchInfo br in AllUsersList)
                    {
                        if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                        if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                    }
                    if (!wasAgreed)
                        CurrentUsers.Add(userinfo);
                    hasUsers = true;
                }
                catch
                {
                    userinfo.User.UserName = string.Empty;
                    userinfo.User.RealUserName = string.Empty;
                }
            }
            else if (role.LookupValue == "Из поля элемента")
            {
                try
                {
                    SPFieldUserValueCollection users = new SPFieldUserValueCollection(web, item[cStep.CardField].ToString());
                    userinfo.User.UserName = "";
                    foreach (SPFieldUserValue usr in users)
                    {
                        user = usr.User;
                        if (userinfo.User.UserName == "")
                        {
                            userinfo.User.RealUserName += user.LoginName;
                            userinfo.User.UserName += cStep.GetDeputy(web, user);
                            bool wasAgreed = false;
                            foreach (BranchInfo br in AllUsersList)
                            {
                                if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                    wasAgreed = true;
                                if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                    wasAgreed = true;
                            }
                            if (!wasAgreed)
                                CurrentUsers.Add(userinfo);
                            hasUsers = true;
                        }
                        else
                        {
                            userinfo = new BranchInfo();
                            userinfo.RoleName = role.LookupValue;
                            userinfo.UserStep = cStep;
                            userinfo.User.RealUserName += user.LoginName;
                            userinfo.User.UserName += cStep.GetDeputy(web, user);
                            bool wasAgreed = false;
                            foreach (BranchInfo br in AllUsersList)
                            {
                                if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                    wasAgreed = true;
                                if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                    wasAgreed = true;
                            }
                            if (!wasAgreed)
                                CurrentUsers.Add(userinfo);
                        }
                    }
                }
                catch
                {
                    userinfo.User.UserName = string.Empty;
                    userinfo.User.RealUserName = string.Empty;
                }
            }
            else if (role.LookupValue == "Из списка задач")
            {
                SPList list = item.Web.GetListExt("/Lists/RfcKEApproveTaskList");
                SPQuery query = new SPQuery();
                query.Query =
                    string.Format(
                        "<Where><And><Eq><FieldRef Name='RFCUserType' /><Value Type='Choice'>Cогласующий</Value></Eq><Eq><FieldRef Name='RFCKeLink'  LookupId='True' /><Value Type='Integer'>{0}</Value></Eq></And></Where>",
                        item.ID);
                SPListItemCollection lic = list.GetItems(query);
                if(lic!=null && lic.Count>0)
                    foreach (SPListItem listItem in lic)
                    { 
                        SPUser keManager = listItem.GetFieldValueUser("KeManager");
                        if(keManager != null)
                        
                            hasUsers = ParseUserField(web, string.Format("{0};#{1}", keManager.ID, keManager.Name), userinfo, cStep, hasUsers, AllUsersList, CurrentUsers);
                        }
                   
            }
            else if (role.LookupValue == "Руководитель ответственного")
            {
                user = item.GetFieldValueUser(cStep.CardField);
                hasUsers = GetRespBoss(user, web, item, cStep, AllUsersList, CurrentUsers);
            }
            else if (role.LookupValue == "Руководители инициатора")
            {
                user = item.GetFieldValueUser(cStep.CardField);
                hasUsers = GetBoss(user, web, item, cStep, AllUsersList, CurrentUsers);
            }
            else if (role.LookupValue == "Руководители инициатора по лимитам")
            {
                user = item.GetFieldValueUser(cStep.CardField);
                hasUsers = GetBossLimit(user, web, item, cStep, AllUsersList, CurrentUsers);
            }
            else if (role.LookupValue == "Роль из поля элемента")
            {
                SPFieldLookupValueCollection roles = item.GetFieldValueLookupMulti(cStep.CardField);
                foreach (SPFieldLookupValue role2 in roles)
                {
                    hasUsers = ParseRole(web, cStep, item, role2, AllUsersList, CurrentUsers);
                }
            }
            else
            {
                SPList roleList = web.Lists[Constant.RoleName];
                SPListItem itemRole = roleList.GetItemById(roleId); //from Role List Instance

                SPList roleDistributionList = web.Lists[Constant.RoleDistributionName];
                var query = new SPQuery();


                List<string> fieldList = new List<string>();
                if (itemRole[Constant.RoleKind] != null && !string.IsNullOrEmpty(itemRole.GetFieldValue(Constant.RoleKind)))
                {
                    SPFieldMultiChoice RoleKind = (SPFieldMultiChoice)itemRole.Fields[Constant.RoleKind];

                    foreach (string str in RoleKind.Choices)
                    {
                        if (itemRole[Constant.RoleKind].ToString().Contains(str))
                            fieldList.Add(str);
                    }
                }

                SPListItemCollection roleCol = null;
                if (fieldList.Count > 0)
                {
                    string eqStr = Constant.Role_Name + "=" + role.LookupValue;
                    foreach (string str in fieldList)
                    {
                        if (item.ParentList.Fields.ContainsField(str) && item[str] != null)
                        {
                            SPField f = item.ParentList.Fields[str];
                            if (f.Type == SPFieldType.Lookup || f.TypeAsString == "lookupfieldwithpicker" || f.TypeAsString.ToLower() == "ensollookup")
                            {
                                SPFieldLookupValue v = new SPFieldLookupValue(item[str].ToString());
                                eqStr += ";|" + str + "=" + v.LookupId;
                            }
                            else
                            {
                                eqStr += ";" + str + "=" + item[str].ToString();
                            }
                        }
                    }
                    roleCol = Helper.GetItemsByValue(web, roleDistributionList.Title,
                        eqStr);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Where>");
                    sb.Append(string.Format("<Eq><FieldRef Name='{0}' LookupId='TRUE' /><Value Type='Lookup'>{1}</Value></Eq>", roleDistributionList.Fields[Constant.Role_Name].InternalName, roleId));
                    sb.Append("</Where>");
                    query.Query = sb.ToString();
                }
                string person = string.Empty;
                try
                {
                    if (roleCol == null)
                        roleCol = roleDistributionList.GetItems(query);
                    if (roleCol != null && roleCol.Count != 0)
                    {
                        userinfo.User.UserName = "";
                        foreach (SPListItem oItem in roleCol)
                        {
                            user = oItem.GetFieldValueUser(Constant.AD);
                            hasUsers = ParseUserField(web, oItem[Constant.AD].ToString(), userinfo, cStep, hasUsers, AllUsersList, CurrentUsers);
                        }
                    }
                    else if (cStep.IsDefaultUser)
                    {
                        query = new SPQuery();
                        query.Query = string.Format("<Where><And><Eq><FieldRef Name='{0}' LookupId='TRUE' /><Value Type='Lookup'>{1}</Value></Eq><Eq><FieldRef ID='{2}'/><Value Type='Boolean'>1</Value></Eq></And></Where>", roleDistributionList.Fields[Constant.Role_Name].InternalName, roleId, Constant.IsDefaultUser);
                        roleCol = null;
                        roleCol = roleDistributionList.GetItems(query);
                        if (roleCol != null && roleCol.Count != 0)
                        {
                            userinfo.User.UserName = "";
                            foreach (SPListItem oItem in roleCol)
                            {
                                hasUsers = ParseUserField(web, oItem[Constant.AD].ToString(), userinfo, cStep, hasUsers, AllUsersList, CurrentUsers);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            if (!hasUsers)
            {
                throw new Exception("В роли '" + role.LookupValue + "' не найдены исполнители!");
            }
            return hasUsers;
        }

        static public bool ParseUserField(SPWeb oWeb, string value, BranchInfo userinfo, Step cStep, bool hasUsers, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            SPFieldUserValue GrpOrUser = new SPFieldUserValue(oWeb, value);
            if (GrpOrUser.User != null)
            {
                if (string.IsNullOrEmpty(userinfo.User.UserName))
                {
                    userinfo.User.RealUserName += GrpOrUser.User.LoginName;
                    userinfo.User.UserName += cStep.GetDeputy(oWeb, GrpOrUser.User);
                    bool wasAgreed = false;
                    foreach (BranchInfo br in AllUsersList)
                    {
                        if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                        if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                            wasAgreed = true;
                    }
                    if (!wasAgreed)
                        CurrentUsers.Add(userinfo);
                    hasUsers = true;
                }
                else
                    userinfo.AdditionalUsers.Add(new UserInfo() { UserName = GrpOrUser.User.LoginName, RealUserName = cStep.GetDeputy(oWeb, GrpOrUser.User) });

            }
            else
            {
                SPGroup group = oWeb.SiteGroups.GetByID(GrpOrUser.LookupId);
                foreach (SPUser user in group.Users)
                {
                    if (userinfo.User.UserName == "")
                    {
                        userinfo.User.UserName += cStep.GetDeputy(oWeb, user);
                        userinfo.User.RealUserName += user.LoginName;
                        bool wasAgreed = false;
                        foreach (BranchInfo br in AllUsersList)
                        {
                            if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                            if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                        }
                        if (!wasAgreed)
                            CurrentUsers.Add(userinfo);
                        hasUsers = true;
                    }
                    else
                        userinfo.AdditionalUsers.Add(new UserInfo() { UserName = cStep.GetDeputy(oWeb, user), RealUserName = user.LoginName });
                }
            }
            return hasUsers;
        }

        public static string GetParDep(SPUser user, SPWeb web)
        {
            string depid = "";
            SPQuery oQuery = new SPQuery();
            SPList customUserList = web.Lists["Сотрудники"];
            string login = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;

            oQuery.ViewXml = "<View><Method Name='Чтение списков'><Filter Name='ADSearch' Value='" + login + "'/>" +
                "</Method><Query><OrderBy><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/>" +
                "</OrderBy><Where><Eq><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/><Value Type='Text'>" + login + "</Value>" +
                "</Eq></Where></Query><ViewFields><FieldRef Name='" + customUserList.Fields["Код подразделения"].InternalName + "'/>" +
                "<FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/></ViewFields><RowLimit>1</RowLimit></View>";
            SPListItemCollection users = customUserList.GetItems(oQuery);
            if (users.Count > 0)
            {
                SPListItem customuser = users[0];
                depid = customuser["Код подразделения"] == null ? "null" : customuser["Код подразделения"].ToString();
                SPList DepList = web.Lists["Подразделения"];
                SPQuery query = new SPQuery();

                StringBuilder sb = new StringBuilder();
                sb.Append("<Where>");
                sb.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq>", DepList.Fields["ID"].InternalName, depid));
                sb.Append("</Where>");
                query.Query = sb.ToString();
                SPListItemCollection deps = DepList.GetItems(query);
                if (deps.Count > 0)
                {
                    SPListItem dep = deps[0];
                    depid = dep["Родительское подразделение"].ToString();
                    if (depid == "0")
                        depid = "";

                }
                else
                    depid = "";
            }
            return depid;
        }

        public static SPUser GetDepBoss(string depid, SPWeb web, SPListItem item, Step cStep, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            SPUser boss = null;
            SPQuery query = new SPQuery();
            SPList bosslist = web.Lists["Руководители и лимиты"];
            StringBuilder sb = new StringBuilder();
            sb.Append("<Where>");
            sb.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq>", bosslist.Fields["Подразделение: ID"].InternalName, depid));
            sb.Append("</Where>");
            query.Query = sb.ToString();
            SPListItemCollection bosses = bosslist.GetItems(query);
            if (bosses.Count > 0)
            {
                SPListItem bossitem = bosses[0];
                SPFieldUserValue bossval = new SPFieldUserValue(web, bossitem["Руководитель подразделения"].ToString());
                boss = bossval.User;
                if (bossitem["Согласование"] != null && bool.Parse(bossitem["Согласование"].ToString()))
                {
                    SPUser initiator = item.GetFieldValueUser("Инициатор");
                    if (initiator.LoginName != boss.LoginName)
                    {
                        BranchInfo userinfo = null;
                        userinfo = new BranchInfo();
                        userinfo.RoleName = "Руководители инициатора";
                        userinfo.UserStep = cStep;
                        userinfo.User.RealUserName = boss.LoginName;
                        userinfo.User.UserName = cStep.GetDeputy(web, boss);
                        bool wasAgreed = false;
                        foreach (BranchInfo br in AllUsersList)
                        {
                            if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                            if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                        }
                        if (!wasAgreed)
                            CurrentUsers.Add(userinfo);
                    }
                }
            }
            return boss;
        }

        public static SPUser GetDepBossLimit(string depid, SPWeb web, SPListItem item, Step cStep, out bool limitmatch, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            limitmatch = false;
            SPUser boss = null;
            SPQuery query = new SPQuery();
            SPList bosslist = web.Lists["Руководители и лимиты"];
            StringBuilder sb = new StringBuilder();
            sb.Append("<Where>");
            sb.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq>", bosslist.Fields["Подразделение: ID"].InternalName, depid));
            sb.Append("</Where>");
            query.Query = sb.ToString();
            SPListItemCollection bosses = bosslist.GetItems(query);
            if (bosses.Count > 0)
            {
                SPListItem bossitem = bosses[0];
                SPFieldUserValue bossval = new SPFieldUserValue(web, bossitem["Руководитель подразделения"].ToString());
                boss = bossval.User;
                limitmatch = (bossitem["Лимит"] != null && double.Parse(bossitem["Лимит"].ToString()) >= double.Parse(item[cStep.ComparePattern].ToString()));
                if (limitmatch || (bossitem["Авторизация"].ToString() != null && bool.Parse(bossitem["Авторизация"].ToString())))
                {
                    SPUser initiator = item.GetFieldValueUser("Инициатор");
                    if (initiator.LoginName != boss.LoginName)
                    {
                        BranchInfo userinfo = null;
                        userinfo = new BranchInfo();
                        userinfo.RoleName = "Руководители инициатора";
                        userinfo.UserStep = cStep;
                        userinfo.User.RealUserName = boss.LoginName;
                        userinfo.User.UserName = cStep.GetDeputy(web, boss);
                        bool wasAgreed = false;
                        foreach (BranchInfo br in AllUsersList)
                        {
                            if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                            if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                wasAgreed = true;
                        }
                        if (!wasAgreed)
                            CurrentUsers.Add(userinfo);
                        if (limitmatch)
                            boss = null;
                    }
                }
            }
            return boss;
        }

        public static bool GetBossLimit(SPUser user, SPWeb web, SPListItem item, Step cStep, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            bool hasUsers = cStep.IfUserNameEmpty;
            SPUser boss = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPServiceContextScope scope = new Microsoft.SharePoint.SPServiceContextScope(SPServiceContext.GetContext(web.Site)))
                {
                    SPQuery oQuery = new SPQuery();
                    SPList customUserList = web.Lists["Сотрудники"];
                    StringBuilder sb = new StringBuilder();
                    string login = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;
                    oQuery.ViewXml = "<View><Method Name='Чтение списков'><Filter Name='ADSearch' Value='" + login + "'/>" +
                        "</Method><Query><OrderBy><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/>" +
                        "</OrderBy><Where><Eq><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/><Value Type='Text'>" + login + "</Value>" +
                        "</Eq></Where></Query><ViewFields><FieldRef Name='" + customUserList.Fields["Код подразделения"].InternalName + "'/>" +
                        "<FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/></ViewFields><RowLimit>1</RowLimit></View>";
                    SPListItemCollection users = customUserList.GetItems(oQuery);
                    if (users.Count > 0)
                    {
                        SPListItem customuser = users[0];
                        string depid = customuser["Код подразделения"] == null ? "null" : customuser["Код подразделения"].ToString();
                        if (depid != "null")
                        {
                            bool limitmatch = false;
                            boss = GetDepBossLimit(depid, web, item, cStep, out limitmatch, AllUsersList, CurrentUsers);
                            if (!limitmatch)
                            {
                                bool Completed = false;
                                do
                                {
                                    hasUsers = true;
                                    SPList DepList = web.Lists["Подразделения"];
                                    SPQuery query = new SPQuery();

                                    sb = new StringBuilder();
                                    sb.Append("<Where>");
                                    sb.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq>", DepList.Fields["ID"].InternalName, depid));
                                    sb.Append("</Where>");
                                    query.Query = sb.ToString();
                                    SPListItemCollection deps = DepList.GetItems(query);
                                    if (deps.Count > 0)
                                    {
                                        SPListItem dep = deps[0];
                                        depid = dep["Родительское подразделение"].ToString();
                                        if (depid == "0")
                                            depid = "";

                                    }
                                    else
                                        depid = "";

                                    if (depid != "")
                                    {
                                        boss = GetDepBossLimit(depid, web, item, cStep, out limitmatch, AllUsersList, CurrentUsers);
                                        if (boss == null)
                                            Completed = true;
                                    }
                                    else
                                    {
                                        Completed = true;
                                        if (CurrentUsers.Count == 0)
                                        {
                                            BranchInfo userinfo = null;
                                            userinfo = new BranchInfo();
                                            userinfo.RoleName = "Руководители инициатора";
                                            userinfo.UserStep = cStep;
                                            userinfo.StartDate = DateTime.Now;
                                            userinfo.User.RealUserName = user.LoginName;
                                            userinfo.User.UserName = cStep.GetDeputy(web, user);
                                            bool wasAgreed = false;
                                            foreach (BranchInfo br in AllUsersList)
                                            {
                                                if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                                    wasAgreed = true;
                                                if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                                    wasAgreed = true;
                                            }
                                            if (!wasAgreed)
                                            {
                                                CurrentUsers.Add(userinfo);
                                            }
                                        }
                                    }
                                }
                                while (!Completed);
                            }
                            else
                            {
                                hasUsers = true;
                            }
                        }
                    }
                }
            });
            return hasUsers;
        }

        public static bool GetRespBoss(SPUser user, SPWeb web, SPListItem item, Step cStep, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            bool hasUsers = cStep.IfUserNameEmpty;
            SPQuery oQuery = new SPQuery();
            SPList customUserList = web.Lists["Руководители ответственных"];
            StringBuilder sb = new StringBuilder();
            oQuery.Query = "<Where><Eq><FieldRef Name='" + customUserList.Fields["Ответственный"].InternalName +
                "' LookupId='True'/><Value Type='User'>" + user.ID + "</Value></Eq></Where>";
            SPListItemCollection users = customUserList.GetItems(oQuery);
            if (users.Count > 0)
            {
                SPListItem customuser = users[0];
                BranchInfo userinfo = new BranchInfo();
                userinfo.RoleName = "Руководители инициатора";
                userinfo.UserStep = cStep;
                userinfo.StartDate = DateTime.Now;
                hasUsers = ParseUserField(web, customuser["Руководитель ответственного"].ToString(), userinfo, cStep, hasUsers, AllUsersList, CurrentUsers);
            }
            return hasUsers;
        }

        public static bool GetBoss(SPUser user, SPWeb web, SPListItem item, Step cStep, List<BranchInfo> AllUsersList, List<BranchInfo> CurrentUsers)
        {
            bool hasUsers = cStep.IfUserNameEmpty;
            SPUser boss = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPServiceContextScope scope = new Microsoft.SharePoint.SPServiceContextScope(SPServiceContext.GetContext(web.Site)))
                {
                    SPQuery oQuery = new SPQuery();
                    SPList customUserList = web.Lists["Сотрудники"];
                    StringBuilder sb = new StringBuilder();
                    string login = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;
                    oQuery.ViewXml = "<View><Method Name='Чтение списков'><Filter Name='ADSearch' Value='" + login + "'/>" +
                        "</Method><Query><OrderBy><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/>" +
                        "</OrderBy><Where><Eq><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/><Value Type='Text'>" + login + "</Value>" +
                        "</Eq></Where></Query><ViewFields><FieldRef Name='" + customUserList.Fields["Код подразделения"].InternalName + "'/>" +
                        "<FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/></ViewFields><RowLimit>1</RowLimit></View>";
                    SPListItemCollection users = customUserList.GetItems(oQuery);
                    if (users.Count > 0)
                    {
                        SPListItem customuser = users[0];
                        string depid = customuser["Код подразделения"] == null ? "null" : customuser["Код подразделения"].ToString();
                        if (depid != "null")
                        {
                            boss = GetDepBoss(depid, web, item, cStep, AllUsersList, CurrentUsers);
                            bool Completed = false;
                            do
                            {
                                hasUsers = true;
                                SPList DepList = web.Lists["Подразделения"];
                                SPQuery query = new SPQuery();

                                sb = new StringBuilder();
                                sb.Append("<Where>");
                                sb.Append(string.Format("<Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq>", DepList.Fields["ID"].InternalName, depid));
                                sb.Append("</Where>");
                                query.Query = sb.ToString();
                                SPListItemCollection deps = DepList.GetItems(query);
                                if (deps.Count > 0)
                                {
                                    SPListItem dep = deps[0];
                                    depid = dep["Родительское подразделение"].ToString();
                                    if (depid == "0")
                                        depid = "";

                                }
                                else
                                    depid = "";

                                if (depid != "")
                                {
                                    boss = GetDepBoss(depid, web, item, cStep, AllUsersList, CurrentUsers);
                                    if (boss == null)
                                        Completed = true;
                                }
                                else
                                {
                                    Completed = true;
                                    if (CurrentUsers.Count == 0)
                                    {
                                        BranchInfo userinfo = null;
                                        userinfo = new BranchInfo();
                                        userinfo.RoleName = "Руководители инициатора";
                                        userinfo.UserStep = cStep;
                                        userinfo.User.RealUserName = user.LoginName;
                                        userinfo.User.UserName = cStep.GetDeputy(web, user);
                                        userinfo.StartDate = DateTime.Now;
                                        bool wasAgreed = false;
                                        foreach (BranchInfo br in AllUsersList)
                                        {
                                            if (br.Approved && !userinfo.UserStep.AgreementRepeated && br.UserStep.ID == userinfo.UserStep.ID && br.User.UserName == userinfo.User.UserName)
                                                wasAgreed = true;
                                            if (br.Approved && userinfo.UserStep.RepeatedUserAgreement && br.User.UserName == userinfo.User.UserName)
                                                wasAgreed = true;
                                        }
                                        if (!wasAgreed)
                                        {
                                            CurrentUsers.Add(userinfo);
                                        }
                                    }
                                }
                            }
                            while (!Completed);
                        }
                    }
                }
            });
            return hasUsers;
        }
    }
}
