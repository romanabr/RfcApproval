using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{

    public class HTMLHistory
    {
        public static string AddHistoryDummy(string approvementlist, string Stage, string ToLogin, SPWeb web, SPListItem item)
        {
            using (SPServiceContextScope scope = new Microsoft.SharePoint.SPServiceContextScope(SPServiceContext.GetContext(web.Site)))
            {
                string usr = ToLogin, dolgSoglasant = string.Empty;

                try
                {
                    if (!string.IsNullOrEmpty(ToLogin))
                    {
                        SPUser user = web.EnsureUser(ToLogin);
                        var userInfo = web.SiteUserInfoList.GetItemById(user.ID);
                        dolgSoglasant = userInfo["Должность"] == null ? string.Empty : userInfo["Должность"].ToString();


                        string login = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;
                        usr = string.Format("{0} ({1})", user.Name, login);


                        SPListItem customuser = null;
                        SPQuery oQuery = new SPQuery();
                        SPList customUserList = web.Lists["Сотрудники"];
                        string log = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;
                        oQuery.ViewXml = "<View><Method Name='Чтение списков'><Filter Name='ADSearch' Value='" + log + "'/>" +
                        "</Method><Query><OrderBy><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/>" +
                        "</OrderBy><Where><Eq><FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/><Value Type='Text'>" + log + "</Value>" +
                        "</Eq></Where></Query><ViewFields><FieldRef Name='" + customUserList.Fields["ФИО"].InternalName + "'/>" +
                        "<FieldRef Name='" + customUserList.Fields["Учетная запись"].InternalName + "'/></ViewFields><RowLimit>1</RowLimit></View>";
                        SPListItemCollection cusers = customUserList.GetItems(oQuery);
                        if (cusers.Count > 0)
                        {
                            customuser = cusers[0];

                            if (customuser["ФИО"] != null)
                            {
                                login = user.LoginName.Contains('|') ? user.LoginName.Substring(user.LoginName.IndexOf("|") + 1) : user.LoginName;
                                usr = string.Format("{0}<br/>({1})", customuser["ФИО"].ToString(), login);
                            }
                        }
                    }
                }
                catch (Exception) { }

                string style = "style='background-color: rgb(255, 255, 255);'";
                string comment = string.Empty;
                string reshenie = string.Empty;
                string dataRow = @"<tr " + style + ">" +
                                    "<td>" + Stage + "</td>" +
                                    "<td>" + dolgSoglasant + "</td>" +
                                    "<td>" + usr + "</td>" +
                                    "<td>" + "" + "</td>" +
                                    "<td>" + "" + "</td>" +
                                    "<td>" + reshenie + "</td>" +
                                    "<td>" + comment + "</td></tr>";
                approvementlist += dataRow;
            }
            return approvementlist;
        }

        public static string AddHistoryDummy(string approvementlist, string Stage)
        {
            string style = "style='background-color: rgb(255, 255, 255);'";
            string comment = string.Empty;
            string reshenie = string.Empty;
            string dataRow = @"<tr " + style + ">" +
                                "<td>" + Stage + "</td>" +
                                "<td>" + "</td>" +
                                "<td>" + "</td>" +
                                "<td>" + "</td>" +
                                "<td>" + "</td>" +
                                "<td>" + reshenie + "</td>" +
                                "<td>" + comment + "</td></tr>";
            approvementlist += dataRow;
            return approvementlist;
        }
    
    }
}
