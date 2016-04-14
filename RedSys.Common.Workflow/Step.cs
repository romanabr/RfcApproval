using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Administration;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Core.Mail;

namespace RedSys.Common.Workflow
{
    [Serializable]
    public class DocumentType
    {
        //public int Id { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class DocumentKind
    {
        public int Id { get; set; }
        public string Name { get; set; }        
    }

    [Serializable]
    public class Role
    {
        protected string _roleval;
        public void SetVal(SPFieldLookupValueCollection Role)
        {
            _roleval = Role.ToString();
        }
        public SPFieldLookupValueCollection GetRoleVal()
        {
            return new SPFieldLookupValueCollection(_roleval);
        }

        public List<RoleKind> RoleKindList { get; set; }
    }

    [Serializable]
    public class RoleKind
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class TaskType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Serializable]
    public class Step
    {
        #region public variables

        public string CopyFields { get; set; }

        public bool Editpermission { get; set; }

        public bool SkipPermissions { get; set; }

        public int ID { get; set; }
        /// <summary>
        /// Номер
        /// </summary>
        public int StageNumber { get; set; }
        /// <summary>
        /// Тип
        /// </summary>
        public StageType StageType { get; set; }
        /// <summary>
        /// Ожидание
        /// </summary>
        public bool Waiting { get; set; }
        /// <summary>
        /// Тип содержимого
        /// </summary>
        public DocumentType DocType { get; set; }
        /// <summary>
        /// Вид документа
        /// </summary>
        public DocumentKind DocKind { get; set; }
        /// <summary>
        /// Название этапа
        /// </summary>
        public string StageName { get; set; }
        /// <summary>
        /// Роль
        /// </summary>
        public Role Role { get; set; }
        /// <summary>
        /// Длительность
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Принудительное согласование
        /// </summary>
        public bool ForcedAgreement { get; set; }
        /// <summary>
        /// Тип этапа
        /// </summary>
        public AgreementType AgreementType { get; set; }
        /// <summary>
        /// Повторное согласование
        /// </summary>
        public bool AgreementRepeated { get; set; }
        /// <summary>
        /// Исключать повтор согласующего
        /// </summary>
        public bool RepeatedUserAgreement { get; set; }
        /// <summary>
        /// Согласовано
        /// </summary>
        public int StepNextGood { get; set; }
        /// <summary>
        /// Отказ
        /// </summary>
        public int StepNexBad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsListAgreement { get; set; }
        /// <summary>
        /// Текст задачи
        /// </summary>
        public string TaskText { get; set; }
        /// <summary>
        /// Тип задачи
        /// </summary>
        public TaskType TaskType { get; set; }
        /// <summary>
        /// Поле карточки
        /// </summary>
        public string CardField { get; set; }
        /// <summary>
        /// Тип сравнения
        /// </summary>
        public string ComparePattern { get; set; }
        public string Operation { get; set; }
        public string FieldType { get; set; }
        public string UserName { get; set; }
        public string RealUserName { get; set; }
        public bool IfUserNameEmpty { get; set; }
        public bool IsDefaultUser { get; set; }
        public bool CreateTask { get; set; }
        public string AdditionalLists { get; set; }
        public string EmailText { get; set; }
        public string EndEmailText { get; set; }
        public string EndStageText { get; set; }
        public string EndWorkflowText { get; set; }
        #endregion

        #region methods
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Step s = (Step)obj;
            return (DocType == s.DocType && DocKind == s.DocKind && StageNumber == s.StageNumber);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region ctor

        public Step(SPListItem item)
        {
            init(item);
        }

        public Step()
        {

        }

        public Step(int ID, SPWeb web)
        {
            if (ID != 0)
            {
                SPList stageList = web.Lists[Constant.StageListName];
                init(stageList.GetItemById(ID));
            }
        }

