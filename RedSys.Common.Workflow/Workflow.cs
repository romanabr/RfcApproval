using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Xml;
using System.Reflection;
using RedSys.Common;
using RedSys.RFC.Core.Helper;

namespace RedSys.Common.Workflow
{
    public class Workflow
    {
        public List<BranchInfo> CompleteUsers { get { return AllUsersList; } }
        public List<BranchInfo> ProcessUsers { get { return CurrentUsers; } }
        protected List<BranchInfo> CurrentUsers;
        protected List<BranchInfo> AllUsersList;
        protected int UserIndex;
        protected bool IsRepeat;
        protected bool SkipPermissions;
        public bool InProgress;
        protected int DelegatedFrom;
        protected Guid ID;

        public BranchInfo CurrentUser;
        protected List<Step> StepsList;
        protected Step CurrentStep;
        protected List<XmlElement> OldWFSData;
        protected SPWeb web;
        protected SPListItem CurItem;
        protected SPUser User;

        public string StoppedBy;
        public string StartedBy;
        public string Initiator { get { return StartedBy; } }
        public DateTime CompleteDate;
        public DateTime StartDate;
        public DateTime InitDate { get { return StartDate; } }
        public bool Result;
        #region UI
        protected Workflow() { }

        public Workflow(SPListItem spli)
        {
            this.InProgress = false;
            web = spli.Web;
            web.AllowUnsafeUpdates = true;
            CurItem = spli;
            CurrentUsers = new List<BranchInfo>();
            AllUsersList = new List<BranchInfo>();
            CurrentUser = new BranchInfo();
            OldWFSData = new List<XmlElement>();
            UserIndex = 0;
            DelegatedFrom = 0;
            CompleteDate = DateTime.MinValue;
            Load();
            StepsList = WFData.GetStages(web, CurItem);
        }

        internal Workflow(SPListItem spli, bool XmlLoad)
        {
            this.InProgress = false;
            web = spli.Web;
            web.AllowUnsafeUpdates = true;
            CurItem = spli;
            CurrentUsers = new List<BranchInfo>();
            AllUsersList = new List<BranchInfo>();
            CurrentUser = new BranchInfo();
            OldWFSData = new List<XmlElement>();
            UserIndex = 0;
            DelegatedFrom = 0;
            CompleteDate = DateTime.MinValue;
            if (XmlLoad)
                Load();
            StepsList = WFData.GetStages(web, CurItem);
        }

        public void StartNew(SPUser user)
        {
            this.InProgress = true;
            this.StoppedBy = "";

            this.ID = Guid.NewGuid();
            AllUsersList = new List<BranchInfo>();
            StepsList = WFData.GetStages(web, CurItem);
            if (StepsList.Count != 0)
            {
                StartDate = DateTime.Now;
                CurrentStep = StepsList[0];
                SkipPermissions = CurrentStep.SkipPermissions;
                SetupStep();
                StartedBy = user.LoginName;
                Save();
            }
        }

        public void RerunStep(string Comment)
        {
            BranchInfo bi = new BranchInfo();
            bi.User = CurrentUser.User;
            bi.Name = "Перенаправлено в другую группу учета";
            bi.RoleName = CurrentUser.RoleName;
            bi.TaskText = CurrentUser.TaskText;
            bi.Comment = Comment;
            bi.Step = 0;
            bi.IsForcedAgreed = true;
            bi.CompleteDate = DateTime.Now;
            bi.DueDate = CurrentUser.DueDate;
            bi.StartDate = CurrentUser.StartDate;
            bi.MoidifiedBy = User.LoginName;
            AllUsersList.Add(bi);
            CurrentUsers = new List<BranchInfo>();
            SPFieldUserValueCollection cusers = new SPFieldUserValueCollection(web, "");
            CurItem["Текущий исполнитель"] = cusers;
            SetupStep();
            Save();
        }

