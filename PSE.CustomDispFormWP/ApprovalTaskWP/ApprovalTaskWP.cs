using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data;
using RedSys.Common.Workflow;
using RedSys.RFC.Core.Helper;
using System.Drawing;
using RedSys.RFC.Data.Code;

namespace PSE.CustomDispFormWP.ApprovalTaskWP
{
    [ToolboxItemAttribute(false)]
    public class ApprovalTaskWP : WebPart
    {
        bool showWP;
        string oldstatus;

        Button Approve;
        Button Decline;
        Button Delegate;
        Button TakeInProcess;
        Button SendInProcess;
        PeopleEditor InProcessUserPicker;
        TextBox Comment;
        Label ErrorLbl;
        LiteralControl annotation;
        LiteralControl firstrow;
        LiteralControl beforerow;
        LiteralControl sendrow;
        LiteralControl sendsplitrow;
        LiteralControl midrow;
        LiteralControl afterrow;
        LiteralControl commentrow;
        Workflow wf;
        Panel p;
        UpdatePanel panel;
        UpdateProgress prog;
        private LoadType _howtowork;
        PeopleEditor UserPicker;

        public enum LoadType
        {
            Синхронно,
            Асинхронно,
            По_запросу
        }

        #region Variables
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue("Synchronous"),
        Category("Отображение"),
        WebDisplayName("Способ загрузки данных"),
        WebDescription("Способ загрузки данных")]
        public LoadType HowToWork
        {
            get
            {
                return _howtowork;
            }
            set { _howtowork = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Типы заданий"),
        WebDisplayName("Название задания согласование"),
        WebDescription("Название типа контента")]
        public string ApprovalTaskName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Типы заданий"),
        WebDisplayName("Название задания доработки"),
        WebDescription("Название типа контента")]
        public string ReworkTaskName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Типы заданий"),
        WebDisplayName("Название задания выполнения"),
        WebDescription("Название типа контента")]
        public string CompleteTaskName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Типы заданий"),
        WebDisplayName("Название задания доп. согласования"),
        WebDescription("Название типа контента")]
        public string DelegateTaskName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Пакет"),
        WebDisplayName("Искать по ид лукапа"),
        WebDescription("Искать по ид лукапа")]
        public bool SearchById
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Настройки"),
        WebDisplayName("Записывать в историю"),
        WebDescription("Записывать в историю")]
        public bool SaveToLog
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Пакет"),
        WebDisplayName("Обработка пакета"),
        WebDescription("Обработка пакета")]
        public bool ChangeChild
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Пакет"),
        WebDisplayName("Библиотеки пакета"),
        WebDescription("Библиотеки пакета")]
        public string ChildLib
        {
            get;
            set;
        }
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Пакет"),
        WebDisplayName("Поле текущего элемента"),
        WebDescription("Поле текущего элемента")]
        public string ChildKeyField
        {
            get;
            set;
        }
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Пакет"),
        WebDisplayName("Поле фильтруемого списка"),
        WebDescription("Поле фильтруемого списка")]
        public string KeyField
        {
            get;
            set;
        }



        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Настройки"),
        WebDescription("Не было согласовано"),
        WebDisplayName("Не было согласовано")]
        public bool wasntApproved
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Проверка данных"),
        WebDisplayName("Статусы рабочего процесса"),
        WebDescription("Статусы рабочего процесса через |")]
        public string StatusFilterString
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(""),
        Category("Проверка данных"),
        WebDisplayName("Поля, необходимые для заполнения"),
        WebDescription("Статусы рабочего процесса через ; (и | для разных статусов)")]
        public string RequiredFieldsString
        {
            get;
            set;
        }


        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Доп. согласование"),
        WebDisplayName("Название поля пользователя доп. согласования"),
        WebDescription("Название типа контента")]
        public string DelegateUserFieldName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Доп. согласование"),
        WebDisplayName("ИД этапа  доп. согласования"),
        WebDescription("Название типа контента")]
        public string DelegateTaskStage
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Взять в работу"),
        WebDisplayName("Поле с исполнителем"),
        WebDescription("Поле с исполнителем")]
        public string InProcessFieldName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Взять в работу"),
        WebDisplayName("Группа для назначения задачи"),
        WebDescription("Группа для назначения задачи")]
        public string InProcessGroupName
        {
            get;
            set;
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        Category("Взять в работу"),
        WebDisplayName("Статусы отображения"),
        WebDescription("Статусы отображения")]
        public string InProcessWFStatuses
        {
            get;
            set;
        }
        #endregion

        protected override void CreateChildControls()
        {
            this.Style.Add("padding", "0");
            showWP = false;


            Comment = new TextBox();
            Comment.Rows = 1;
            Comment.TextMode = TextBoxMode.MultiLine;
            Comment.Style.Add("width","calc(100% - 10px)");

            TakeInProcess = new Button();
            TakeInProcess.Text = "Взять в работу";
            TakeInProcess.Font.Bold = true;
            TakeInProcess.Click += TakeInProcess_Click;
            //TakeInProcess.Width = 392;
            TakeInProcess.Style.Add("margin", "0");
            TakeInProcess.Visible = false;
            TakeInProcess.UseSubmitBehavior = false;

            Approve = new Button();
            Approve.Text = "Согласовать";
            Approve.Font.Bold = true;
            Approve.Click += new EventHandler(Approve_Click);
            //Approve.Width = 189;
            Approve.BackColor = Color.LimeGreen;
            Approve.Style.Add("margin", "0");
            Approve.Style.Add("width", "calc(50% - 6px)");
            Approve.Style.Add("box-sizing", "border-box");
            Approve.UseSubmitBehavior = false;

            Decline = new Button();
            Decline.Text = "Отклонить";
            Decline.Click += new EventHandler(Decline_Click);
            Decline.BackColor = Color.Tomato;
            Decline.Font.Bold = true;
            //Decline.Width = 189;
            Decline.UseSubmitBehavior = false;
            Decline.Style.Add("width", "calc(50% - 6px)");
            Decline.Style.Add("box-sizing", "border-box");

            firstrow = new LiteralControl();
            beforerow = new LiteralControl();
            sendrow = new LiteralControl();
            sendsplitrow = new LiteralControl();
            midrow = new LiteralControl();
            afterrow = new LiteralControl();
            p = new Panel();
            p.GroupingText = "Задача";
            p.Visible = false;
            p.Style.Add("width", "100%");

            InProcessUserPicker = new PeopleEditor();
            InProcessUserPicker.MultiSelect = false;
            InProcessUserPicker.Visible = false;
            InProcessUserPicker.Width = 260;
            InProcessUserPicker.PlaceButtonsUnderEntityEditor = false;
            SendInProcess = new Button();
            SendInProcess.Text = "Назначить";
            SendInProcess.Font.Bold = true;
            SendInProcess.Click += SendInProcess_Click;
            SendInProcess.Width = 98;
            SendInProcess.UseSubmitBehavior = false;
            SendInProcess.Style.Add("margin", "0");
            SendInProcess.Visible = false;
            firstrow.Text = "<table width='100%'><tr><td colsan='3'>";
            p.Controls.Add(firstrow);
            p.Controls.Add(TakeInProcess);
            p.Controls.Add(sendsplitrow);
            p.Controls.Add(InProcessUserPicker);
            p.Controls.Add(sendrow);
            p.Controls.Add(SendInProcess);
            p.Controls.Add(beforerow);

            p.Controls.Add(Approve);
            //p.Controls.Add(new LiteralControl("</td><td colspan='2'>"));
            p.Controls.Add(Decline);

            p.Controls.Add(midrow);
            UserPicker = new PeopleEditor();
            UserPicker.MultiSelect = false;
            UserPicker.Visible = false;
            UserPicker.Width = 260;
            UserPicker.PlaceButtonsUnderEntityEditor = false;
            p.Controls.Add(UserPicker);
            p.Controls.Add(afterrow);
            Delegate = new Button();
            Delegate.Text = "Отправить";
            Delegate.Font.Bold = true;
            Delegate.Click += Delegate_Click;
            Delegate.Width = 98;
            Delegate.UseSubmitBehavior = false;
            Delegate.Style.Add("margin", "0");
            Delegate.Visible = false;
            Delegate.BackColor = Color.Yellow;
            p.Controls.Add(Delegate);

            commentrow = new LiteralControl("</td></tr><tr><td colspan='3'><span style='font-weight:bold'>Комментарий</span></td></tr><tr><td colspan='3'>");
            p.Controls.Add(commentrow);
            p.Controls.Add(Comment);

            ErrorLbl = new Label();
            ErrorLbl.ForeColor = Color.Red;
            p.Controls.Add(new LiteralControl("</td></tr></table>"));
            p.Controls.Add(ErrorLbl);

            panel = new UpdatePanel();
            panel.UpdateMode = UpdatePanelUpdateMode.Conditional;

            panel.ID = this.ID + "_panel";
            Controls.Add(panel);

            prog = new UpdateProgress();
            prog.AssociatedUpdatePanelID = panel.ID;

            panel.ContentTemplateContainer.Controls.Add(prog);
            Panel pn = new Panel();
            System.Web.UI.WebControls.Image progimg = new System.Web.UI.WebControls.Image();
            progimg.ImageAlign = ImageAlign.Middle;
            progimg.ImageUrl = "/_layouts/15/images/progressbar.gif";
            pn.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
            pn.Controls.Add(progimg);
            prog.Controls.Add(pn);

            if (HowToWork == LoadType.Синхронно)
            {
                Controls.Add(p);
                GetTask();
            }
            else if (HowToWork == LoadType.Асинхронно)
            {
                Timer timercontrol = new Timer();
                timercontrol.Interval = 500;
                timercontrol.Tick += new EventHandler<EventArgs>(timercontrol_Tick);
                panel.ContentTemplateContainer.Controls.Add(p);
                panel.ContentTemplateContainer.Controls.Add(timercontrol);
            }
            else
            {
                panel.ContentTemplateContainer.Controls.Add(p);
                Button b = new Button();
                b.Click += new EventHandler(btn_Click);
                b.Style.Add(HtmlTextWriterStyle.BackgroundImage, "/_layouts/images/icshow_docs.png");
                b.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                b.Width = 200;
                b.Height = 40;
                b.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
                pn = new Panel();
                pn.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
                pn.Controls.Add(b);
                panel.ContentTemplateContainer.Controls.Add(pn);
            }
        }

        void SendInProcess_Click(object sender, EventArgs e)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        SPList СurList = oWeb.Lists[SPContext.Current.List.Title];
                        SPListItem curitem = СurList.GetItemById(SPContext.Current.Item.ID);
                        oWeb.AllowUnsafeUpdates = true;
                        using (EventReceiverManager ev = new EventReceiverManager(true))
                        {
                            string login = ((PickerEntity)InProcessUserPicker.Entities[0]).Key;
                            if (login.Contains("|"))
                                login = login.Substring(login.IndexOf("|") + 1);
                            curitem[InProcessFieldName] = oWeb.EnsureUser(login);
                            curitem.SystemUpdate(false);
                            ev.StartEventReceiver();
                        }
                    }
                }
            });
            Page.Response.Redirect(this.Page.Request.Url.AbsoluteUri);
        }

        void TakeInProcess_Click(object sender, EventArgs e)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        SPList СurList = oWeb.Lists[SPContext.Current.List.Title];
                        SPListItem curitem = СurList.GetItemById(SPContext.Current.Item.ID);
                        oWeb.AllowUnsafeUpdates = true;
                        using (EventReceiverManager ev = new EventReceiverManager(true))
                        {
                            string login = SPContext.Current.Web.CurrentUser.LoginName;
                            if (login.Contains("|"))
                                login = login.Substring(login.IndexOf("|") + 1);
                            curitem[InProcessFieldName] = oWeb.EnsureUser(login);
                            curitem.SystemUpdate(false);
                            ev.StartEventReceiver();
                        }
                    }
                }
            });
            Page.Response.Redirect(this.Page.Request.Url.AbsoluteUri);
        }

        void Delegate_Click(object sender, EventArgs e)
        {
            if (!CheckComment())
                return;
            UserPicker.Validate();
            if (!UserPicker.IsValid || UserPicker.Entities.Count == 0)
            {
                ExceptionHelper.DUmpException(null, "* Необходимо указать пользователя.", this);
                return;
            }
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        SPList СurList = oWeb.Lists[SPContext.Current.List.Title];
                        SPListItem curitem = СurList.GetItemById(SPContext.Current.Item.ID);
                        using (EventReceiverManager ev = new EventReceiverManager(true))
                        {
                            string login = ((PickerEntity)UserPicker.Entities[0]).Key;
                            if (login.Contains("|"))
                                login = login.Substring(login.IndexOf("|") + 1);

                            curitem[DelegateUserFieldName] = oWeb.EnsureUser(login); //new SPFieldUserValue(oWeb, oWeb.SiteUsers[login].ID, login); 
                            curitem.SystemUpdate(false);
                            if (ChangeChild)
                            {
                                // CompleteChild(curitem, Result);
                            }
                            ev.StartEventReceiver();
                        }
                        //oldstatus = curitem["Статус рабочего процесса"] == null ? "" : curitem["Статус рабочего процесса"].ToString();
                        GetTask();
                        wf.Delegate(Comment.Text, int.Parse(DelegateTaskStage));
                        oWeb.AllowUnsafeUpdates = true;
                    }
                }
            });
            Page.Response.Redirect(this.Page.Request.Url.AbsoluteUri);
        }

        public void GetTask()
        {
            SPListItem currentItem = SPContext.Current.Item as SPListItem;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite oSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb oWeb = oSite.OpenWeb())
                    {
                        oWeb.AllowUnsafeUpdates = true;
                        SPList oLst = oWeb.Lists[SPContext.Current.List.Title];
                        if (Page.Request.QueryString["ID"] != null)
                        {
                            SPListItem oItem = oLst.GetItemById(int.Parse(Page.Request.QueryString["ID"]));
                            if (oItem["Текущий исполнитель"] != null && oItem["Текущий исполнитель"].ToString().Contains(SPContext.Current.Web.CurrentUser.Name))
                            {
                                wf = new Workflow(oItem);
                                if (wf.InProgress && wf.GetCurrentStep(SPContext.Current.Web.CurrentUser))
                                {

                                    if (!string.IsNullOrEmpty(InProcessWFStatuses) && !string.IsNullOrEmpty(InProcessFieldName) &&
                                        (oItem[InProcessFieldName] == null || oItem[InProcessFieldName].ToString() == "") &&
                                        InProcessWFStatuses.Contains(wf.CurrentUser.UserStep.StageName))
                                    {
                                        TakeInProcess.Visible = true;
                                        firstrow.Text = "<table><tr><td colspan='3'>";
                                        beforerow.Text = "</td></tr><tr><td width='50'>";
                                        if (!string.IsNullOrEmpty(InProcessGroupName) && SPContext.Current.Web.SiteGroups[InProcessGroupName].ContainsCurrentUser)
                                        {
                                            SendInProcess.Visible = true;
                                            InProcessUserPicker.Visible = true;
                                            sendsplitrow.Text = "</td></tr><tr><td colspan='2' heigth='0'>";
                                            sendrow.Text = "</td><td valign='top' width = '98' heigth='0'>";
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(DelegateTaskName) && wf.CurrentUser.TaskType == DelegateTaskName)
                                    {
                                        Delegate.Visible = true;
                                        UserPicker.Visible = true;
                                        midrow.Text = "</td></tr><tr><td colspan='2' heigth='0'>";
                                        afterrow.Text = "</td><td valign='top' width = '98' heigth='0'>";
                                    }
                                    else if (!string.IsNullOrEmpty(ApprovalTaskName) && wf.CurrentUser.TaskType == ApprovalTaskName)
                                    {
                                    }
                                    else if (!string.IsNullOrEmpty(CompleteTaskName) && wf.CurrentUser.TaskType == CompleteTaskName)
                                    {
                                        Decline.Visible = false;
                                        Approve.Text = "Сохранить и продолжить";
                                    }
                                    else
                                    {
                                        //annotation.Text = "<br/>Ваш документ не был согласован. Внесите изменения и нажмите кнопку «Продолжить». Или закончите согласование нажав кнопку «Прервать»</br></br><table><tr><td>";
                                        Approve.Text = "Продолжить";
                                        Decline.Text = "Прекратить";
                                    }
                                    showWP = true;
                                }
                            }
                            if (showWP)
                            {
                                p.Visible = true;
                                //this.ChromeType = PartChromeType.TitleAndBorder;
                                this.Title = "Согласовать запрос";
                                // this.Width = "512";
                            }
                            else
                                this.ChromeType = PartChromeType.None;
                        }
                    }
                }
            });
        }

        void btn_Click(object sender, EventArgs e)
        {
            GetTask();
            panel.Update();
            ((Button)sender).Visible = false;
        }

        void timercontrol_Tick(object sender, EventArgs e)
        {
            GetTask();
            Timer tmr = sender as Timer;
            tmr.Enabled = false;
            prog.AssociatedUpdatePanelID = panel.ID;
            panel.Update();
        }

        void Decline_Click(object sender, EventArgs e)
        {
            if (CheckComment())
            {
                CompleteTask(false);
            }
        }

        void Approve_Click(object sender, EventArgs e)
        {
            if (CheckRequiered())
            {
                CompleteTask(true);
            }
            else
            {
                ExceptionHelper.DUmpException(null, "Fill the required fields.", this);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        private bool CheckComment()
        {
            bool result = Comment.Text != "";
            if (!result)
                ExceptionHelper.DUmpException(null, "Comment is required.", this);
            else
                ErrorLbl.Text = "";
            return result;
        }

        private bool CheckRequiered()
        {
            bool fieldsFilled = true;
            if (!string.IsNullOrEmpty(StatusFilterString) && !string.IsNullOrEmpty(RequiredFieldsString))
            {
                SPListItem cItem = SPContext.Current.ListItem;
                int index = 0;
                foreach (string s in StatusFilterString.Split('|'))
                {
                    if (cItem["Статус рабочего процесса"] != null && cItem["Статус рабочего процесса"].ToString() != "" &&
                        cItem["Статус рабочего процесса"].ToString().ToLower() == s.ToLower())
                    {
                        string fieldstocheck = RequiredFieldsString.Split('|')[index];
                        foreach (string f in fieldstocheck.Split(';'))
                        {
                            if (cItem.ParentList.Fields.ContainsField(f)
                                && (cItem[f] == null || cItem[f].ToString() == ""))
                                fieldsFilled = false;
                        }

                    }
                    index++;
                }
            }
            return fieldsFilled;
        }

        private void CompleteTask(bool Result)
        {
            try
            {
                GetTask();
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite oSite = new SPSite(SPContext.Current.Web.Url))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            SPList СurList = oWeb.Lists[SPContext.Current.List.Title];
                            SPListItem curitem = СurList.GetItemById(SPContext.Current.Item.ID);
                            oldstatus = curitem["Статус рабочего процесса"] == null ? "" : curitem["Статус рабочего процесса"].ToString();

                            wf.EndStep(Result, Comment.Text);
                            oWeb.AllowUnsafeUpdates = true;
                            using (EventReceiverManager ev = new EventReceiverManager(true))
                            {
                                if (!string.IsNullOrEmpty(InProcessFieldName))
                                {
                                    curitem[InProcessFieldName] = null;
                                }
                                if (wasntApproved)
                                {
                                    if (!Result)
                                    {
                                        curitem["Не было согласовано"] = true;
                                    }
                                    else if (Approve.Text == "Продолжить")
                                    {
                                        curitem["Не было согласовано"] = false;
                                    }
                                }

                                curitem.SystemUpdate(false);
                                if (ChangeChild)
                                {
                                   // CompleteChild(curitem, Result);
                                }

                                RFCEntity rfcEntity = new RFCEntity(curitem);
                                rfcEntity.Tasks.CompleteCurrentUserTask(SPContext.Current.Web.CurrentUser, Result,Comment.Text);

                                ev.StartEventReceiver();
                            }
                        }
                    }
                });
                Page.Response.Redirect(this.Page.Request.Url.AbsoluteUri);

            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex, ex.Message, this);
            }
        }

        public void CompleteChild(SPListItem oItem, bool result)
        {
            SPList mainList = oItem.ParentList;
            SPListItem mainItem = mainList.GetItemById(oItem.ID);
            string status = mainItem["Статус рабочего процесса"].ToString();
            if (!string.IsNullOrEmpty(ChildLib))
            {
                string ViewLists = "";
                string ViewFields = "";

                SPList ChildList = null;
                foreach (string str in ChildLib.Split(';'))
                {
                    ChildList = oItem.Web.Lists[str];
                    ViewLists += "<List ID='" + ChildList.ID.ToString() + "'/>";
                }

                string camlexSt = "";
                if (SearchById)
                {
                    camlexSt = "<Where><And><And><Eq><FieldRef Name='" + ChildList.Fields["Статус рабочего процесса"].InternalName + "'/><Value Type='Text'>" + oldstatus +
                    "</Value></Eq><Eq><FieldRef Name='IsDocumentSet'/><Value Type='Boolean'>1</Value></Eq></And>" +
                    "<Contains><FieldRef Name='" + ChildList.Fields[KeyField].InternalName + "' LookupId='TRUE'/><Value Type='Lookup'>" + mainItem.ID.ToString() + "</Value></Contains>" +
                    "</And></Where><OrderBy><FieldRef Name='ID' /></OrderBy>";
                }
                else
                {
                    camlexSt = "<Where><And><And><Eq><FieldRef Name='" + ChildList.Fields["Статус рабочего процесса"].InternalName + "'/><Value Type='Text'>" + oldstatus +
                   "</Value></Eq><Eq><FieldRef Name='IsDocumentSet'/><Value Type='Boolean'>1</Value></Eq></And>" +
                   "<Contains><FieldRef Name='" + ChildList.Fields[KeyField].InternalName + "'/><Value Type='Text'>" + mainItem[ChildKeyField].ToString() + "</Value></Contains>" +
                   "</And></Where><OrderBy><FieldRef Name='ID' /></OrderBy>";
                }

                ViewLists = "<Lists>" + ViewLists + "</Lists>";
                ViewFields += "<FieldRef Name='Title' Nullable='TRUE'/>";

                SPSiteDataQuery sdq = new SPSiteDataQuery();
                sdq.Lists = ViewLists;
                sdq.Query = camlexSt;
                sdq.ViewFields = ViewFields;
                sdq.Webs = "<Webs Scope='SiteCollection'/>";
                DataTable dt = oItem.Web.GetSiteData(sdq);
                foreach (DataRow dr in dt.Rows)
                {
                    SPList addlist = oItem.Web.Lists[new Guid(dr["ListId"].ToString())];
                    SPListItem additem = addlist.GetItemById(int.Parse(dr["ID"].ToString()));
                    Workflow wf = new Workflow(additem);
                    if (wf.InProgress && wf.GetCurrentStep(SPContext.Current.Web.CurrentUser))
                    {
                        wf.EndStep(result, Comment.Text);
                    }
                }
            }
        }
    }
}
