using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using CamlexNET;

namespace Ensol.Common.Workflow
{
    public static class Helper
    {

        public class EventReceiverManager : SPEventReceiverBase, IDisposable
        {
            public EventReceiverManager(bool disableImmediately)
            {
                EventFiringEnabled = !disableImmediately;
            }

            public void StopEventReceiver()
            {
                EventFiringEnabled = false;
            }
            public void StartEventReceiver()
            {
                EventFiringEnabled = true;
            }

            public void Dispose()
            {
                EventFiringEnabled = true;
            }
        }
        #region SetFieldValue

        public static void SetStatus(SPListItem item, string status)
        {
            item.Web.AllowUnsafeUpdates = true;
            item["Stage"] = status;
            if (item["Связанные документы"] != null)
            {
                SPFieldLookupValueCollection linkeddocs = (SPFieldLookupValueCollection)item["Связанные документы"];
                foreach (SPFieldLookupValue linkeddoc in linkeddocs)
                {
                    using (EventReceiverManager ev = new EventReceiverManager(true))
                    {
                        SPListItem doc2update = item.Web.Lists[new Guid(((SPFieldLookup)item.Fields["Связанные документы"]).LookupList)].GetItemById(linkeddoc.LookupId);
                        doc2update["Stage"] = status;
                        doc2update.SystemUpdate(false);
                        ev.StartEventReceiver();
                    }
                }
            }
        }

        public static void SetFieldValueUser(this SPListItem item,
          string fieldName, IEnumerable<SPPrincipal> principals)
        {
            if (item != null)
            {
                SPFieldUserValueCollection fieldValues =
                  new SPFieldUserValueCollection();

                foreach (SPPrincipal principal in principals)
                {
                    fieldValues.Add(
                      new SPFieldUserValue(
                        item.Web, principal.ID, principal.Name));
                }
                item[fieldName] = fieldValues;
            }
        }

        public static void SetFieldValueUser(this SPListItem item,
          Guid fieldName, List<SPUser> principals)
        {
            if (item != null)
            {
                SPFieldUserValueCollection fieldValues =
                  new SPFieldUserValueCollection();

                foreach (SPUser principal in principals)
                {
                    fieldValues.Add(
                      new SPFieldUserValue(
                        item.Web, principal.ID, principal.Name));
                }
                item[fieldName] = fieldValues;
            }
        }

        public static void SetFieldValueUser(this SPListItem item,
         string fieldName, List<SPUser> principals)
        {
            if (item != null)
            {
                SPFieldUserValueCollection fieldValues =
                  new SPFieldUserValueCollection();

                foreach (SPUser principal in principals)
                {
                    fieldValues.Add(
                      new SPFieldUserValue(
                        item.Web, principal.ID, principal.Name));
                }
                item[fieldName] = fieldValues;
            }
        }
        #endregion

        #region GetFieldValue
        public static string GetFieldValueByType(this SPListItem listItem, string fieldName, string type)
        {
            string retString = string.Empty;
            try
            {
                if (listItem[fieldName] != null)
                {
                    switch (type)
                    {
                        case ("string"): retString = listItem.GetFieldValue(fieldName); break;
                        case ("boolean"): retString = (listItem.GetFieldValueBoolean(fieldName) ? "Да" : "Нет"); break;
                        case ("shortdate"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToShortDateString() : string.Empty); break;
                        case ("shorttime"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToShortTimeString() : string.Empty); break;
                        case ("formatyyyy-MM-dd"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToString(type.Replace("format", "")) : string.Empty); break;
                        case ("double"): retString = listItem.GetFieldValueDouble(fieldName).ToString(); break;
                        case ("int"): retString = listItem.GetFieldValueInt(fieldName).ToString(); break;
                        case ("lookupid"): retString = listItem.GetFieldValueLookup(fieldName).LookupId.ToString(); break;
                        case ("lookupvalue"): retString = listItem.GetFieldValueLookup(fieldName).LookupValue.ToString(); break;
                        case ("userlogin"): retString = listItem.GetFieldValueUser(fieldName).LoginName.ToString(); break;
                        case ("username"): retString = listItem.GetFieldValueUser(fieldName).Name.ToString(); break;
                        case ("useremail"): retString = listItem.GetFieldValueUser(fieldName).Email.ToString(); break;
                        default: retString = listItem.GetFieldValue(fieldName); break;

                    }
                }
            }
            catch (Exception ex)
            {
            }

            return retString;
        }


