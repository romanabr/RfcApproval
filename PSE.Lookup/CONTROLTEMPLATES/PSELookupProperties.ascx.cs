using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PSELookup.Field;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Globalization;
using System.Collections.Generic;


namespace PSELookup.CONTROLTEMPLATES
{
    public partial class PSELookup : UserControl, IFieldEditor
    {
        readonly string[] EXCLUDED_FIELDS = new string[]{
      "_Author","_Category", "_CheckinComment", "_Comments", "_Contributor", "_Coverage", "_DCDateCreated",
      "_DCDateModified", "_EditMenuTableEnd", "_EditMenuTableStart", "_EndDate", "_Format",
      "_HasCopyDestinations", "_IsCurrentVersion", "_LastPrinted", "_Level", "_ModerationComments",
      "_ModerationStatus", "_Photo", "_Publisher", "_Relation", "_ResourceType", "_Revision",
      "_RightsManagement", "_SharedFileIndex", "_Source", "_SourceUrl", "_Status", "ActualWork",
      "AdminTaskAction", "AdminTaskDescription", "AdminTaskOrder","AppAuthor", "AppEditor", "AssignedTo", "Attachments",
      "AttendeeStatus", "Author", "BaseAssociationGuid", "BaseName", "Birthday", "Body",
      "BodyAndMore", "BodyWasExpanded", "Categories", "CheckoutUser", "Comment", "Comments", "Completed",
      "Created_x0020_By", "Created_x0020_Date", "DateCompleted", "DiscussionLastUpdated",
      "DiscussionTitle", "DocIcon", "DueDate", "Editor", "EmailBody", "EmailCalendarDateStamp",
      "EmailCalendarSequence", "EmailCalendarUid", "EndDate", "EventType", "Expires",
      "ExtendedProperties", "fAllDayEvent", "File_x0020_Size", "File_x0020_Type", "FileDirRef",
      "FileLeafRef", "FileRef", "FileSizeDisplay", "FileType","FolderChildCount", "FormData", "FormURN", "fRecurrence",
      "FSObjType", "FullBody", "Group", "GUID", "HasCustomEmailBody", "Hobbies", "HTML_x0020_File_x0020_Type",
      "IMAddress", "ImageCreateDate", "ImageHeight", "ImageSize", "ImageWidth", "Indentation", "IndentLevel",
      "InstanceID", "IsActive", "IsSiteAdmin", "ItemChildCount", "Keywords", "Last_x0020_Modified", "LessLink",
      "LimitedBody", "LinkDiscussionTitle", "LinkTitle", "LinkTitleNoMenu", "LinkDiscussionTitleNoMenu", "LinkFilename", "LinkFilenameNoMenu",
      "LinkIssueIDNoMenu", "MasterSeriesItemID", "MessageBody", "MessageId",
      "MetaInfo", "Modified_x0020_By", "MoreLink", "Notes", "Occurred", "ol_Department",
      "ol_EventAddress", "owshiddenversion", "ParentFolderId", "ParentLeafName", "ParentVersionString",
      "PendingModTime", "PercentComplete", "PermMask", "PersonViewMinimal", "Picture", "PostCategory",
      "Priority", "ProgId", "PublishedDate", "PublishingStartDate","PublishingExpirationDate", "QuotedTextWasExpanded", "RecurrenceData", "RecurrenceID",
      "RelatedIssues", "RelevantMessages", "RepairDocument", "ReplyNoGif", "RulesUrl", "ScopeId", "SelectedFlag",
      "SelectFilename", "ShortestThreadIndex", "ShortestThreadIndexId", "ShortestThreadIndexIdLookup",
      "ShowCombineView", "ShowRepairView", "StartDate", "StatusBar", "SystemTask", "TaskCompanies",
      "TaskDueDate", "TaskGroup", "TaskStatus", "TaskType", "TemplateUrl", "ThreadIndex", "Threading",
      "ThreadingControls", "ThreadTopic", "Thumbnail", "TimeZone", "ToggleQuotedText", "TotalWork",
      "TrimmedBody", "UniqueId", "VirusStatus", "WebPage", "WorkAddress", "WorkflowAssociation",
      "WorkflowInstance", "WorkflowInstanceID", "WorkflowItemId", "WorkflowListId", "WorkflowVersion",
      "xd_ProgID", "xd_Signature", "XMLTZone", "XomlUrl"
    };
        public SPWeb selectedWeb = null;
        public SPList selectedList = null;
        public SPField selectedField = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            EnsureChildControls();
            selectedWeb = SPContext.Current.Web;
        }