        public void EndStep(bool Result, string comment)
        {
            bool approval = (CurrentUser.IsForcedAgreed) ? true : Result;
            CurrentUser.Approved = approval;
            CurrentUser.Comment = comment;
            CurrentUser.MoidifiedBy = User.LoginName;
            CurrentUser.CompleteDate = DateTime.Now;
            AllUsersList.Add(CurrentUser);
            WorkflowUtils.UpdateTask(CurItem, CurrentUser, approval ? "Согласовано" : "Не согласовано");
            CurrentUser.UserStep.EndTaskMail(CurItem.Web, CurItem, comment, approval, User.LoginName);
            this.Result = approval;
            if (!this.SkipPermissions)
                WorkflowUtils.SetPerm(CurrentUser, AllUsersList, web, CurItem);
            if (this.DelegatedFrom != 0)
            {
                int step = DelegatedFrom;
                Step tempStep = WFData.GetStepByNumber(web, CurItem, step);
                CurrentStep.EndStageMail(web, CurItem, Result, tempStep);
                CurrentStep = tempStep;
                CurItem.Web.AllowUnsafeUpdates = true;
                SetupStep();
                DelegatedFrom = 0;
                IsRepeat = false;
            }
            else if (CurrentUsers[UserIndex].UserStep.AgreementType == AgreementType.Parallel)
            {
                SPFieldUserValueCollection cusers = new SPFieldUserValueCollection(web, CurItem["Текущий исполнитель"].ToString());
                SPFieldUserValueCollection users = WorkflowUtils.RemoveUsersField(cusers, CurrentUser, web);
                CurItem["Текущий исполнитель"] = users;
                bool end = true;
                foreach (BranchInfo bi in CurrentUsers)
                {
                    if (bi.MoidifiedBy == null || bi.MoidifiedBy.Trim() == "")
                        end = false;
                }
                if (end)
                {
                    bool res = true;
                    foreach (BranchInfo bi in CurrentUsers)
                    {
                        if (!bi.Approved)
                            res = false;
                    }
                    this.Result = res;
                    CurrentUsers = new List<BranchInfo>();
                    GetNexStep();
                }
            }
            else
            {
                if (Result)
                {
                    UserIndex++;
                    if (UserIndex >= CurrentUsers.Count)
                    {
                        CurrentUsers = new List<BranchInfo>();
                        GetNexStep();
                    }
                    else
                    {
                        CurrentUser = CurrentUsers[UserIndex];
                        SetFields();
                    }
                }
                else
                {
                    CurrentUsers = new List<BranchInfo>();
                    GetNexStep();
                }
            }
            Save();
        }

        public bool GetCurrentStep(SPUser user)
        {
            User = user;
            bool result = false;
            if (CurrentUsers[0].UserStep.AgreementType == AgreementType.Parallel)
            {
                foreach (BranchInfo br in CurrentUsers)
                {
                    if (br.User.RealUserName == user.LoginName || br.User.UserName == user.LoginName)
                    {
                        CurrentUser = br;
                        result = true;
                    }
                    foreach (UserInfo ui in br.AdditionalUsers)
                    {
                        if (ui.RealUserName == user.LoginName || ui.UserName == user.LoginName)
                        {
                            CurrentUser = br;
                            result = true;
                        }
                    }
                }
            }
            else
            {
                BranchInfo br = CurrentUsers[UserIndex];
                if (br.User.RealUserName == user.LoginName || br.User.UserName == user.LoginName)
                {
                    CurrentUser = br;
                    result = true;
                }
                foreach (UserInfo ui in br.AdditionalUsers)
                {
                    if (ui.RealUserName == user.LoginName || ui.UserName == user.LoginName)
                    {
                        CurrentUser = br;
                        result = true;
                    }
                }
            }
            if (result)
            {
                CurrentStep = CurrentUser.UserStep;
            }
            return result;
        }

        public void Stop(SPUser user)
        {
            InProgress = false;
            StoppedBy = user.LoginName;
            CompleteDate = DateTime.Now;
            User = user;
            WorkflowUtils.CreateCancelRecord(CurItem, user, this);
            if (CurItem.HasUniqueRoleAssignments)
                CurItem.ResetRoleInheritance();
            Save();
        }

        public void AddLSRecord(BranchInfo bi)
        {
            AllUsersList.Add(bi);
            Save();
        }

