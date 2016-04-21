using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using RedSys.Common.Workflow;
using RedSys.RFC.Core.Helper;

namespace PSE.CustomDispFormWP.ApprovalListWP
{
    [ToolboxItemAttribute(false)]
    public class ApprovalListWP : WebPart
    {
        string _urlFilterName;
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue("ID"),
        Category("Настройки"),
        WebDescription("Названия параметра в строке URL, отвечающего за ID элемента"),
        WebDisplayName("Названия параметра в строке URL")]
        public string UrlFilterName
        {
            get { return _urlFilterName; }
            set { _urlFilterName = value; }
        }

        string _listName;
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Настройки Фильтра"),
        WebDescription("Название списка, из которого должен выбираться согласуемый документ"),
        WebDisplayName("Название списка")]
        public string ListName
        {
            get { return _listName; }
            set { _listName = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Настройки"),
        WebDescription("Столбцы списка, отображаемые в шапке"),
        WebDisplayName("Столбцы списка")]
        public string listFields
        { get; set; }

        string _fieldName;
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue("Лист согласования"),
        Category("Настройки Фильтра"),
        WebDescription("Название поля, из которого должен браться лист согласования"),
        WebDisplayName("Название поля")]
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Настройки Фильтра"),
        WebDescription("Брать из текущего элемента"),
        WebDisplayName("Брать из текущего элемента")]
        public bool searchInCurrentItem
        {
            get;
            set;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if (!searchInCurrentItem)
            {
                Button btn = new Button();
                btn.Text = "Назад";
                btn.Attributes.Add("onClick", "javascript:history.back(); return false;");
                this.Controls.Add(btn);
                this.Controls.Add(new LiteralControl("<br/>"));
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            int id;
            string styles =
@"<style type='text/css'>
table.listsogl td
{
height: 35px;
font-size: 8pt;
font-family: 'Arial';
font-style: normal;
font-weight: normal; 
margin-top: 0px;
margin-bottom: 0px;
vertical-align: top;}
</style>";
            string Header = "";
            string text = "";
            SPListItem oItem = null;
            try
            {
                if (searchInCurrentItem)
                {
                    oItem = SPContext.Current.Item as SPListItem;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Page.Request.QueryString[_urlFilterName])
                        &&
                        int.TryParse(Page.Request.QueryString[_urlFilterName], out id))
                    {
                        SPWeb oWeb = SPContext.Current.Web;
                        SPList oList = oWeb.Lists[_listName];
                        oItem = oList.GetItemById(id);
                    }
                }

                if (oItem != null)
                {
                    Header = formHeader(oItem);
                    ItemWorkflows wfs = new ItemWorkflows(oItem);
                    foreach (Workflow wf in wfs.AllItemWorkflows)
                    {
                        if (!string.IsNullOrEmpty(wf.Initiator))
                        {
                            text = AddStartHistory(wf);
                            foreach (BranchInfo bi in wf.CompleteUsers)
                            {
                                text += FormAppListRow(bi);
                            }
                            foreach (BranchInfo bi in wf.ProcessUsers)
                            {
                                if (DateTime.Compare(bi.CompleteDate, DateTime.MinValue) == 0)
                                    text += FormAppListCurrentRow(bi);
                            }
                            if (!string.IsNullOrEmpty(wf.StoppedBy))
                            {
                                text += FormAppListCancel(wf);
                            }
                            text = FormAppList(text);
                            writer.Write(styles + Header + text);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex);
            }
        }
        protected string formHeader(SPListItem oItem)
        {

            string header = "<div style='left: 25px; width: 95%; height: auto; overflow: hidden; z-index: 0;'>";
            if (!string.IsNullOrEmpty(listFields))
            {
                header = "<div style='left: 25px; width: 95%; height: auto; overflow: hidden; z-index: 0;'>" +
"<table class='listsogl' border='0' cellpadding='4' cellspacing='1' bordercolorlight='#DDDDDD' bordercolordark='#DDDDDD' bgcolor='#aaaaaa'>";
                try
                {
                    SPList olist = oItem.ParentList;
                    foreach (string s in listFields.Split(';'))
                    {
                        if (olist.Fields.ContainsField(s))
                        {
                            string val = "";
                            if (oItem[s] != null)
                            {
                                val = oItem[s].ToString();
                                if (olist.Fields[s].Type == SPFieldType.User)
                                {
                                    if (((SPFieldUser)olist.Fields[s]).AllowMultipleValues)
                                    {
                                        SPFieldUserValueCollection vals = new SPFieldUserValueCollection(oItem.Web, val);
                                        val = "";
                                        foreach (SPFieldUserValue v in vals)
                                        {
                                            val += v.User.Name;
                                            val += ";";
                                        }

                                    }
                                    else
                                    {
                                        val = new SPFieldUserValue(oItem.Web, val).User.Name;
                                    }
                                }
                                if (olist.Fields[s].Type == SPFieldType.Lookup || olist.Fields[s].TypeAsString.ToLower() == "ensollookup" || olist.Fields[s].TypeAsString.ToLower() == "lookupfieldwithpicker")
                                {
                                    val = new SPFieldLookupValue(val).LookupValue;
                                }
                            }
                            header += "<tr><td style='height: 15px;vertical-align: middle' bgcolor='#DDDDDD'><b>" + s + ": </b></td>" +
                                "<td width='100%' style='height: 15px;vertical-align: middle' bgcolor='#FFFFFF' colspan='6'>" + val + "</td></tr>";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.DUmpException(ex);
                }
                header += "</table>";
            }
            return header;
        }

        protected string FormAppList(string Rows)
        {
            string result =
 @"<table class='listsogl' border='0' cellpadding='4' cellspacing='1' bordercolorlight='#DDDDDD' bordercolordark='#DDDDDD' bgcolor='#aaaaaa'>
<tr><td width='100' bgcolor='#EFEFEF'  style='vertical-align: middle'><b>Шаг согласования</b></td>
<td width='20%' bgcolor='#EFEFEF' style='vertical-align: middle'><b>Имя</b></td>
<td width='15%' bgcolor='#EFEFEF' style='vertical-align: middle'><b>Дата начала</b></td>
<td width='15%' bgcolor='#EFEFEF' style='vertical-align: middle'><b>Дата окончания</b></td>
<td width='25%' bgcolor='#EFEFEF' style='vertical-align: middle'><b>Результат</b></td>
<td width='25%' bgcolor='#EFEFEF' style='vertical-align: middle'><b>Комментарий</b></td></tr>" +
 Rows + @"</table></div><br/><br/><br/>";
            return result;
        }

        protected string FormAppListRow(BranchInfo bi)
        {
            string row = "";
            string dolgSoglasant = "";
            SPUser modifyByUser = null;
            string usr = bi.User.RealUserName;

            if (!string.IsNullOrEmpty(bi.MoidifiedBy))
            {
                modifyByUser = SPContext.Current.Web.EnsureUser(bi.MoidifiedBy);
                string login = modifyByUser.LoginName.Contains("|") ?
                    modifyByUser.LoginName.Substring(modifyByUser.LoginName.IndexOf("|") + 1) :
                    modifyByUser.LoginName;
                usr = string.Format("{0} ({1})", modifyByUser.Name, login);
            }

            string style = string.Empty;
            string comment = bi.Comment;
            string reshenie = string.Empty;
            string ttl = bi.UserStep.StageName;
            if (!bi.Approved)
            {
                if (!bi.Delegted)
                {
                    reshenie = "Отклонено";
                    style = "style='background-color: rgb(255, 190, 190);'";
                }
                else
                {
                    reshenie = "Требуется дополнительное согласование";
                    style = "style='background-color: rgb(255, 255, 123);'";
                }
            }
            else
            {
                if (bi.Step == 0)
                {
                    reshenie = bi.Name;
                    ttl = bi.RoleName;
                }
                else
                    reshenie = "Согласовано";
                style = "style='background-color: #F8F8F8;'";
            }

            DateTime dateEnd = bi.CompleteDate;
            DateTime dateBegin = bi.StartDate;
            row = @"<tr " + style + ">" +
                                "<td>" + ttl + "</td>" +
                                "<td>" + usr + "</td>" +
                                "<td>" + dateBegin.ToString() + "</td>" +
                                "<td>" + dateEnd.ToString() + "</td>" +
                                "<td>" + reshenie + "</td>" +
                                "<td>" + comment + "</td></tr>";
            return row;
        }

        protected string FormAppListCurrentRow(BranchInfo bi)
        {
            string row = "";

            string usr = bi.User.RealUserName;
            if (!string.IsNullOrEmpty(usr))
            {
                SPUser user = SPContext.Current.Web.EnsureUser(usr);

                string login = usr.Contains("|") ?
                        usr.Substring(usr.IndexOf("|") + 1) :
                        usr;
                usr = string.Format("{0} ({1})", user.Name, login);
            }
            if (!string.IsNullOrEmpty(bi.User.UserName) && bi.User.UserName != bi.User.RealUserName)
            {
                SPUser user = SPContext.Current.Web.EnsureUser(bi.User.UserName);
                string login = bi.User.UserName.Contains("|") ?
                        bi.User.UserName.Substring(bi.User.UserName.IndexOf("|") + 1) :
                        bi.User.UserName;
                usr += string.Format("<br/>{0} ({1})", user.Name, login);
            }

            foreach (UserInfo ui in bi.AdditionalUsers)
            {
                if (!string.IsNullOrEmpty(ui.RealUserName))
                {
                    SPUser user = SPContext.Current.Web.EnsureUser(ui.RealUserName);
                    string login = ui.RealUserName.Contains("|") ?
                             ui.RealUserName.Substring(ui.RealUserName.IndexOf("|") + 1) :
                             ui.RealUserName;
                    usr += string.Format("<br/>{0} ({1})", user.Name, login);
                }
                if (!string.IsNullOrEmpty(ui.UserName) && ui.UserName != ui.RealUserName)
                {
                    SPUser user = SPContext.Current.Web.EnsureUser(ui.UserName);
                    string login = ui.UserName.Contains("|") ?
                            ui.UserName.Substring(ui.UserName.IndexOf("|") + 1) :
                            ui.UserName;
                    usr += string.Format("<br/>{0} ({1})", user.Name, login);
                }
            }
            string style = "style ='background-color: #F8F8F8;'";
            DateTime dateBegin = bi.StartDate;
            row = @"<tr " + style + ">" +
                                "<td>" + bi.UserStep.StageName + "</td>" +
                                "<td>" + usr + "</td>" +
                                "<td>" + dateBegin.ToString() + "</td>" +
                                "<td></td>" +
                                "<td></td>" +
                                "<td></td></tr>";
            return row;
        }

        protected string AddStartHistory(Workflow wf)
        {
            string Fio = "";

            string login = wf.Initiator;
            login = login.Contains("|") ?
                    login.Substring(login.IndexOf("|") + 1) :
                    login;
            if (string.IsNullOrEmpty(login))
                return "";
            SPUser user = SPContext.Current.Web.EnsureUser(login);

            Fio = string.Format("{0} ({1})", user.Name, login);

            DateTime dateEnd = wf.InitDate;

            string dataRow = @"<tr style = 'background-color: #F8F8F8;'>" +
"<td>Начало согласования</td>" +
"<td>" + Fio + "</TD>" +
"<td  colspan=2>" + dateEnd.ToString() + "</td>" +
"<td width=100 colspan=2></td></tr>";
            return dataRow;
        }

        protected string FormAppListCancel(Workflow wf)
        {
            SPUser StoppedByUser = null;
            string usr = wf.StoppedBy;
            StoppedByUser = SPContext.Current.Web.EnsureUser(usr);
            string login = StoppedByUser.LoginName.Contains("|") ?
                StoppedByUser.LoginName.Substring(StoppedByUser.LoginName.IndexOf("|") + 1) :
                StoppedByUser.LoginName;
            usr = string.Format("{0} ({1})", StoppedByUser.Name, login);

            string style = string.Empty;
            style = "style='background-color: rgb(255, 190, 190);'";
            DateTime dateEnd = wf.CompleteDate;

            string row = @"<tr " + style + ">" +
                "<td>Процесс остановлен</td>" +
                "<td>" + usr + "</td>" +
                "<td colspan='2'>" + dateEnd.ToString() + "</td>" +
                "<td colspan='2'></td></tr>";
            return row;
        }
    
}
}