        public void InitializeWithField(SPField field)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                selectedWeb = SPContext.Current.Web;
                EnsureChildControls();
                if (!Page.IsPostBack)
                {

                    LookupTypeDDL.Items.Add(new ListItem("Асинхронный", "0"));
                    foreach (SPList list in selectedWeb.Lists)
                    {
                        if (list.Hidden != true)
                            ListOfListsComboBox.Items.Add(new ListItem(list.Title, list.ID.ToString()));
                    }
                    if (field != null)
                    {
                        try
                        {
                            selectedList = selectedWeb.Lists[new Guid(field.GetFieldAttribute("ListOfLists"))];
                            ListOfListsComboBox.SelectedValue = selectedList.ID.ToString();
                        }
                        catch
                        {
                            ListOfListsComboBox.SelectedValue = null;
                        }
                    }
                    else
                        selectedList = selectedWeb.Lists[new Guid(ListOfListsComboBox.SelectedItem.Value)];

                    foreach (SPField listField in selectedList.Fields)
                    {
                        if (CanFieldBeDisplayed(listField))
                        {
                            drpdFieldList.Items.Add(new ListItem(listField.Title + " " + listField.AuthoringInfo, listField.InternalName));
                            ListOfFieldsComboBox.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", listField.Title), listField.InternalName.ToString()));
                            //ListOfFieldsTitleComboBox.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", listField.Title), listField.InternalName.ToString()));
                        }
                    }
                    if (field != null)
                    {
                        try
                        {
                            ListOfFieldsComboBox.SelectedValue = field.GetFieldAttribute("ListOfFields");
                            selectedField = selectedList.Fields.GetFieldByInternalName(ListOfFieldsComboBox.SelectedItem.Value);
                        }
                        catch
                        {
                            ListOfFieldsComboBox.SelectedValue = null;
                        }
                    }
                    else
                    {
                        selectedField = selectedList.Fields.GetFieldByInternalName(ListOfFieldsComboBox.SelectedItem.Value);
                    }

                }

                if (field == null || Page.IsPostBack) return;
                //filterTextBox.Text = field.GetFieldAttribute("Filter");
                //maxHeightTextBox.Text = field.GetFieldAttribute("MaxHeight");
                //titleFieldTextBox.Text = field.GetFieldAttribute("TitleField");
                //valueFieldTextBox.Text = field.GetFieldAttribute("ValueField");
                //postBackCheckBox.Checked = field.GetFieldAttribute("AutoPostBack") == "true";
                //dynamicFilterTextBox.Text = field.GetFieldAttribute("DynamicFilter");
                //dynamicFilterSourceFieldTextBox.Text = field.GetFieldAttribute("DynamicFilterSourceField");
                //ReadOnlyValueTextBox.Checked = field.GetFieldAttribute("ReadOnlyLookUp") == "true";
                MultipleValuesChck.Checked = field.GetFieldAttribute("MultipleValues") == "true";
                orderByTextBox.Text = field.GetFieldAttribute("OrderBy");
                staticFilterTextBox.Text = field.GetFieldAttribute("staticFilter");
                CascadeParentList.Text = field.GetFieldAttribute("CascadeParent");
                maxRowsTextBox.Text = field.GetFieldAttribute("MaxRows");
                minLengthTextBox.Text = field.GetFieldAttribute("MinLength");
                descriptionFieldsTextBox.Text = field.GetFieldAttribute("DescriptionFields");
                OrderByASC.Checked = field.GetFieldAttribute("OrderByASC") == "true";
                int lkpind = 0;
                int.TryParse(field.GetFieldAttribute("LookupType"), out lkpind);
                LookupTypeDDL.SelectedIndex = lkpind;

                string arrayOtherLookUpGuid = null;

                if (!String.IsNullOrEmpty(field.GetFieldAttribute("OtherLookUp")))
                {
                    string[] arrayOtherLookUp = field.GetFieldAttribute("OtherLookUp").Split(',');
                    foreach (string lookUpFieldName in arrayOtherLookUp)
                    {
                        string fieldName = lookUpFieldName.Trim();
                        if (selectedList.Fields.TryGetFieldByStaticName(fieldName) != null)
                        {
                            arrayOtherLookUpGuid += selectedList.Fields.TryGetFieldByStaticName(fieldName).Title + ",";
                        }
                    }
                }

                var lookupField = field as SPFieldLookup;
                if (lookupField == null) return;

                #region - Select Lists -
                if (lookupField.LookupList != "")
                {
                    ListOfListsComboBox.Visible = false;
                    ListOfListsLabel.Visible = true;
                    ListOfListsLabel.Text = selectedList.Title;
                }
                #endregion