        protected void init(SPListItem item)
        {
            this.ID = item.ID;
            if (item[Constant.StageNumber] != null)
                this.StageNumber = (int)item[Constant.StageNumber];
            if (item[Constant.StageType] != null)
                this.StageType = (item.GetFieldValue(Constant.StageType) == Constant.AUTOSTAGE) ? StageType.Auto : (item.GetFieldValue(Constant.StageType) == "Ручной") ? StageType.Manual : StageType.Code;
            this.Waiting = item.GetFieldValueBoolean(Constant.Waiting);
            this.Editpermission = item.ParentList.Fields.ContainsField("Права на карточку") ? item["Права на карточку"] != null ?
                item["Права на карточку"].ToString().Contains("Изменение") : true : true;
            if (item[Constant.DocumentType_Ref] != null)
            {
                this.DocType = new DocumentType
                {
                    Name = (string)item[Constant.DocumentType_Ref]
                    //Id = item.GetFieldValueLookup(Constant.DocumentType_Ref).LookupId,
                    //Name = item.GetFieldValueLookup(Constant.DocumentType_Ref).LookupValue
                };
            }
            if (item.Fields.ContainsField(Constant.DocumentKind) && item[Constant.DocumentKind] != null)
            {
                this.DocKind = new DocumentKind
                {
                    Id = item.GetFieldValueLookup(Constant.DocumentKind).LookupId,
                    Name = item.GetFieldValueLookup(Constant.DocumentKind).LookupValue
                };
            }

            this.StageName = item.GetFieldValue(Constant.StageName);
            if (item[Constant.Role_Name] != null)
            {
                this.Role = new Role();
                Role.SetVal(item.GetFieldValueLookupMulti(Constant.Role_Name));
            }
            if (item[Constant.Duration] != null)
                this.Duration = int.Parse(item[Constant.Duration].ToString());
            if (item[Constant.CopyFields] != null)
                this.CopyFields = (string)item[Constant.CopyFields];
            if (item[Constant.ForcedAgreement] != null)
                this.ForcedAgreement = (bool)item[Constant.ForcedAgreement];
            if (item[Constant.SkipPermissions] != null)
                this.SkipPermissions = (bool)item[Constant.SkipPermissions];
            if (item[Constant.TypeAgreement] != null)
                this.AgreementType = (item[Constant.TypeAgreement].ToString() == Constant.PARALLELSTAGE) ? AgreementType.Parallel : AgreementType.Successive;
            if (item[Constant.RepeatedAgreement] != null)
                this.AgreementRepeated = (bool)item[Constant.RepeatedAgreement];
            if (item[Constant.RepeatedUserAgreement] != null)
                this.RepeatedUserAgreement = (bool)item[Constant.RepeatedUserAgreement];
            if (item[Constant.StepNextGood] != null)
                this.StepNextGood = (int)item[Constant.StepNextGood];
            if (item[Constant.StepNextBad] != null)
                this.StepNexBad = (int)item[Constant.StepNextBad];
            if (item[Constant.ListOfAgreement] != null)
                this.IsListAgreement = (bool)item[Constant.ListOfAgreement];
            if (item[Constant.ContentOfTask] != null)
                this.TaskText = item[Constant.ContentOfTask].ToString();
            if (item[Constant.TypeOfTask_str] != null)
            {
                this.TaskType = new TaskType
                {
                    Id = item.GetFieldValueLookup(Constant.TypeOfTask_str).LookupId,
                    Name = item.GetFieldValueLookup(Constant.TypeOfTask_str).LookupValue
                };
            }

            this.CardField = item.GetFieldValue(Constant.CardField); ;
            this.ComparePattern = item.GetFieldValue(Constant.ComparePattern);
            if (item[Constant.Operation] != null)
            {
                switch (item[Constant.Operation].ToString())
                {
                    case "Начинается с":
                        //   this.Operation = Operation.BeginsWith;
                        this.Operation = "BeginsWith";
                        break;
                    case "Включает":
                        // this.Operation = Operation.Contains;
                        this.Operation = "Contains";
                        break;
                    case "Равно":
                        // this.Operation = Operation.Eq;
                        this.Operation = "Eq";
                        break;
                    case "Больше или равно":
                        //this.Operation = Operation.Geq;
                        this.Operation = "Geq";
                        break;
                    case "Больше":
                        // this.Operation = Operation.Gt;
                        this.Operation = "Gt";
                        break;
                    case "Не нуль":
                        //  this.Operation = Operation.IsNotNull;
                        this.Operation = "IsNotNull";
                        break;
                    case "Нуль":
                        //   this.Operation = Operation.IsNull;
                        this.Operation = "IsNull";
                        break;
                    case "Меньше или равно":
                        //this.Operation = Operation.Leq;
                        this.Operation = "Leq";
                        break;
                    case "Меньше":
                        // this.Operation = Operation.Lt;
                        this.Operation = "Lt";
                        break;
                    case "Не равно":
                        // this.Operation = Operation.Neq;
                        this.Operation = "Neq";
                        break;
                }
            }
            this.FieldType = string.Format("System.{0}", item.GetFieldValue(Constant.FieldType));
            this.IfUserNameEmpty = item.GetFieldValueBoolean(Constant.EmptyUserNameGuid);
            this.IsDefaultUser = item.GetFieldValueBoolean(Constant.IsDefaultUser);
            this.CreateTask = item.GetFieldValueBoolean(Constant.CreateTaskFieldName);
            this.EmailText = item.GetFieldValue("Текст уведомления");

            this.EndEmailText = item.GetFieldValue("Уведомление по завершению задачи");
            this.EndStageText = item.GetFieldValue("Уведомление по завершению этапа");
            this.EndWorkflowText = item.GetFieldValue("Уведомление по завершению процесса");
        }
        #endregion