        public static string GetFieldValue(this SPListItem listItem, string fieldName)
        {
            string text = string.Empty;
            if (fieldName == string.Empty)
            {
                return text;
            }
            try
            {
                object myObj = listItem[fieldName];
                return ((myObj != null) ? myObj.ToString() : string.Empty);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string GetFieldValue(this SPListItem listItem, Guid fieldName)
        {
            string text = string.Empty;
            if (fieldName == Guid.Empty)
            {
                return text;
            }
            try
            {
                object myObj = listItem[fieldName];
                return ((myObj != null) ? myObj.ToString() : string.Empty);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static SPFieldLookupValue GetFieldValueLookup(this SPListItem listItem, Guid fieldName)
        {
            if (fieldName == Guid.Empty)
            {
                return null;
            }
            try
            {
                SPFieldLookupValue spFieldLookupValue = new SPFieldLookupValue(listItem[fieldName].ToString());
                return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SPFieldLookupValueCollection GetFieldValueLookupMulti(this SPListItem listItem, Guid fieldName)
        {
            if (fieldName == Guid.Empty)
            {
                return null;
            }
            try
            {
                SPFieldLookupValueCollection spFieldLookupValue = new SPFieldLookupValueCollection(listItem[fieldName].ToString());
                return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SPFieldLookupValueCollection GetFieldValueLookupMulti(this SPListItem listItem, string fieldName)
        {
            if (fieldName == String.Empty)
            {
                return null;
            }
            try
            {
                SPFieldLookupValueCollection spFieldLookupValue = new SPFieldLookupValueCollection(listItem[fieldName].ToString());
                return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SPFieldLookupValue GetFieldValueLookup(this SPListItem listItem, string fieldName)
        {
            if (fieldName == string.Empty)
            {
                return null;
            }
            try
            {
                SPFieldLookupValue spFieldLookupValue = new SPFieldLookupValue(listItem[fieldName].ToString());
                return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool GetFieldValueBoolean(this SPListItem listItem, Guid fieldName)
        {
            bool retBool = false;
            if (fieldName == Guid.Empty)
            {
                return retBool;
            }
            try
            {
                if (listItem != null)
                {
                    if (listItem[fieldName] == null) return retBool;
                    retBool = (bool)listItem[fieldName];
                }
                else
                {
                    return retBool;
                }
            }
            catch (Exception ex)
            {
                return retBool;
            }
            return retBool;

        }

        public static bool GetFieldValueBoolean(this SPListItem listItem, string fieldName)
        {
            bool retBool = false;
            if (fieldName == string.Empty)
            {
                return retBool;
            }
            try
            {
                if (listItem != null)
                {
                    if (listItem[fieldName] == null) return retBool;
                    retBool = (bool)listItem[fieldName];
                }
                else
                {
                    return retBool;
                }
            }
            catch (Exception ex)
            {
                return retBool;
            }
            return retBool;

        }

        public static SPUser GetFieldValueUser(this SPListItem item, Guid fieldName)
        {
            if (fieldName == Guid.Empty)
            {
                return null;
            }
            try
            {
                if (item != null)
                {
                    SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[fieldName] as string);
                    return userValue.User;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static SPUser GetFieldValueUser(this SPListItem item, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }
            try
            {
                if (item != null)
                {
                    SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[fieldName] as string);
                    if (userValue != null && userValue.User == null && userValue.LookupId != 0)
                    {
                        return item.Web.SiteUsers.GetByID(userValue.LookupId);
                    }
                    return userValue.User;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DateTime? GetFieldValueDateTime(this SPListItem listItem, Guid fieldName)
        {
            if (fieldName == Guid.Empty)
            {
                return null;
            }
            try
            {
                if (listItem != null)
                {
                    object myObj = listItem[fieldName];
                    if (myObj == null)
                    {
                        return null;
                    }
                    else
                    {
                        return SPUtility.CreateDateTimeFromISO8601DateTimeString(listItem[fieldName].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DateTime res ;
                bool r = DateTime.TryParse(listItem[fieldName].ToString(), out res);
                if (r)
                    return res;
                else
                    return null;
            }
            return null;
        }

        public static DateTime? GetFieldValueDateTime(this SPListItem listItem, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }
            try
            {
                if (listItem != null)
                {
                    object myObj = listItem[fieldName];
                    if (myObj == null)
                    {
                        return null;
                    }
                    else
                    {
                        return SPUtility.CreateDateTimeFromISO8601DateTimeString(listItem[fieldName].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                DateTime res;
                bool r = DateTime.TryParse(listItem[fieldName].ToString(), out res);
                if (r)
                    return res;
                else
                    return null;
            }
            return null;
        }

        public static int GetFieldValueInt(this SPListItem listItem, Guid fieldName)
        {
            string retString = GetFieldValue(listItem, fieldName);
            int retInt = 0;
            Int32.TryParse(retString, out retInt);
            return retInt;
        }
        public static int GetFieldValueInt(this SPListItem listItem, string fieldName)
        {
            string retString = GetFieldValue(listItem, fieldName);
            int retInt = 0;
            Int32.TryParse(retString, out retInt);
            return retInt;
        }

        public static double GetFieldValueDouble(this SPListItem listItem, Guid fieldName)
        {
            string retString = GetFieldValue(listItem, fieldName);
            double retInt = 0;
            double.TryParse(retString, out retInt);
            return retInt;
        }

        public static double GetFieldValueDouble(this SPListItem listItem, string fieldName)
        {
            string retString = GetFieldValue(listItem, fieldName);
            double retInt = 0;
            double.TryParse(retString, out retInt);
            return retInt;
        }
        #endregion

        #region web

        public static SPList GetListExt(this SPWeb spWeb, string url)
        {
            if (spWeb == null) return null;
            SPList spList = null;
            try
            {
                spList = spWeb.GetList(SPUtility.ConcatUrls(spWeb.Url, url));
            }
            catch
            {
            }
            return spList;
        }

        #endregion

        #region parser

        public static bool IsTrue(this string value)
        {
            try
            {
                // 1
                // Avoid exceptions
                if (value == null)
                {
                    return false;
                }

                // 2
                // Remove whitespace from string
                value = value.Trim();

                // 3
                // Lowercase the string
                value = value.ToLower();

                // 4
                // Check for word true
                if (value == "true")
                {
                    return true;
                }

                // 5
                // Check for letter true
                if (value == "t")
                {
                    return true;
                }

                // 6
                // Check for one
                if (value == "1")
                {
                    return true;
                }

                // 7
                // Check for word yes
                if (value == "yes")
                {
                    return true;
                }

                // 8
                // Check for letter yes
                if (value == "y")
                {
                    return true;
                }

                // 9
                // It is false
                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Permission

        public static void AddPermissions(this SPListItem item, IEnumerable<SPPrincipal> principals, SPRoleType roleType)
        {
            /*if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(true);
            }
            item.SetPermissions(principals, roleType);*/
        }



        //SPGroup group = web.Groups[0];
        //SPUser user = web.Users[0];
        //SPUser user2 = web.EnsureUser("mangaldas.mano");
        //SPUser user3 = web.EnsureUser("Domain Users"); ;
        //SPPrincipal[] principals = { group, user, user2, user3 };
        public static void SetPermissions(this SPListItem item, IEnumerable<SPPrincipal> principals, SPRoleType roleType)
        {
            /*if (item != null)
            {

                foreach (SPPrincipal principal in principals)
                {
                    SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                    SetPermissions(item, principal, roleDefinition);
                }
            }*/
        }


        public static void SetPermissions(this SPListItem item, SPUser user, SPRoleType roleType)
        {
            /*if (item != null)
            {
                SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                SetPermissions(item, (SPPrincipal)user, roleDefinition);
            }*/
        }

        public static void SetPermissions(this SPListItem item, SPPrincipal principal, SPRoleType roleType)
        {
            /*if (item != null)
            {
                SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                SetPermissions(item, principal, roleDefinition);
            }*/
        }

        public static void SetPermissions(this SPListItem item, SPUser user, SPRoleDefinition roleDefinition)
        {
            /*if (item != null)
            {
                SetPermissions(item, (SPPrincipal)user, roleDefinition);
            }*/
        }

        public static void SetPermissions(this SPListItem item, SPPrincipal principal, SPRoleDefinition roleDefinition)
        {
            /*if (item != null)
            {
                SPRoleAssignment roleAssignment = new SPRoleAssignment(principal);

                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                item.RoleAssignments.Add(roleAssignment);
            }*/
        }

        public static void RemovePermissions(this SPListItem item, SPUser user)
        {
            /*if (item != null)
            {
                RemovePermissions(item, user as SPPrincipal);
            }*/
        }

        public static void RemovePermissions(this SPListItem item, SPPrincipal principal)
        {
            /*if (item != null)
            {
                item.RoleAssignments.Remove(principal);
                HandleEventFiring ev = new HandleEventFiring();
                ev.AccDisableEventFiring();
                //item.SendToAxapta();
                item.SystemUpdate(false);
                ev.AccEnableEventFiring();
            }*/
        }

        public static void RemovePermissionsSpecificRole(this SPListItem item, SPPrincipal principal, SPRoleDefinition roleDefinition)
        {
            /*if (item != null)
            {
                SPRoleAssignment roleAssignment = item.RoleAssignments.GetAssignmentByPrincipal(principal);
                if (roleAssignment != null)
                {
                    if (roleAssignment.RoleDefinitionBindings.Contains(roleDefinition))
                    {
                        roleAssignment.RoleDefinitionBindings.Remove(roleDefinition);
                        roleAssignment.Update();
                    }
                }
            }*/
        }

        public static void RemovePermissionsSpecificRole(this SPListItem item, SPPrincipal principal, SPRoleType roleType)
        {
            /*if (item != null)
            {
                SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                RemovePermissionsSpecificRole(item, principal, roleDefinition);
            }*/
        }

        public static void ChangePermissions(this SPListItem item, SPPrincipal principal, SPRoleType roleType)
        {
            /*if (item != null)
            {
                SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                ChangePermissions(item, principal, roleDefinition);
            }*/
        }

        public static void ChangePermissions(this SPListItem item, SPPrincipal principal, SPRoleDefinition roleDefinition)
        {
            /*SPRoleAssignment roleAssignment = item.RoleAssignments.GetAssignmentByPrincipal(principal);
            if (roleAssignment != null)
            {
                roleAssignment.RoleDefinitionBindings.RemoveAll();
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                roleAssignment.Update();
            }*/
        }


        #endregion

        #region splistitem

        public static void SendToAxapta(this SPListItem spListItem, string weburl, string listurl)
        {
            //Sync.Execute(spListItem, false, "importData");
            //if (spListItem.ParentList.Title.StartsWith("Договоры") ||
            //    spListItem.ParentList.Title.StartsWith("Заявки") ||
            //    spListItem.ParentList.Title.StartsWith("Контрагенты"))
            //{
            //    try
            //    {
            //        Microsoft.Dynamics.BusinessConnectorNet.Axapta DynAx = AIT.Common.Common.GetAxaptaConnection();
            //        Data data = new Data(spListItem.Web.Site.Url, "", listurl);
            //        string xmlData = data.Sync(spListItem);

            //        // string retString = (string)DynAx.CallStaticClassMethod("ndnSPImportBase", "importData", xmlData);


            //        spListItem["Выгрузка в DAX"] = (string)DynAx.CallStaticClassMethod("ndnSPImportBaseTest", "importData", xmlData);
            //        DynAx.Logoff();
            //    }
            //    catch
            //    {
            //        ErrorSync.Create(spListItem.ID, spListItem.ParentList.ID.ToString(), spListItem.Web.ID.ToString(), spListItem.Web.Site.ID.ToString());
            //        spListItem["Ошибка выгрузки в DAX"] = 1;
            //    }
            //}
        }

        #endregion

        #region email
        /*
        public static bool SendMail(string Subject, string Body, bool IsBodyHtml, string From, List<string> To, string Cc, string Bcc, Guid SiteID)
        {
            bool mailSent = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    try
                    {
                        SmtpClient smtpClient = new SmtpClient();
                        using (SPSite spSite = new SPSite(SiteID))
                        {
                            smtpClient.Host =
                                spSite.WebApplication.
                                    OutboundMailServiceInstance.Server.Address;
                            MailMessage mailMessage = new MailMessage();
                            mailMessage.From = new MailAddress(From);
                            mailMessage.Subject = Subject;
                            mailMessage.Body = Body;
                            foreach (string s in To)
                            {
                                mailMessage.To.Add(new MailAddress(s));
                            }
                            if (!String.IsNullOrEmpty(Cc))
                            {
                                MailAddress CCAddress = new MailAddress(Cc);
                                mailMessage.CC.Add(CCAddress);
                            }
                            if (!String.IsNullOrEmpty(Bcc))
                            {
                                MailAddress BCCAddress = new MailAddress(Bcc);
                                mailMessage.Bcc.Add(BCCAddress);
                            }
                            mailMessage.IsBodyHtml = IsBodyHtml;
                            smtpClient.Send(mailMessage);
                            mailSent = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        AIT.Common.Common.DUmpException(ex);
                    }
                });
            }
            catch (Exception ex) {
                AIT.Common.Common.DUmpException(ex);
                return mailSent; 
            }
            return mailSent;
        }
        */
        /*
        public static bool SendMail(string Subject, string Body, bool IsBodyHtml, string From, string To, string Cc, string Bcc, Guid SiteID)
        {
            bool mailSent = false;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SmtpClient smtpClient = new SmtpClient();
                    using (SPSite spSite = new SPSite(SiteID))
                    {
                        string[] logins = To.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> emails = new List<string>();
                        foreach (string login in logins)
                        {
                            try
                            {
                                emails.Add(spSite.RootWeb.SiteUsers[login].Email);
                            }
                            catch (Exception ex)
                            {
                                AIT.Common.Common.DUmpException(ex);
                            }
                        }
                        SendMail(Subject, Body, IsBodyHtml, From, emails, Cc, Bcc, SiteID);
                    }
                });
            }
            catch (Exception ex)
            {
                AIT.Common.Common.DUmpException(ex);
                return mailSent;
            }
            return mailSent;
        }
        */
        public static bool SendMail(string Subject, string Body, bool IsBodyHtml, string To, Guid SiteID)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite spSite = new SPSite(SiteID))
                {
                    string[] logins = To.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in logins)
                    {
                        SPUser u = spSite.RootWeb.SiteUsers[s];
                        string am = u.Email;
                        Microsoft.SharePoint.Utilities.SPUtility.SendEmail(spSite.RootWeb, true, IsBodyHtml, am, Subject, Body);
                    }
                }
            });
            return true;
        }

        #endregion

        #region Query
        public static SPListItemCollection GetItemsByValue(SPWeb web, string listName, string filterString)
        {
            var expressions = new List<Expression<Func<SPListItem, bool>>>();

            SPQuery search = new SPQuery();
            search.ViewAttributes = "Scope=\"RecursiveAll\"";
            SPListItemCollection resultItems = null;
            SPList tList = web.Lists[listName];

            string[] filterPairs = filterString.Split(';');

            try
            {
                foreach (string pair in filterPairs)
                {
                    string fieldName = pair.Split('=')[0];
                    string fieldValue = pair.Split('=')[1];
                    if (fieldValue.IndexOf(";#") > -1)
                        fieldValue = fieldValue.Split('#')[1];

                    switch (fieldValue)
                    {
                        case "null": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == null); break;
                        case "notnull": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) != null); break;
                        default:
                            if (fieldName.Contains('|'))
                            {
                                expressions.Add(f => f[tList.Fields[fieldName].InternalName] == (DataTypes.LookupValue)fieldValue);
                            }
                            else
                                expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == fieldValue);
                            break;
                    }
                }
                search.Query = Camlex.Query().WhereAll(expressions).ToString();
                resultItems = tList.GetItems(search);
            }
            catch { }
            return resultItems;
        }
        #endregion


    }
}

