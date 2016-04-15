using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint.Workflow;

using CamlexNET;
using RedSys.RFC.Core.Helper;

namespace CustomDispFormWebPart.CustomDispWebPart
{
    [ToolboxItemAttribute(false)]
    public class CustomDispWebPart : WebPart
    {
        bool _wpEnableEdit;
        bool _wpOpenInNewWindow;
        bool _wpShowTitle;
        bool _wpShowContentTypeAndDate;
        bool _wpShowWorkflowStatus;
        string _wpDisplayFieldList;


        public CustomDispWebPart()
        {
            _wpEnableEdit = true;
            _wpOpenInNewWindow = true;
            _wpShowTitle = true;
            _wpShowContentTypeAndDate = true;
            _wpShowWorkflowStatus = true;
        }

        #region Properties
        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Разрешить изменения"),
        WebDescription("Разрешить изменения")]
        public bool EnableEdit
        {
            get { return _wpEnableEdit; }
            set { _wpEnableEdit = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Редактировать в новом окне"),
        WebDescription("Требуется JScript функция OpenDialog")]
        public bool OpenInNewWindow
        {
            get { return _wpOpenInNewWindow; }
            set { _wpOpenInNewWindow = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Показывать Наименование"),
        WebDescription("Показывать Наименование")]
        public bool ShowTitle
        {
            get { return _wpShowTitle; }
            set { _wpShowTitle = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Показывать Тип контента и Дату документа"),
        WebDescription("Показывать Тип контента и Дату документа")]
        public bool ShowContentTypeAndDate
        {
            get { return _wpShowContentTypeAndDate; }
            set { _wpShowContentTypeAndDate = value; }
        }

        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Показывать статус Рабочего процесса"),
        WebDescription("Показывать статус Рабочего процесса")]
        public bool ShowWorkflowStatus
        {
            get { return _wpShowWorkflowStatus; }
            set { _wpShowWorkflowStatus = value; }
        }


        [WebBrowsable(true),
        Personalizable(PersonalizationScope.Shared),
        DefaultValue(true),
        Category("Отображение"),
        WebDisplayName("Список полей (разделитель ;)"),
        WebDescription("Оставить пустым если по умолчанию")]
        public string DisplayFieldList
        {
            get { return _wpDisplayFieldList; }
            set { _wpDisplayFieldList = value; }
        }
        #endregion


        public string GetWFStatus(int intStatus)
        {
            string stStatus = null;
            switch (intStatus)
            {
                case 0: stStatus = "Не запущен"; break;
                case 1: stStatus = "Ошибка при запуске"; break;
                case 2: stStatus = "В процессе"; break;
                case 3: stStatus = "Ошибка"; break;
                case 4: stStatus = "Остановлен пользователем"; break;
                case 5: stStatus = "Завершен"; break;
                case 6: stStatus = "Ошибка при запуске. Попытка перезапуска..."; break;
                case 7: stStatus = "Ошибка. Попытка перезапуска..."; break;
                case 8: stStatus = "ViewQueryOverflow"; break;
                case 15: stStatus = "Max"; break;
                default: stStatus = "n/a"; break;
            }

            return stStatus;
        }
        public SPListItemCollection GetItemsByFilterExpression(SPWeb web, string listName, string filterString)
        {
            var expressions = new List<Expression<Func<SPListItem, bool>>>();

            SPQuery search = new SPQuery();
            search.ViewAttributes = "Scope=\"RecursiveAll\"";
            SPListItemCollection resultItems = null;
            SPList tList = web.Lists[listName];

            string[] filterPairs = filterString.Split(',');

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
                        default: expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]).Contains(fieldValue)); break;
                    }
                }

                search.Query = Camlex.Query().WhereAll(expressions).ToString();
                resultItems = tList.GetItems(search);
            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex);
            }

            return resultItems;
        }

        protected override void CreateChildControls()
        {
            this.Style.Add("padding", "0px");
            try
            {
                Table tbl = new Table();
                tbl.Style.Add("border-collapse", "collapse");
                tbl.CssClass = "ms-formtable";
                SPListItem idCurItem = SPContext.Current.List.GetItemById(SPContext.Current.ListItem.ID);
                DocumentSet curDocSet = DocumentSet.GetDocumentSet(idCurItem.Folder);
				SPListItem curDocSetItem = curDocSet.Item;

				if (ShowTitle)
                {
                    Label titleLb = new Label();
                    titleLb.Text = curDocSetItem.Title;
                    titleLb.Font.Bold = true;
                    titleLb.Font.Size = 14;
                    this.Controls.Add(titleLb);
                }

                if (ShowContentTypeAndDate)
                {
                    TableRow ctrow = new TableRow();
                    ctrow.Style.Add(" border-bottom", "1pt solid lightgray");
                    Label ctNameLabel = new Label();
                    ctNameLabel.Text = "Тип";
                    ctNameLabel.Font.Bold = true;
                    TableCell ctNameCell = new TableCell();
                    ctNameCell.Style.Add("padding", "0px");
                    ctNameCell.CssClass = "ms-formlabel";
                    ctNameCell.Controls.Add(ctNameLabel);

                    Label ctLabel = new Label();
                    ctLabel.ID = "ContentTypeLabel";
                    ctLabel.Text = curDocSet.ContentType.Name;
                    TableCell ctCell = new TableCell();
                    ctCell.Style.Add("padding", "0px");
                    ctCell.Controls.Add(ctLabel);

                    ctrow.Cells.Add(ctNameCell);
                    TableCell ctSpaceSell = new TableCell();
                    ctSpaceSell.Style.Add("padding", "0px");
                    ctSpaceSell.Width = System.Web.UI.WebControls.Unit.Pixel(20);
                    ctrow.Cells.Add(ctSpaceSell);
                    ctrow.Cells.Add(ctCell);

                    tbl.Rows.Add(ctrow);
                }

                List<SPField> fldsToDisp = new List<SPField>();
				SPFieldCollection curDocSetFieldCollection = curDocSetItem.Fields;

                if (!string.IsNullOrEmpty(DisplayFieldList))
                {
                    string[] flds = DisplayFieldList.Trim().Split(new[] { ';' },StringSplitOptions.RemoveEmptyEntries);

                    foreach (string fld in flds)
                        fldsToDisp.Add(curDocSetFieldCollection[fld]);
                }
                else
                {
                    foreach (SPField fld in curDocSet.ContentTypeTemplate.WelcomePageFields)
                        fldsToDisp.Add(fld);
                }

                foreach (SPField fld in fldsToDisp)
                {
                    try
                    {
                        TableRow row = new TableRow();
                        row.Style.Add(" border-bottom", "1pt solid lightgray");

                        TableCell cell1 = new TableCell();
                        cell1.Style.Add("padding", "0px");
                        Label fldCaption = new Label();
                        fldCaption.Text = fld.Title;
                        fldCaption.Font.Bold = true;
                        cell1.Controls.Add(fldCaption);

                        cell1.CssClass = "ms-formlabel";
                        row.Cells.Add(cell1);

                        TableCell spaceSell = new TableCell();
                        spaceSell.Style.Add("padding", "0px");
                        spaceSell.Width = System.Web.UI.WebControls.Unit.Pixel(20);
                        row.Cells.Add(spaceSell);


                        if (curDocSetItem[fld.Title] != null)
                        {
                            TableCell cell2 = new TableCell();
                            cell2.Style.Add("padding", "0px");

                            switch (fld.TypeAsString)
                            {

                                case "Number": Label numLabel = new Label();
                                    numLabel.Text = String.Format("{0:N}", curDocSetItem[fld.Title]);
                                    cell2.Controls.Add(numLabel);
                                    break;

                                case "Boolean": Label boolLabel = new Label();
                                    boolLabel.Text = (bool)curDocSetItem[fld.Title] ? "Да" : "Нет";
                                    cell2.Controls.Add(boolLabel);
                                    break;

                                case "DateTime": Label dateLabel = new Label();
                                    dateLabel.Text = Convert.ToDateTime(curDocSetItem[fld.Title]).ToShortDateString();
                                    cell2.Controls.Add(dateLabel);
                                    break;

                                case "Lookup":
                                    cell2.Controls.Add(new LiteralControl(curDocSetItem[fld.Title].ToString().Split('#')[1]));
                                    break;

                                case "PSELookup":
                                case "LookupFieldWithPicker":
                                    SPLinkButton lupLink = new SPLinkButton();
                                    SPFieldLookup lupFld = (SPFieldLookup)fld;

                                    SPList lupList = SPContext.Current.Web.Lists[new Guid(lupFld.LookupList)];
                                    SPFieldLookupValue lupvalue = (SPFieldLookupValue)curDocSetItem[fld.Title];
                                    lupLink.Text = lupList.GetItemById(lupvalue.LookupId).Title;
                                    if (lupList.GetItemById(lupvalue.LookupId).Fields.ContainsField("IsDocumentSet") && (bool)lupList.GetItemById(lupvalue.LookupId)["IsDocumentSet"] == true)
                                        lupLink.NavigateUrl = "/" + lupList.GetItemById(lupvalue.LookupId).Url + "?Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                    else
                                        lupLink.NavigateUrl = lupList.DefaultDisplayFormUrl + "?ID=" + curDocSetItem[fld.Title].ToString().Split(';')[0] + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                    cell2.Controls.Add(lupLink);
                                    break;

                                case "User":
                                    SPLinkButton userLink = new SPLinkButton();
                                    SPFieldUser userFld = (SPFieldUser)fld;
                                    SPList userList = SPContext.Current.Web.Lists[new Guid(userFld.LookupList)];
                                    userLink.Text = curDocSetItem[fld.Title].ToString().Split('#')[1];
                                    userLink.NavigateUrl = userList.DefaultDisplayFormUrl + "?ID=" + curDocSetItem[fld.Title].ToString().Split(';')[0] + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                    cell2.Controls.Add(userLink);
                                    break;
                                case "UserMulti":
                                    SPFieldUser userMultiFld = (SPFieldUser)fld;
                                    userList = SPContext.Current.Web.Lists[new Guid(userMultiFld.LookupList)];
                                    
                                        SPFieldUserValueCollection uvalues = new SPFieldUserValueCollection(SPContext.Current.Web, curDocSetItem[fld.Title].ToString());
                                        foreach (SPFieldUserValue uvalue in uvalues)
                                        {
                                            userLink = new SPLinkButton();
                                            userLink.Text = uvalue.User.Name + " ";
                                            userLink.NavigateUrl = userList.DefaultDisplayFormUrl + "?ID=" + uvalue.User.ID + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                            cell2.Controls.Add(userLink);
                                        }
                                   
                                    break;
                                case "LookupFieldWithPickerMulti":
                                    SPFieldLookup lupMultiFld = (SPFieldLookup)fld;
                                    SPFieldLookupValueCollection values = (SPFieldLookupValueCollection)curDocSetItem[fld.Title];
                                    SPList lupMultiList = SPContext.Current.Web.Lists[new Guid(lupMultiFld.LookupList)];
                                    foreach (SPFieldLookupValue value in values)
                                    {
                                        SPLinkButton lupMultiLink = new SPLinkButton();
                                        lupMultiLink.Text = value.LookupValue + " ";
                                        lupMultiLink.NavigateUrl = lupMultiList.DefaultDisplayFormUrl + "?ID=" + value.LookupId.ToString() + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                        cell2.Controls.Add(lupMultiLink);
                                        cell2.Controls.Add(new LiteralControl("; "));
                                    }
                                    break;

                                default: cell2.Controls.Add(new LiteralControl(curDocSetItem[fld.Title].ToString()));
                                    break;
                            }

                            cell2.CssClass = "ms-formbody";
                            row.Cells.Add(cell2);
                        }

                        tbl.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHelper.DUmpException(new Exception ("RETHROWS " + Page.Request .RawUrl, ex));
                    }
                }
                
                if (ShowWorkflowStatus)
                {
                    foreach (SPWorkflow wf in curDocSetItem.Workflows)
                    {
                        TableCell wf_cell = new TableCell();
                        TableRow wfrow = new TableRow();

                        wfrow.Style.Add(" border-bottom", "1pt solid lightgray");
                        wf_cell.Style.Add("padding", "0px");

                        Label wf_Caption = new Label();
                        wf_Caption.Text = wf.ParentAssociation.Name;
                        wf_Caption.Font.Bold = true;
                        wf_cell.Controls.Add(wf_Caption);

                        wf_cell.CssClass = "ms-formlabel";
                        wfrow.Cells.Add(wf_cell);

                        TableCell wfspaceCell = new TableCell();
                        wfspaceCell.Width = Unit.Pixel(20);
                        wfspaceCell.Style.Add("padding", "0px");

                        wfrow.Cells.Add(wfspaceCell);

                        TableCell wfLinkCell = new TableCell();
                        SPLinkButton wfLink = new SPLinkButton();
                        int statusIndex = Convert.ToInt16(curDocSetItem[wf.ParentAssociation.Name]);
                        wfLink.Text = GetWFStatus(statusIndex);
                        wfLinkCell.Style.Add("padding", "0px");
                        wfLink.NavigateUrl = "/_layouts/15/WrkStat.aspx?List=" + curDocSet.ParentList.ID.ToString() + "&WorkflowInstanceID=" + wf.InstanceId.ToString() + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());
                        wfLinkCell.Controls.Add(wfLink);

                        wfrow.Cells.Add(wfLinkCell);

                        tbl.Rows.Add(wfrow);

                        break;
                    }

                }

                this.Controls.Add(tbl);

                if (EnableEdit)
                {
                    LiteralControl lt1 = new LiteralControl("<br></br>");
                    this.Controls.Add(lt1);

                    SPLinkButton editLink = new SPLinkButton();
                    editLink.Text = "Изменить";
                    editLink.ImageUrl = "/_layouts/15/images/edit.gif";
                    string editLinkText = SPContext.Current.List.DefaultEditFormUrl + "?ID=" + SPContext.Current.ListItem.ID + "&ContentTypeId=" + SPContext.Current.ListItem.ContentTypeId.ToString() + "&Source=" + HttpUtility.UrlEncode(Context.Request.Url.ToString());


                    if (OpenInNewWindow)
                        editLink.OnClientClick = "javascript:OpenPopUpPage('" + editLinkText + "',RefreshOnDialogClose,700,900);return false;";
                    else
                        editLink.NavigateUrl = editLinkText;

                    this.Controls.Add(editLink);
                }
            }
            catch (Exception ex)
            {
                Label lb = new Label();
                lb.Text = ex.Message;
                this.Controls.Add(lb);
            }
        }
    }
}