                PrepareListsDDL(field);
            });
        }

        public void OnSaveChange(SPField field, bool isNewField)
        {
            EnsureChildControls();
            field.SetFieldAttribute("MultipleValues", MultipleValuesChck.Checked.ToString().ToLower());
            field.SetFieldAttribute("ListOfLists", ListOfListsComboBox.SelectedItem.Value);
            field.SetFieldAttribute("ValueField", ListOfFieldsComboBox.SelectedItem.Value);
            field.SetFieldAttribute("LookupType", LookupTypeDDL.SelectedItem.Value);

            field.SetFieldAttribute("MaxRows", maxRowsTextBox.Text);
            field.SetFieldAttribute("MinLength", minLengthTextBox.Text);
            field.SetFieldAttribute("DescriptionFields", descriptionFieldsTextBox.Text);

            field.SetFieldAttribute("CascadeParent", CascadeParentList.Text);
            field.SetFieldAttribute("DynamicFilter", CascadeParentList.Text);
            field.SetFieldAttribute("DynamicFilterSourceField", CascadeParentList.Text);

            field.SetFieldAttribute("staticFilter", staticFilterTextBox.Text);
            field.SetFieldAttribute("Filter", staticFilterTextBox.Text);

            field.SetFieldAttribute("ListOfFields", ListOfFieldsComboBox.SelectedItem.Value);
            field.SetFieldAttribute("TitleField", ListOfFieldsComboBox.SelectedItem.Value);

            field.SetFieldAttribute("OrderBy", orderByTextBox.Text);
            field.SetFieldAttribute("OrderByASC", OrderByASC.Checked.ToString().ToLower());

            field.SetFieldAttribute("ListOfSites", selectedWeb.ID.ToString());

            //x_lookupFieldEditor.OnSaveChange(field, isNewField);
            //field.SetFieldAttribute("MaxHeight", maxHeightTextBox.Text);
            //field.SetFieldAttribute("AutoPostBack", postBackCheckBox.Checked.ToString().ToLower());
            //field.SetFieldAttribute("ReadOnlyLookUp", ReadOnlyValueTextBox.Checked.ToString().ToLower());

            var lookupField = field as SPFieldLookup;

            #region - Store Web, List -
            if (lookupField.LookupList == null)
            {
                lookupField.LookupWebId = selectedWeb.ID;
                lookupField.LookupList = "{" + ListOfListsComboBox.SelectedItem.Value + "}";
                lookupField.UnlimitedLengthInDocumentLibrary = false;
                lookupField.LookupField = ListOfFieldsComboBox.SelectedItem.Value;
            }
            else
                lookupField.LookupField = ListOfFieldsComboBox.SelectedItem.Value;
            #endregion

            #region DependentFields
            string DependentFields = "";
            SPList lList = selectedWeb.Lists[new Guid(lookupField.LookupList)];
            foreach (ListItem itm in drpdFieldList.Items)
            {
                if (itm.Selected)
                    DependentFields += lList.Fields.TryGetFieldByStaticName(itm.Value).Title + ";";
            }
            field.SetFieldAttribute("DependentLookUp", DependentFields.Trim(';'));
            #endregion
        }

        public bool DisplayAsNewSection
        {
            get
            {
                EnsureChildControls();
                return true;
            }
        }

        public void FillDrpdFieldList()
        {
            EnsureChildControls();
            SPWeb lookupWeb = selectedWeb;
            SPList lookupList = selectedList;
            drpdFieldList.Items.Clear();

            foreach (SPField field in lookupList.Fields)
            {
                if (CanFieldBeDisplayed(field))
                {
                    drpdFieldList.Items.Add(new ListItem(field.Title + " " + field.AuthoringInfo, field.InternalName));
                }
            }
        }

        protected void PrepareListsDDL(SPField field)
        {
            string DependentLookUp = field.GetFieldAttribute("DependentLookUp");
            if (!String.IsNullOrEmpty(DependentLookUp))
            {
                string[] arrayDependentLookUp = DependentLookUp.Trim(';').Split(';');
                foreach (ListItem itm in drpdFieldList.Items)
                {
                    foreach (string s in arrayDependentLookUp)
                    {
                        if (itm.Value == selectedList.Fields[s].InternalName)
                            itm.Selected = true;
                    }
                }
            }
        }

        protected void ListOfListsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedList = selectedWeb.Lists[new Guid(ListOfListsComboBox.SelectedItem.Value)];
            ListOfFieldsComboBox.Items.Clear();
            foreach (SPField listField in selectedList.Fields)
            {
                if (CanFieldBeDisplayed(listField))
                {
                    ListOfFieldsComboBox.Items.Add(new ListItem(string.Format(CultureInfo.InvariantCulture, "{0}", listField.Title), listField.InternalName.ToString()));
                }
            }
            FillDrpdFieldList();
            selectedField = selectedList.Fields.GetFieldByInternalName(ListOfFieldsComboBox.SelectedItem.Value);//selectedList.Fields[ListOfFieldsComboBox.SelectedItem.Text];
        }

        #region CanFieldBeDisplayed method
        protected bool CanFieldBeDisplayed(SPField f)
        {
            bool retval = false;
            if (f != null && !f.Hidden && (Array.IndexOf<string>(
              EXCLUDED_FIELDS, f.InternalName) < 0))
            {
                switch (f.Type)
                {
                    case SPFieldType.Computed:
                        if (((SPFieldComputed)f).EnableLookup) { retval = true; }
                        break;
                    case SPFieldType.Calculated:
                        if (((SPFieldCalculated)f).OutputType == SPFieldType.Text) { retval = true; }
                        break;
                    default:
                        retval = true;
                        break;
                }
            }
            return retval;
        }
        #endregion
    }
}