        public void ReplaceCurrentUser(SPUser olduser, SPUser newuser, bool ChangeUser, bool ChangeTask)
        {
            foreach (BranchInfo br in CurrentUsers)
            {
                if (ChangeTask)
                    WorkflowUtils.ReplaceTaskUser(CurItem, olduser, newuser, br, ChangeUser);
                if (ChangeUser)
                {
                    if (olduser.LoginName.Contains(br.User.RealUserName))
                    {
                        br.User.RealUserName = newuser.LoginName;
                    }
                    if (olduser.LoginName.Contains(br.User.UserName))
                    {
                        br.User.UserName = newuser.LoginName;
                    }
                    foreach (UserInfo ui in br.AdditionalUsers)
                    {
                        if (olduser.LoginName.Contains(ui.RealUserName))
                        {
                            ui.RealUserName = newuser.LoginName;
                        }
                        if (olduser.LoginName.Contains(ui.UserName))
                        {
                            ui.UserName = newuser.LoginName;
                        }
                    }
                }
                else
                {
                    bool add = false;
                    if (olduser.LoginName.Contains(br.User.RealUserName))
                    {
                        add = true;
                    }
                    if (olduser.LoginName.Contains(br.User.UserName))
                    {
                        add = true;
                    }
                    foreach (UserInfo ui in br.AdditionalUsers)
                    {
                        if (olduser.LoginName.Contains(ui.RealUserName))
                        {
                            add = true;
                        }
                        if (olduser.LoginName.Contains(ui.UserName))
                        {
                            add = true;
                        }
                    }
                    if (add)
                    {
                        UserInfo ui = new UserInfo();
                        ui.RealUserName = newuser.LoginName;
                        ui.UserName = newuser.LoginName;
                        br.AdditionalUsers.Add(ui);
                    }
                }
            }
            Save();
        }

        public void Delegate(string comment, int DelegateStepID)
        {
            bool approval = false;
            CurrentUser.Approved = approval;
            CurrentUser.Comment = comment;
            CurrentUser.MoidifiedBy = User.LoginName;
            CurrentUser.CompleteDate = DateTime.Now;
            CurrentUser.Delegted = true;
            AllUsersList.Add(CurrentUser);
            WorkflowUtils.UpdateTask(CurItem, CurrentUser, "Доп. согласование");
            this.Result = approval;
            this.DelegatedFrom = CurrentStep.StageNumber;

            CurrentUsers = new List<BranchInfo>();
            Step tempStep = new Step(DelegateStepID, web);
            CurrentStep.EndStageMail(web, CurItem, Result, tempStep);
            CurrentStep = tempStep;
            CurItem.Web.AllowUnsafeUpdates = true;

            if (CurrentStep == null || CurrentStep.StageNumber == 0)
            {
                Finish();
            }
            else
            {
                SetupStep();
            }
            IsRepeat = false;
            Save();
        }
        #endregion

        #region Core
        protected void GetNexStep()
        {
            int step = (Result) ? CurrentStep.StepNextGood : CurrentStep.StepNexBad;
            Step tempStep = WFData.GetStepByNumber(web, CurItem, step);
            CurrentStep.EndStageMail(web, CurItem, Result, tempStep);
            CurrentStep = tempStep;
            CurItem.Web.AllowUnsafeUpdates = true;

            if (CurrentStep == null || CurrentStep.StageNumber == 0)
            {
                Finish();
            }
            else
            {
                SetupStep();
            }
            IsRepeat = false;
        }