        #region Method

        public string GetDeputy(SPWeb web, SPUser user)
        {
            string retString = user.LoginName;
            if (user != null)
            {
                SPList spList = web.Lists["Заместители"];
                SPQuery spQuery = new SPQuery();
                spQuery.Query = string.Format("<Where><And><Eq><FieldRef Name=\"Sub\" LookupId=\"True\"/><Value Type=\"Lookup\">{0}</Value></Eq><And><Leq><FieldRef Name=\"Start_Date\"/><Value Type=\"DateTime\">{1}</Value></Leq><Geq><FieldRef Name=\"End_Date\"/><Value Type=\"DateTime\" >{2}</Value></Geq></And></And></Where>", user.ID, DateTime.Now.ToString("s"), DateTime.Now.ToString("s"));
                SPListItemCollection listItemCollection = spList.GetItems(spQuery);
                if (listItemCollection != null && listItemCollection.Count != 0)
                {
                    SPUser deputyUser = listItemCollection[0].GetFieldValueUser(spList.Fields.GetFieldByInternalName("DeputyUser").Title);
                    if (deputyUser == null)
                    {
                        return user.LoginName;
                    }
                    else
                    {
                        return deputyUser.LoginName;
                    }
                }
            }
            return retString;
        }

        public bool StartTaskMail(SPWeb web, SPListItem oItem,SPFieldUserValueCollection to)
        {
            //SPList UsersL = web.Lists["Настройки уведомлений"];
            //SPQuery oq = new SPQuery();
            //oq.Query = "<Where><Eq><FieldRef Name='" + UsersL.Fields["Пользователь"].InternalName + "' LookupId='TRUE' /><Value Type='Integer'>" + web.Users[RealUserName].ID + "</Value></Eq></Where>";
            //SPListItemCollection coll = UsersL.GetItems(oq);
            bool retBool = false;
            //if (coll.Count == 0)
            if (!string.IsNullOrEmpty(this.EmailText) && to != null && to.Count != 0)
                {
                 string toLogin =   string.Join(";", to.Select(user => user.LookupValue));
                    retBool = MailHelper.SendMail(
                     this.GetBody(string.Format("{1} Назначена задача: {0}", this.StageName, oItem.ContentType.Name), web, oItem, string.Empty, false, string.Empty),
                    this.GetBody(this.EmailText, web, oItem, string.Empty, false, string.Empty),
                    true,toLogin , web.Site.ID);
                }
            return retBool;
        }

        public bool EndTaskMail(SPWeb web, SPListItem oItem, string taskcomment, bool isapprove, string modifyby)
        {
            //SPList UsersL = web.Lists["Настройки уведомлений"];
            //SPQuery oq = new SPQuery();
            //oq.Query = "<Where><Eq><FieldRef Name='" + UsersL.Fields["Пользователь"].InternalName + "' LookupId='TRUE' /><Value Type='Integer'>" + web.Users[RealUserName].ID + "</Value></Eq></Where>";
            //SPListItemCollection coll = UsersL.GetItems(oq);
            bool retBool = false;
            //if (coll.Count > 0)
                if (!string.IsNullOrEmpty(this.EndEmailText))
                {
                    retBool = MailHelper.SendMail(
                        this.GetBody(string.Format("{1} Исполнена задача: {0}", this.StageName, oItem.ContentType.Name), web, oItem, taskcomment, isapprove, modifyby),
                       this.GetBody(this.EndEmailText, web, oItem, taskcomment, isapprove, modifyby),
                    true, oItem.GetFieldValueUser("Ответственный") != null ? oItem.GetFieldValueUser("Ответственный").LoginName : oItem.GetFieldValueUser(SPBuiltInFieldId.Author).LoginName, web.Site.ID);
                }
            return retBool;
        }

