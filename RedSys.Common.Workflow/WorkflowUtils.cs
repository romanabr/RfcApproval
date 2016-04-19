using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{
    class WorkflowUtils
    {
        #region Permissions
        public static void SetPerm(BranchInfo Current, List<BranchInfo> Allusers, SPWeb web, SPListItem spli)
        {
            spli.Web.AllowUnsafeUpdates = true;
            if (spli.HasUniqueRoleAssignments)
                spli.ResetRoleInheritance();
            spli.BreakRoleInheritance(true);

            SPRoleAssignment cont = new SPRoleAssignment(spli.Web.AssociatedOwnerGroup);
            SPRoleDefinition role = web.RoleDefinitions.GetByType(SPRoleType.Administrator);
            cont.RoleDefinitionBindings.Add(role);
            spli.RoleAssignments.Add(cont);
            AddEditPerm(Current, web, spli);
            foreach (BranchInfo br in Allusers)
            {
                AddReadPerm(br, web, spli);
            }
        }

        public static void AddEditPerm(BranchInfo Current, SPWeb web, SPListItem spli)
        {
            if (Current.UserStep.Editpermission)
            {
                AEP(Current.User, web, spli);
                foreach (UserInfo add in Current.AdditionalUsers)
                {
                    AEP(add, web, spli);
                }
            }
            else
            {
                AddReadPerm(Current, web, spli);
            }
        }

        protected static void AEP(UserInfo ui, SPWeb web, SPListItem spli)
        {
            SPUser u = web.EnsureUser(ui.RealUserName);
            if (u != null)
            {
                SPRoleAssignment cont = new SPRoleAssignment(u);
                SPRoleDefinition role = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                cont.RoleDefinitionBindings.Add(role);
                spli.RoleAssignments.Add(cont);
            }
            if (ui.RealUserName != ui.UserName)
            {
                u = web.EnsureUser(ui.UserName);
                if (u != null)
                {
                    SPRoleAssignment cont = new SPRoleAssignment(u);
                    SPRoleDefinition role = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
                    cont.RoleDefinitionBindings.Add(role);
                    spli.RoleAssignments.Add(cont);
                }
            }
        }

        protected static void AddReadPerm(BranchInfo Current, SPWeb web, SPListItem spli)
        {
            ARP(Current.User, web, spli);
            foreach (UserInfo add in Current.AdditionalUsers)
            {
                ARP(add, web, spli);
            }
        }

        protected static void ARP(UserInfo ui, SPWeb web, SPListItem spli)
        {
            SPUser u = web.EnsureUser(ui.RealUserName);
            if (u != null)
            {
                SPRoleAssignment cont = new SPRoleAssignment(u);
                SPRoleDefinition role = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                cont.RoleDefinitionBindings.Add(role);
                spli.RoleAssignments.Add(cont);
            }
            if (ui.RealUserName != ui.UserName)
            {
                u = web.EnsureUser(ui.UserName);
                if (u != null)
                {
                    SPRoleAssignment cont = new SPRoleAssignment(u);
                    SPRoleDefinition role = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                    cont.RoleDefinitionBindings.Add(role);
                    spli.RoleAssignments.Add(cont);
                }
            }
        }
        #endregion

        #region UserFields
        public static SPFieldUserValueCollection FormUsersField(BranchInfo Current, SPWeb web)
        {
            SPFieldUserValueCollection curs = new SPFieldUserValueCollection();
            curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.UserName].ID, Current.User.UserName));
            if (Current.User.RealUserName != Current.User.UserName)
            {
                curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.RealUserName].ID, Current.User.RealUserName));
            }
            foreach (UserInfo usr in Current.AdditionalUsers)
            {
                curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.RealUserName].ID, usr.RealUserName));
                if (usr.UserName != usr.RealUserName)
                {
                    curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.UserName].ID, usr.UserName));
                }
            }
            return curs;
        }

        public static SPFieldUserValueCollection AddUsersField(SPFieldUserValueCollection curs, BranchInfo Current, SPWeb web)
        {
            curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.UserName].ID, Current.User.UserName));
            if (Current.User.UserName != Current.User.RealUserName)
            {
                curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.RealUserName].ID, Current.User.RealUserName));
            }
            if (Current.AdditionalUsers != null)
                foreach (UserInfo usr in Current.AdditionalUsers)
                {
                    curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.UserName].ID, usr.UserName));
                    if (usr.UserName != usr.RealUserName)
                    {
                        curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.RealUserName].ID, usr.RealUserName));
                    }
                }
            return curs;
        }

        internal static SPFieldUserValueCollection RemoveUsersField(SPFieldUserValueCollection cusers, int iD, SPWeb web)
        {

            for (int i = cusers.Count - 1; i >= 0; i--)
            {
                SPFieldUserValue val = cusers[i];
                if (val.User.ID == iD)
                    cusers.Remove(val);
               
            }
            return cusers;
        }

        public static SPFieldUserValueCollection AddUsersField(BranchInfo Current, SPWeb web)
        {
            SPFieldUserValueCollection curs = new SPFieldUserValueCollection();
            curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.UserName].ID, Current.User.UserName));
            if (Current.User.UserName != Current.User.RealUserName)
            {
                curs.Add(new SPFieldUserValue(web, web.SiteUsers[Current.User.RealUserName].ID, Current.User.RealUserName));
            }
            if (Current.AdditionalUsers != null)
                foreach (UserInfo usr in Current.AdditionalUsers)
                {
                    curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.UserName].ID, usr.UserName));
                    if (usr.UserName != usr.RealUserName)
                    {
                        curs.Add(new SPFieldUserValue(web, web.SiteUsers[usr.RealUserName].ID, usr.RealUserName));
                    }
                }
            return curs;
        }

        public static SPFieldUserValueCollection RemoveUsersField(SPFieldUserValueCollection curs, BranchInfo Current, SPWeb web)
        {
            for (int i = curs.Count - 1; i >= 0; i--)
            {
                SPFieldUserValue val = curs[i];
                if (val.User.LoginName == Current.User.UserName || val.User.LoginName == Current.User.RealUserName)
                    curs.Remove(val);
                foreach (UserInfo user in Current.AdditionalUsers)
                {
                    if (val.User.LoginName == user.RealUserName || val.User.LoginName == user.UserName)
                        curs.Remove(val);
                }
            }
            return curs;
        }
        #endregion

        #region Tasks
        public static void CreateTask(SPListItem item, BranchInfo bi)
        {
            if (bi.UserStep.CreateTask)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(item.Web.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList tasklist = web.Lists["Статистика задач"];
                            SPListItem taskitem = tasklist.AddItem();
                            taskitem[SPBuiltInFieldId.Title] = bi.UserStep.StageName;
                            taskitem["Дата начала"] = bi.StartDate;
                            taskitem["Срок исполнения"] = bi.DueDate;
                            taskitem["Документ ИД"] = item.ID;
                            taskitem["Библиотека"] = item.ParentList.Title;
                            SPFieldUserValueCollection val = FormUsersField(bi, web);
                            taskitem["Исполнители задачи"] = val;
                            SPFieldUrlValue uval = new SPFieldUrlValue();
                            uval.Description = item[SPBuiltInFieldId.Title].ToString();
                            uval.Url = web.Url + "/" + item.Folder.Url;
                            taskitem["Документ"] = uval;
                            if (!string.IsNullOrEmpty(bi.UserStep.CopyFields))
                                foreach (string s in bi.UserStep.CopyFields.Split(';'))
                                    taskitem[s] = item[s];

                            // bi.UserStep.StartTaskMail(item.Web, item);

                            taskitem.Update();
                            bi.TaskId = taskitem.ID;
                        }
                    }
                });
            }
        }

        public static void UpdateTask(SPListItem item, BranchInfo bi, string result)
        {
            if (bi.UserStep.CreateTask)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(item.Web.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList tasklist = web.Lists["Статистика задач"];
                            SPListItem taskitem = tasklist.GetItemById(bi.TaskId);
                            SPFieldUserValue fuv = new SPFieldUserValue(web, web.SiteUsers[bi.MoidifiedBy].ID, bi.MoidifiedBy);
                            taskitem["Завершивший исполнитель"] = fuv;
                            taskitem["Решение согласующего"] = result;
                            taskitem["Дата завершения"] = bi.CompleteDate;
                            taskitem.Update();

                            // bi.UserStep.EndTaskMail(item.Web, item, string.Empty,result == "Согласовано" ? true : false, fuv.User.Name);
                        }
                    }
                });
            }
        }

        public static void CreateCancelRecord(SPListItem item, SPUser user, Workflow wf)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(item.Web.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        SPList tasklist = web.Lists["Статистика задач"];
                        SPListItem taskitem = tasklist.AddItem();
                        taskitem[SPBuiltInFieldId.Title] = "Прерывание процесса";
                        taskitem["Дата начала"] = DateTime.Now;
                        taskitem["Дата завершения"] = DateTime.Now;
                        taskitem["Документ ИД"] = item.ID;
                        taskitem["Библиотека"] = item.ParentList.Title;
                        SPFieldUrlValue uval = new SPFieldUrlValue();
                        uval.Description = item[SPBuiltInFieldId.Title].ToString();
                        uval.Url = web.Url + item.Folder.Url;
                        taskitem["Документ"] = uval;
                        taskitem["Завершивший исполнитель"] = new SPFieldUserValue(web, web.SiteUsers[user.LoginName].ID, user.LoginName);
                        taskitem["Решение согласующего"] = "Прервано";
                        taskitem.Update();

                        foreach (BranchInfo bi in wf.ProcessUsers)
                        {
                            if (bi.UserStep.CreateTask && bi.TaskId != 0)
                            {
                                taskitem = tasklist.GetItemById(bi.TaskId);
                                taskitem["Завершивший исполнитель"] = new SPFieldUserValue(web, web.SiteUsers[user.LoginName].ID, user.LoginName);
                                taskitem["Решение согласующего"] = "Прервано";
                                taskitem["Дата завершения"] = DateTime.Now;
                                taskitem.Update();
                            }
                        }
                    }
                }
            });
        }


        public static void ReplaceTaskUser(SPListItem item, SPUser olduser, SPUser newuser, BranchInfo bi, bool ChangeUser)
        {
            if (bi.UserStep.CreateTask)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(item.Web.Url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            SPList tasklist = web.Lists["Статистика задач"];
                            SPListItem taskitem = tasklist.GetItemById(bi.TaskId);
                            SPFieldUserValueCollection val = new SPFieldUserValueCollection(web,taskitem["Исполнители задачи"].ToString());
                            foreach (SPFieldUserValue vu in val)
                            {
                                if (vu.User.LoginName == olduser.LoginName)
                                {
                                    if (ChangeUser)
                                        RemoveUsersField(val, bi, web);
                                    val.Add(new SPFieldUserValue(web, newuser.ID, newuser.LoginName));
                                    break;
                                }
                            }
                            FormUsersField(bi, web);
                            taskitem["Исполнители задачи"] = val;
                            taskitem.Update();
                        }
                    }
                });
            }
        }
        #endregion
    }
}