        protected void SetupStep()
        {
            bool finish = false;
            do
            {
                if (CurrentStep.StageType == StageType.Manual)
                {
                    GetStageUsers();
                    if (CurrentUsers.Count == 0)
                    {
                        if (CurrentStep.IfUserNameEmpty)
                        {
                            CurrentStep = StepFinder.GetNexStepPositiveExecuteCode(CurrentStep, web, CurItem);
                        }
                        else
                            throw new Exception("На найдены исполнители на этапе " + CurrentStep.StageName);
                    }
                    else
                    {
                        SetupManualStep();
                        finish = true;
                    }
                }
                else if (CurrentStep.StageType == StageType.Auto)
                {
                    if (CurrentStep.StepNexBad == 0 && CurrentStep.StepNextGood == 0)
                    {
                        CurrentStep.EndStageMail(web, CurItem, Result, null);
                        Finish();
                        finish = true;
                    }
                    else
                    {
                        CompareObjects compareObjects = new CompareObjects();
                        bool StepResult = compareObjects.Compare(CurItem[CurrentStep.CardField], CurrentStep.FieldType, CurrentStep.Operation, CurrentStep.ComparePattern);
                        if (StepResult)
                            CurrentStep = StepFinder.GetNexStepPositiveExecuteCode(CurrentStep, web, CurItem);
                        else
                            CurrentStep = StepFinder.GetNexStepNegativeExecuteCode(CurrentStep, web, CurItem);
                    }
                }
                else
                {
                    Type type = typeof(Common);
                    MethodInfo method = type.GetMethod(CurrentStep.CardField);
                    string[] sargs = CurrentStep.ComparePattern.Split(';');
                    object[] args = new object[sargs.Length];
                    int i = 0;
                    foreach (string s in sargs)
                    {
                        object o = s;
                        if (s.ToLower() == "%item%")
                            o = this.CurItem;
                        bool b = false;
                        if (bool.TryParse(s, out b))
                            o = b;
                        int j = 0;
                        if (int.TryParse(s, out j))
                            o = j;
                        args[i] = o;
                        i++;
                    }
                    bool StepResult = false;
                    try
                    {
                        StepResult = (bool)method.Invoke(null, args);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                            throw ex.InnerException;
                        else
                            throw ex;
                    }
                    if (StepResult)
                        CurrentStep = StepFinder.GetNexStepPositiveExecuteCode(CurrentStep, web, CurItem);
                    else
                        CurrentStep = StepFinder.GetNexStepNegativeExecuteCode(CurrentStep, web, CurItem);
                }
                if (CurrentStep == null || CurrentStep.StageNumber == 0)
                {
                    Finish();
                    finish = true;
                }
            } while (!finish);
        }

        protected void SetupManualStep()
        {
            //foreach (BranchInfo cb in CurrentUsers)
            //    AllUsersList.Add(cb);
            CurrentUser = CurrentUsers[0];
            if (CurrentUser.UserStep.AgreementType == AgreementType.Parallel)
            {
                bool fs = false;
                foreach (BranchInfo br in CurrentUsers)
                {
                    CurrentUser = br;
                    if (!fs)
                    {
                        SetFields();
                        fs = true;
                    }
                    else
                    {
                        SetAddFields();
                    }
                }
            }
            else
            {
                SetFields();
            }
        }

        protected void GetStageUsers()
        {
            CurrentUsers = new List<BranchInfo>();
            UserIndex = 0;
            int ind = -1;
            if (CurrentStep.AgreementType == AgreementType.Successive)
            {
                Users.GetUsers(web, CurItem, CurrentStep, AllUsersList, CurrentUsers);
            }
            else
            {
                if (CurrentStep.DocKind == null)
                {
                    ind = StepsList.FindIndex(r => (r.DocKind == CurrentStep.DocKind && r.DocType.Name == CurrentStep.DocType.Name && r.StageNumber == CurrentStep.StageNumber));
                }
                else
                {
                    ind = StepsList.FindIndex(r => (r.DocKind.Id == CurrentStep.DocKind.Id && r.DocType.Name == CurrentStep.DocType.Name && r.StageNumber == CurrentStep.StageNumber));
                }
                if (ind == -1) return;
                for (int i = ind; i < StepsList.Count; i++)
                {
                    if (StepsList[i].AgreementType == AgreementType.Parallel)
                    {
                        Users.GetUsers(web, CurItem, StepsList[i], AllUsersList, CurrentUsers);
                    }
                    else
                        break;
                }
            }
        }

