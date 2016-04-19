using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.DocumentManagement.DocumentSets;
using RedSys.RFC.Core.Helper;
using System.Linq;

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
        WebDisplayName("Список групп и полей (разделитель групп |, разделитель полей ;)"),
        WebDescription("Оставить пустым если по умолчанию, пример заполнения ##groupname;field1;field2;field3|##groupname1;field4;field5")]
        public string DisplayFieldList
        {
            get { return _wpDisplayFieldList; }
            set { _wpDisplayFieldList = value; }
        }
        #endregion


        

        protected override void CreateChildControls()
        {
            this.Style.Add("width","100%");
            try
            {
                Table tbl = new Table();
                tbl.Style.Add("border-collapse", "collapse");
                tbl.CssClass = "ms-formtable";
                SPListItem idCurItem = SPContext.Current.List.GetItemById(SPContext.Current.ListItem.ID);
                DocumentSet curDocSet = DocumentSet.GetDocumentSet(idCurItem.Folder);
				SPListItem curDocSetItem = curDocSet.Item;

                LiteralControl css = new  LiteralControl("<style>fieldset {-moz-border-radius: 4px;border-radius: 4px;-webkit-border-radius: 4px;} legend {font-weight:bold}</style>");
                this.Controls.Add(css);
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

                Dictionary<string, List<SPField>> fldsToDisp = new Dictionary<string, List<SPField>>();
				SPFieldCollection curDocSetFieldCollection = curDocSetItem.Fields;

                if (!string.IsNullOrEmpty(DisplayFieldList))
                {
                    string[] groups = DisplayFieldList.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string gr in groups)
                    {
                        string[] flds = gr.Trim().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (flds.Count() <= 1) continue;
                        string groupname = flds[0].StartsWith("##") ? flds[0].Replace("##", "") : string.Empty;
                        if (!fldsToDisp.ContainsKey(groupname))
                            fldsToDisp.Add(groupname, new List<SPField>());

                        for(int i=1; i<flds.Count();i++)
                            fldsToDisp[groupname].Add(curDocSetFieldCollection.GetField(flds[i]));
                    }
                }
                else
                {
                    fldsToDisp.Add(string.Empty, new List<SPField>());
                    foreach (SPField fld in curDocSet.ContentTypeTemplate.WelcomePageFields)
                        fldsToDisp[string.Empty].Add(fld);
                }
                foreach (KeyValuePair<string,List<SPField>> fieldsetTitle in fldsToDisp)
                {
                    Panel fieldSet = new Panel();
                    if(!string.IsNullOrEmpty(fieldsetTitle.Key))
                    fieldSet.GroupingText = fieldsetTitle.Key;
                    


                    Table fieldSetTable = new Table();
                    fieldSetTable.Style.Add("border-collapse", "collapse");
                    fieldSetTable.CssClass = "ms-formtable";
                    fieldSetTable.Width = new Unit(100, UnitType.Percentage);
                    fieldSet.Controls.Add(fieldSetTable);
                    for (int i = 0; i < fieldsetTitle.Value.Count; i++)
                    {
                        SPField fld = fieldsetTitle.Value[i];
                        try
                        {
                            TableRow row = new TableRow();
                            if(i != fieldsetTitle.Value.Count-1)
                            row.Style.Add("border-bottom", "1pt solid lightgray");

                            TableCell cell1 = new TableCell();
                            cell1.Style.Add("padding-bottom", "6px");
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
                                    case "Number":
                                        Label numLabel = new Label();
                                        numLabel.Text = String.Format("{0:N}", curDocSetItem[fld.Title]);
                                        cell2.Controls.Add(numLabel);
                                        break;

                                    case "Boolean":
                                        Label boolLabel = new Label();
                                        boolLabel.Text = curDocSetItem.GetFieldValueBoolean(fld.Title) ? "Да" : "Нет";
                                        cell2.Controls.Add(boolLabel);
                                        break;

                                    case "DateTime":
                                        Label dateLabel = new Label();
                                        DateTime? dt = curDocSetItem.GetFieldValueDateTime(fld.Title);
                                        if (dt.HasValue)
                                        {
                                            dateLabel.Text = ((SPFieldDateTime)fld).DisplayFormat== SPDateTimeFieldFormatType.DateOnly? dt.Value.ToShortDateString() : dt.Value.ToString();
                                        }
                                        cell2.Controls.Add(dateLabel);
                                        break;

                                    case "Lookup":
                                        SPFieldLookupValue flv = curDocSetItem.GetFieldValueLookup(fld.Title);
                                        cell2.Controls.Add(
                                            new LiteralControl(flv.LookupValue));
                                        break;

                                    case "PSELookup":
                                    case "LookupFieldWithPicker":
                                        SPLinkButton lupLink = new SPLinkButton();
                                        SPFieldLookup lupFld = (SPFieldLookup) fld;

                                        SPList lupList = SPContext.Current.Web.Lists[new Guid(lupFld.LookupList)];
                                        SPFieldLookupValue lupvalue = curDocSetItem.GetFieldValueLookup(fld.Title);
                                        lupLink.Text = lupList.GetItemById(lupvalue.LookupId).Title;
                                        if (
                                            lupList.GetItemById(lupvalue.LookupId).Fields.ContainsField("IsDocumentSet") &&
                                            (bool) lupList.GetItemById(lupvalue.LookupId)["IsDocumentSet"] == true)
                                            lupLink.NavigateUrl = "/" + lupList.GetItemById(lupvalue.LookupId).Url +
                                                                  "?Source=" +
                                                                  HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                        else
                                            lupLink.NavigateUrl = lupList.DefaultDisplayFormUrl + "?ID=" +
                                                                  curDocSetItem[fld.Title].ToString().Split(';')[0] +
                                                                  "&Source=" +
                                                                  HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                        cell2.Controls.Add(lupLink);
                                        break;

                                    case "User":
                                        SPLinkButton userLink = new SPLinkButton();
                                        SPFieldUser userFld = (SPFieldUser) fld;
                                        SPList userList = SPContext.Current.Web.Lists[new Guid(userFld.LookupList)];
                                        userLink.Text = curDocSetItem[fld.Title].ToString().Split('#')[1];
                                        userLink.NavigateUrl = userList.DefaultDisplayFormUrl + "?ID=" +
                                                               curDocSetItem[fld.Title].ToString().Split(';')[0] +
                                                               "&Source=" +
                                                               HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                        cell2.Controls.Add(userLink);
                                        break;
                                    case "UserMulti":
                                        SPFieldUser userMultiFld = (SPFieldUser) fld;
                                        userList = SPContext.Current.Web.Lists[new Guid(userMultiFld.LookupList)];

                                        SPFieldUserValueCollection uvalues =
                                            new SPFieldUserValueCollection(SPContext.Current.Web,
                                                curDocSetItem[fld.Title].ToString());
                                        foreach (SPFieldUserValue uvalue in uvalues)
                                        {
                                            userLink = new SPLinkButton();
                                            userLink.Text = uvalue.User.Name + " ";
                                            userLink.NavigateUrl = userList.DefaultDisplayFormUrl + "?ID=" +
                                                                   uvalue.User.ID + "&Source=" +
                                                                   HttpUtility.UrlEncode(Context.Request.Url.ToString());
                                            cell2.Controls.Add(userLink);
                                        }

                                        break;
                                    case "LookupFieldWithPickerMulti":
                                        SPFieldLookup lupMultiFld = (SPFieldLookup) fld;
                                        SPFieldLookupValueCollection values =
                                            (SPFieldLookupValueCollection) curDocSetItem[fld.Title];
                                        SPList lupMultiList =
                                            SPContext.Current.Web.Lists[new Guid(lupMultiFld.LookupList)];
                                        foreach (SPFieldLookupValue value in values)
                                        {
                                            SPLinkButton lupMultiLink = new SPLinkButton();
                                            lupMultiLink.Text = value.LookupValue + " ";
                                            lupMultiLink.NavigateUrl = lupMultiList.DefaultDisplayFormUrl + "?ID=" +
                                                                       value.LookupId.ToString() + "&Source=" +
                                                                       HttpUtility.UrlEncode(
                                                                           Context.Request.Url.ToString());
                                            cell2.Controls.Add(lupMultiLink);
                                            cell2.Controls.Add(new LiteralControl("; "));
                                        }
                                        break;

                                    default:
                                        cell2.Controls.Add(new LiteralControl(curDocSetItem[fld.Title].ToString()));
                                        break;
                                }

                                cell2.CssClass = "ms-formbody";
                                row.Cells.Add(cell2);
                            }

                            fieldSetTable.Rows.Add(row);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHelper.DUmpException(new Exception("RETHROWS " + Page.Request.RawUrl, ex));
                        }
                    }
                    this.Controls.Add(fieldSet);
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
