using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSys.Common.Workflow;
using RedSys.Common;
using RedSys.RFC.Core.Helper;

namespace RedSys.Common.ReplaceUsers
{
    public class ReplaceUsersHelper
    {
        public static void ReplaceUsers(Settings appOptions)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite oSite = new SPSite(appOptions.Url))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            //appOptions.Load();
                            oWeb.AllowUnsafeUpdates = true;
                            ExceptionHelper.DUmpMessage("Подключение к  сайту прошло успешно, начало замены пользователей.\n\n");

                            if (!string.IsNullOrEmpty(appOptions.ListNames) && !string.IsNullOrEmpty(appOptions.FieldNames))
                            {
                                ExceptionHelper.DUmpMessage("Начало замены пользователей в списках\n\n");
                                ChangeUsers(oWeb, appOptions.ListNames, appOptions.FieldNames, appOptions.ChangeUsers, appOptions.UpdateTasks, appOptions);
                            }
                            else
                                ExceptionHelper.DUmpMessage("Неверно указаны настройки списков, этап пропускается\n\n");

                            if (appOptions.UpdateBosses && !string.IsNullOrEmpty(appOptions.BossListNames) && !string.IsNullOrEmpty(appOptions.BossFieldNames))
                            {
                                ExceptionHelper.DUmpMessage("Начало замены пользователей в руководителях\n\n");
                                ChangeUsers(oWeb, appOptions.BossListNames, appOptions.BossFieldNames, true, false, appOptions);
                            }
                            else if (appOptions.UpdateBosses)
                                ExceptionHelper.DUmpMessage("Неверно указаны настройки руководителей, этап пропускается\n\n");

                            if (appOptions.UpdateRoles && !string.IsNullOrEmpty(appOptions.RoleListNames) && !string.IsNullOrEmpty(appOptions.RoleFieldNames))
                            {
                                ExceptionHelper.DUmpMessage("Начало замены пользователей в ролях\n\n");
                                ChangeUsers(oWeb, appOptions.RoleListNames, appOptions.RoleFieldNames, true, false, appOptions);
                            }
                            else if (appOptions.UpdateRoles)
                                ExceptionHelper.DUmpMessage("Неверно указаны настройки ролей, этап пропускается\n\n");

                            ExceptionHelper.DUmpMessage("\n\nОбновление пользователей завершено.\n\n");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpMessage("Ошибка: " + ex.Message + "\n StackTrace: " + ex.StackTrace + "\n Завершение работы");
            }
        }

        protected static void ChangeUsers(SPWeb web, string Lists, string Fields, bool Change, bool ChangeTask, Settings appOptions)
        {

            string AlphaList = Lists.Split(';')[0];

            SPUser user = web.EnsureUser(appOptions.OldUser);
            SPUser newuser = web.EnsureUser(appOptions.NewUser);


            SPList addlist = web.Lists.TryGetList(appOptions.ListNames);
            if (addlist != null)
            {
                using (EventReceiverManager emr = new EventReceiverManager(true))
                {
                    SPQuery oQuery = new SPQuery();
                    oQuery.ViewAttributes = "Scope=\"RecursiveAll\"";
                    oQuery.Query = CamlexNET.Camlex.Query().Where(x => x[addlist.Fields[appOptions.FieldNames].InternalName] == (CamlexNET.DataTypes.UserId)user.ID.ToString()).ToString();
                    SPListItemCollection items = addlist.GetItems(oQuery);
                    foreach (SPListItem additem in items)
                    {
                        ExceptionHelper.DUmpMessage("\nНайден документ в списке " + addlist.Title + ", ID: " + additem.ID + ", Название: " + additem.Title + ", Значение:" + additem[appOptions.FieldNames].ToString());
                        foreach (string str in Fields.Split(';'))
                        {
                            if (addlist.Fields.ContainsField(str))
                            {
                                if (additem[str] == null)
                                    continue;
                                SPFieldUser field = addlist.Fields[str] as SPFieldUser;
                                SPFieldUserValueCollection fval = new SPFieldUserValueCollection(web, additem[str].ToString());
                                foreach (SPFieldUserValue fuval in fval)
                                {
                                    if (fuval.User.ID == user.ID)
                                    {
                                        if (field.AllowMultipleValues)
                                        {
                                            SPFieldUserValueCollection val = new SPFieldUserValueCollection(web, additem[str].ToString());
                                            if (Change)
                                            {
                                                foreach (SPFieldUserValue v in val)
                                                {
                                                    if (v.User.ID == user.ID)
                                                    {
                                                        val.Remove(v);
                                                        break;
                                                    }
                                                }
                                                val.Add(new SPFieldUserValue(web, newuser.ID, newuser.Name));
                                            }
                                            else
                                                val.Add(new SPFieldUserValue(web, newuser.ID, newuser.Name));
                                            additem[str] = val;
                                        }
                                        else
                                        {
                                            SPFieldUserValue val = new SPFieldUserValue(web, newuser.ID, newuser.Name);
                                            additem[str] = val;
                                        }
                                        CopyPermissions(user, newuser, additem);
                                        additem.SystemUpdate(false);
                                    }
                                }
                            }
                        }
                        if (ChangeTask)
                        {
                            Workflow.Workflow wf = new Workflow.Workflow(additem);
                            wf.ReplaceCurrentUser(user, newuser, Change, ChangeTask);
                        }
                    }
                    emr.StartEventReceiver();
                }

            }
        }

        protected static void CopyPermissions(SPUser olduser, SPUser newuser, SPListItem item)
        {
            if (!item.HasUniqueRoleAssignments)
                return;
            //SPBasePermissions perms = item.GetUserEffectivePermissions(olduser.LoginName);
            SPRoleAssignment cont = new SPRoleAssignment(newuser);
            SPRoleDefinition role = item.ParentList.ParentWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
            cont.RoleDefinitionBindings.Add(role);
            item.RoleAssignments.Add(cont);
        }
    }
}