        public bool EndStageMail(SPWeb web,SPListItem oItem, bool stepresult, Step tempStep)
        {
            bool retBool = false;
            if (tempStep != null)
            {
                if (!string.IsNullOrEmpty(this.EndStageText))
                {

                    retBool = MailHelper.SendMail(
                        this.GetBody(string.Format("{1} Закончен этап: {0}", this.StageName, oItem.ContentType.Name), web, oItem, string.Empty, stepresult, string.Empty),
                         this.GetBody(this.EndStageText, web, oItem, string.Empty, stepresult, string.Empty), true, oItem.GetFieldValueUser("Ответственный") != null ? oItem.GetFieldValueUser("Ответственный").LoginName : oItem.GetFieldValueUser(SPBuiltInFieldId.Author).LoginName, web.Site.ID);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(this.EndWorkflowText))
                    retBool = MailHelper.SendMail(string.Format("{0} Закончен процесс", oItem.ContentType.Name),
                         this.GetBody(this.EndWorkflowText, web, oItem, string.Empty, stepresult, string.Empty), true, oItem.GetFieldValueUser("Ответственный") != null ? oItem.GetFieldValueUser("Ответственный").LoginName : oItem.GetFieldValueUser(SPBuiltInFieldId.Author).LoginName, web.Site.ID);
            }
            return retBool;
        }

        public string GetBody(string body, SPWeb web, SPListItem oItem, string taskcomment, bool isapprove, string modifyby)
        {
            Regex regex = new Regex("(%%Item:).*(%%)|(%%Step:).*(%%)|(%%Common:).*(%%)");
            SPListItem stepItem = web.Lists[Constant.StageListName].GetItemById(this.ID);

            MatchCollection mc = regex.Matches(body);
            foreach (Match m in mc)
            {
                string matchReplace = string.Empty;
                string fieldstring = m.Value.Trim(new char[] { '%' }).Split(new char[] { ':' })[1];

                string fieldvaluestring = string.Empty;
                if (fieldstring.Contains('|'))
                {
                    string[] fieldstrings = fieldstring.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    fieldstring = fieldstrings[0];
                    fieldvaluestring = fieldstrings[1];
                }
                try
                {
                    if (m.Value.ToLower().StartsWith("%%common"))
                    {
                        switch (fieldstring)
                        {
                            case ("TaskUrl"): matchReplace = string.Format("{0}", oItem.Url); break;
                            case ("ItemUrl"):
                                String incomingURL = "";
                                SPWebApplication webApp = oItem.Web.Site.WebApplication;
                                foreach (SPAlternateUrl altUrl in webApp.AlternateUrls)
                                {
                                    if (altUrl.UrlZone == SPUrlZone.Internet)
                                    {
                                        incomingURL = SPUtility.AlternateServerUrlFromHttpRequestUrl(altUrl.Uri).AbsoluteUri + oItem.Web.ServerRelativeUrl.Substring(1) + "/";
                                    }
                                }
                                if (String.IsNullOrEmpty(incomingURL))
                                {
                                    incomingURL = oItem.Web.Url + "/";
                                }
                                matchReplace = incomingURL + oItem.Folder.Url; break;
                            case ("ModifyBy"): matchReplace = modifyby; break;
                            case ("TaskComment"): matchReplace = taskcomment; break;
                            case ("Result"): matchReplace = (isapprove ? fieldvaluestring.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0] : fieldvaluestring.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1]); break;
                            case ("DateTimeNow"): DateTime.Now.ToString("yyyy-MM-dd hh:mm"); break;
                            case ("DateTimeToday"): matchReplace = DateTime.Today.ToShortDateString(); break;
                        }

                    }
                    else if (m.Value.ToLower().StartsWith("%%item"))
                    {
                        matchReplace = oItem.GetFieldValueByType(fieldstring, fieldvaluestring);
                    }
                    else if (m.Value.ToLower().StartsWith("%%step"))
                    {
                        matchReplace = stepItem.GetFieldValueByType(fieldstring, fieldvaluestring);
                    }

                    body = body.Replace(m.Value, matchReplace);
                }
                catch
                {
                }
            }

            return body;
        }

        #endregion
    }

    [Serializable]
    public class RoleDistribution
    {
        public int Id { get; set; }
        public string User { get; set; }
        public Role Role { get; set; }
        public RoleKind RoleKind { get; set; } //одно из 3-х значений
        public List<RoleKind> RoleObject { get; set; }
    }
}