        protected void Finish()
        {
            this.InProgress = false;
            this.CompleteDate = DateTime.Now;
            CurItem["Текущий исполнитель"] = string.Empty;
            if (CurItem.HasUniqueRoleAssignments)
                CurItem.ResetRoleInheritance();
            if (CurrentStep != null)
                if (CurrentStep.StageType == StageType.Auto)
                    CurItem["Статус рабочего процесса"] = CurrentStep.StageName;
            if (CurItem.Fields.ContainsFieldWithStaticName("StartDate"))
                CurItem["StartDate"] = null;
            if (CurItem.Fields.ContainsField("Срок исполнения"))
                CurItem["Срок исполнения"] = null;
        }
        #endregion

        #region FieldsWork
        protected void SetFields()
        {
            CurrentUser.TaskType = CurrentUser.UserStep.TaskType.Name;
            CurrentUser.DueDate = DateTime.Now.AddBusinessDays(CurrentUser.UserStep.Duration, web.Lists["Выходные"]);
            CurrentUser.IsForcedAgreed = CurrentUser.UserStep.ForcedAgreement;
            CurrentUser.Name = CurrentUser.UserStep.StageName;
            CurrentUser.TaskText = CurrentUser.UserStep.TaskText;
            CurrentUser.StartDate = DateTime.Now;

            if (CurItem.Fields.ContainsField(Constant.StartDateFieldName))
                CurItem[Constant.StartDateFieldName] = DateTime.Now;
            if (CurItem.Fields.ContainsField(Constant.DueDateFieldName) && CurrentUser.UserStep.Duration != 0)
                CurItem[Constant.DueDateFieldName] = CurrentUser.DueDate;
            else if (CurItem.Fields.ContainsField(Constant.DueDateFieldName) && CurrentUser.UserStep.Duration == 0)
                CurItem[Constant.DueDateFieldName] = null;

            CurItem["Статус рабочего процесса"] = CurrentUser.Name;
            SPFieldUserValueCollection users =
                WorkflowUtils.FormUsersField(CurrentUser, web);
            CurItem[Constant.CurrentUsersFieldName] = users;
            if (!this.SkipPermissions)
                WorkflowUtils.SetPerm(CurrentUser, AllUsersList, web, CurItem);
            WorkflowUtils.CreateTask(CurItem, CurrentUser);
            CurrentUser.UserStep.StartTaskMail(CurItem.Web, CurItem, users);
        }

        protected void SetAddFields()
        {
            CurrentUser.TaskType = CurrentUser.UserStep.TaskType.Name;
            CurrentUser.DueDate = DateTime.Now.AddBusinessDays(CurrentUser.UserStep.Duration, web.Lists["Выходные"]);
            CurrentUser.IsForcedAgreed = CurrentUser.UserStep.ForcedAgreement;
            CurrentUser.Name = CurrentUser.UserStep.StageName;
            CurrentUser.TaskText = CurrentUser.UserStep.TaskText;

            if (CurItem.Fields.ContainsField(Constant.DueDateFieldName) && CurrentUser.UserStep.Duration != 0 &&
                (CurItem[Constant.DueDateFieldName] == null || CurItem[Constant.DueDateFieldName].ToString() == "" ||
                DateTime.Compare(DateTime.Parse(CurItem[Constant.DueDateFieldName].ToString()), CurrentUser.DueDate) < 0))
                CurItem[Constant.DueDateFieldName] = CurrentUser.DueDate;



            SPFieldUserValueCollection users =
                new SPFieldUserValueCollection(web, CurItem[Constant.CurrentUsersFieldName] == null ?
                    "" : CurItem[Constant.CurrentUsersFieldName].ToString());
            SPFieldUserValueCollection addusers = WorkflowUtils.AddUsersField(CurrentUser, web);
            if (addusers != null && addusers.Count != 0)
                users.AddRange(addusers);
            CurItem[Constant.CurrentUsersFieldName] = users;
            if (!this.SkipPermissions)
                WorkflowUtils.AddEditPerm(CurrentUser, web, CurItem);
            WorkflowUtils.CreateTask(CurItem, CurrentUser);
            CurrentUser.UserStep.StartTaskMail(CurItem.Web, CurItem, addusers);
        }
        #endregion

        #region xml
        protected void Save()
        {
            XmlDocument doc = new XmlDocument();

            XmlElement wfs = doc.CreateElement("Workflows");
            foreach (XmlElement oldwf in OldWFSData)
            {
                wfs.AppendChild(doc.ImportNode(oldwf, true));
            }
            XmlElement node = doc.CreateElement("Workflow");

            XmlAttribute prop = doc.CreateAttribute("ID");
            prop.Value = ID.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("IsRepeat");
            prop.Value = IsRepeat.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("SkipPermissions");
            prop.Value = SkipPermissions.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("InProgress");
            prop.Value = InProgress.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("UserIndex");
            prop.Value = UserIndex.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("DelegatedFrom");
            prop.Value = DelegatedFrom.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("StoppedBy");
            prop.Value = StoppedBy;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("StartedBy");
            prop.Value = StartedBy;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("StartDate");
            prop.Value = StartDate.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("CompleteDate");
            prop.Value = CompleteDate.ToString();
            node.Attributes.Append(prop);

            XmlElement currb = doc.CreateElement("CurrentUsers");
            foreach (BranchInfo curruser in CurrentUsers)
            {
                currb.AppendChild(curruser.Save(doc, doc.CreateElement("TaskInfo")));
            }
            node.AppendChild(currb);

            XmlElement allb = doc.CreateElement("AllUsersList");
            foreach (BranchInfo allbuser in AllUsersList)
            {
                allb.AppendChild(allbuser.Save(doc, doc.CreateElement("TaskInfo")));
            }
            node.AppendChild(allb);
            wfs.AppendChild(node);
            doc.AppendChild(wfs);


            string s = "";
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw))
                {
                    doc.Save(xw);
                    xw.Flush();
                    s = sw.GetStringBuilder().ToString();
                }
            }

            bool upd = false;
            HandleEventFiring ev = new HandleEventFiring();
            CurItem.Web.AllowUnsafeUpdates = true;
            ev.AccDisableEventFiring();
            do
            {
                try
                {
                    CurItem["WFData"] = s;
                    CurItem.SystemUpdate(false);
                    upd = true;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            while (!upd);
            ev.AccEnableEventFiring();

        }

        protected void Load()
        {
			string wfData = CurItem.GetFieldValue("WFData");
            if (!string.IsNullOrEmpty(wfData))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(wfData);

                XmlElement wfs = doc["Workflows"];
                XmlElement node = null;
                foreach (XmlElement cn in wfs.ChildNodes)
                {
                    if (!bool.Parse(cn.Attributes["InProgress"].Value))
                    {
                        OldWFSData.Add(cn);
                    }
                    node = cn;
                }
                LoadData(doc, node);
            }
        }

        internal void LoadData(XmlDocument doc, XmlNode node)
        {
            this.ID = Guid.Parse(node.Attributes["ID"].Value);
            this.IsRepeat = bool.Parse(node.Attributes["IsRepeat"].Value);
            this.InProgress = bool.Parse(node.Attributes["InProgress"].Value);
            this.SkipPermissions = bool.Parse(node.Attributes["SkipPermissions"].Value);
            this.UserIndex = int.Parse(node.Attributes["UserIndex"].Value);
            if (node.Attributes["DelegatedFrom"] != null && node.Attributes["DelegatedFrom"].Value != null)
                this.DelegatedFrom = int.Parse(node.Attributes["DelegatedFrom"].Value);
            this.StartedBy = node.Attributes["StartedBy"].Value;
            this.StoppedBy = node.Attributes["StoppedBy"].Value;
            this.StartDate = DateTime.Parse(node.Attributes["StartDate"].Value);
            this.CompleteDate = DateTime.Parse(node.Attributes["CompleteDate"].Value);

            if (node["CurrentUsers"] != null)
            {
                foreach (XmlElement cn in node["CurrentUsers"].ChildNodes)
                {
                    BranchInfo bi = new BranchInfo();
                    bi.Load(doc, cn, web);
                    CurrentUsers.Add(bi);
                }
            }

            if (node["AllUsersList"] != null)
            {
                foreach (XmlElement cn in node["AllUsersList"].ChildNodes)
                {
                    BranchInfo bi = new BranchInfo();
                    bi.Load(doc, cn, web);
                    AllUsersList.Add(bi);
                }
            }
        }
        #endregion
    }
}
