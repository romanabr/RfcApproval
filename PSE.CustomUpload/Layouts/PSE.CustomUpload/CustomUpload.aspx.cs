using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using CamlexNET;
using RedSys.RFC.Core.Helper;

namespace PSE.CustomUpload.Layouts.PSE.CustomUpload
{
    public partial class CustomUpload : LayoutsPageBase
    {
        List<FormField> Fields;
        string FieldsStr;
        string CheckInRequired = string.Empty;
        bool VersionFieldVisble = true;


        protected void Page_Load(object sender, EventArgs e)
        {
            Fields = new List<FormField>();
            var s = SPContext.Current.Site.Url;
            var web = SPContext.Current.Web;
            var lstVar = web.Lists["Variables"];
            SPQuery search = new SPQuery();
            search.ViewAttributes = "Scope=\"RecursiveAll\"";
            SPListItemCollection resultItems = null;
            search.Query = Camlex.Query().Where(x => (string)x["Title"] == "PSE.CustomUpload - Поля").ToString();
            resultItems = lstVar.GetItems(search);
            if (resultItems.Count == 1)
                FieldsStr = Convert.ToString(resultItems[0]["PSEValue"]);

            SPQuery checkInQuery = new SPQuery();
            checkInQuery.ViewAttributes = "Scope=\"RecursiveAll\"";
            checkInQuery.Query = Camlex.Query().Where(x => (string)x["Title"] == "PSE.CustomUpload - CheckIn").ToString();
            SPListItemCollection checkInRequiredCollection = lstVar.GetItems(checkInQuery);
            if (checkInRequiredCollection != null && checkInRequiredCollection.Count >= 1)
            {
                CheckInRequired = checkInRequiredCollection[0]["PSEValue"].ToString();
            }

            ctlPanelFields.Controls.Add(new LiteralControl("<hr/>"));
            ctlPanelFields.Controls.Add(new LiteralControl("<table width='100%'>"));
            SPList currentList = SPContext.Current.List;
            foreach (string fieldName in FieldsStr.Split(';'))
            {
                SPField field = currentList.Fields.GetField(fieldName);
                if (field == null) continue;
                FormField ff = new FormField();
                ff.ListId = SPContext.Current.ListId;
                ff.ControlMode = SPControlMode.New;
                ff.FieldName = fieldName;
                ff.InputFieldLabel = fieldName;
                ff.ID = field.InternalName;

                ctlPanelFields.Controls.Add(new LiteralControl("<tr><td width='182px'>" + fieldName + "</td><td width='367px'>"));
                ctlPanelFields.Controls.Add(ff);
                ctlPanelFields.Controls.Add(new LiteralControl("</td></tr>"));
                Fields.Add(ff);
            }
            ctlPanelFields.Controls.Add(new LiteralControl("</table>"));
            ctlPanelFields.Controls.Add(new LiteralControl("<hr/>"));

            SPQuery versionQuery = new SPQuery();
            versionQuery.ViewAttributes = "Scope=\"RecursiveAll\"";
            versionQuery.Query = Camlex.Query().Where(x => (string)x["Title"] == "PSE.CustomUpload - Version").ToString();
            SPListItemCollection versionCollection = lstVar.GetItems(versionQuery);
            if (versionCollection != null && versionCollection.Count >= 1)
            {
                VersionFieldVisble = Convert.ToBoolean(versionCollection[0]["PSEValue"]);
                VersionCommentSection.Visible = VersionFieldVisble;
            }

            /*UserObj = new FormField();
            UserObj.ListId = SPContext.Current.ListId;
            UserObj.ControlMode = SPControlMode.New;
            UserObj.FieldName = "Категория";
            UserObj.InputFieldLabel = "Категория";
            ctlPanelFields.Controls.Add(new LiteralControl("Категория: &nbsp"));
            ctlPanelFields.Controls.Add(UserObj);
            //ctlPanelFields.Controls.Add(new LiteralControl("<br/>"));

            UserObj2 = new FormField();
            UserObj2.ListId = SPContext.Current.ListId;
            UserObj2.ControlMode = SPControlMode.New;
            UserObj2.FieldName = "Отображать первым";
            UserObj2.InputFieldLabel = "Отображать первым";
            ctlPanelFields.Controls.Add(new LiteralControl("Отображать первым: &nbsp"));
            ctlPanelFields.Controls.Add(UserObj2);
            //UserObj2 = new CheckBox();
            //FstDisp.Text = "Отображать первым";
            //ctlPanelFields.Controls.Add(FstDisp);*/
        }

        protected void OnSubmit(object sender, EventArgs e)
        {
            if (SPUtility.IsCompatibilityLevel15Up && !this.IsAJAX && (base.Request.QueryString["MultipleUpload"] == "1" || this.MultipleUploadMode))
            {
                throw new SPException(SPResource.GetString("MultipleUploadNotSupported", new object[0]));
            }

            if (!this.IsAJAX)
            {
                if (this.MultipleUploadMode)
                {
                    if (base.IsDialogMode)
                    {
                        this.Context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup();</script>");
                        this.Context.Response.Flush();
                        this.Context.Response.End();
                        return;
                    }
                    SPUtility.Redirect(this.RedirectUrl, SPRedirectFlags.Static | SPRedirectFlags.UseSource, this.Context);
                    return;
                }
                else
                {
                    this.Page.Validate();
                    //UserObj.Validate();
                    if (!this.Page.IsValid)// || !UserObj.IsValid)
                    {
                        return;
                    }
                    foreach (FormField ff in Fields)
                    {
                        ff.Validate();
                        if (!ff.IsValid)
                        {
                            return;
                        }
                        SPField field = ff.List.Fields.GetField(ff.FieldName);

                        if ((field != null && field.Required) && (ff.Value == null || ((field.Type == SPFieldType.Text || field.Type == SPFieldType.Note) && string.IsNullOrEmpty(ff.Value.ToString()))))
                        {
                            ff.ErrorMessage = "Field is required";
                            return;
                        }
                    }
                    using (SPLongOperation sPLongOperation = new SPLongOperation(this))
                    {
                        sPLongOperation.Begin();
                        SPFile sPFile = null;
                        this.isFileSuccessfullyUploaded = true;
                        try
                        {
                            sPFile = this.UploadFile(null);
                        }
                        catch (Exception ex)
                        {
                            this.isFileSuccessfullyUploaded = false;
                            if (ex is PathTooLongException)
                            {
                                SPException ex2 = new SPException(ex.Message);
                                throw ex2;
                            }
                            throw;
                        }
                        finally
                        {
                        }

                        try
                        {


                            if (CheckInRequired == "1" && isFileSuccessfullyUploaded == true && sPFile != null)
                            {
                                sPFile.CheckIn(CheckInComment.Text, SPCheckinType.MajorCheckIn);
                            }
                        }
                        catch
                        {

                        }

                        if (sPFile != null)
                        {
                            try
                            {
                                if ((this.IsSimpleList && sPFile.Level != SPFileLevel.Checkout) || sPFile.Item == null)
                                {
                                    if (base.IsDialogMode)
                                    {
                                        string serverRelativeUrl = sPFile.ServerRelativeUrl;
                                        string scriptLiteralToEncode = (serverRelativeUrl.IndexOf(".", StringComparison.Ordinal) > 0) ? SPUtility.MapToIcon(base.Web, serverRelativeUrl, string.Empty, IconSize.Size16) : "icgen.gif";
                                        string strScript = string.Format(CultureInfo.InvariantCulture, "retVal = {{}};\r\n                                            retVal['newFileUrl'] = \"{0}\";\r\n                                            retVal['isFolder'] = \"false\";\r\n                                            retVal['newFileSize'] = {1};\r\n                                            retVal['newFileIcon'] = \"{2}\";\r\n                                            window.frameElement.commitPopup(retVal);", new object[]
                                                {
                                                    SPHttpUtility.EcmaScriptStringLiteralEncode(serverRelativeUrl),
                                                    sPFile.Length,
                                                    SPHttpUtility.EcmaScriptStringLiteralEncode(scriptLiteralToEncode)
                                                });
                                        sPLongOperation.EndScript("window.frameElement.commitPopup();");
                                    }
                                    else
                                    {
                                        sPLongOperation.EndScript("window.frameElement.commitPopup();");
                                        sPLongOperation.End(this.RedirectUrl, SPRedirectFlags.Static | SPRedirectFlags.UseSource, this.Context, null);
                                    }
                                }
                                else
                                {
                                    sPLongOperation.EndScript("window.frameElement.commitPopup();");
                                    sPLongOperation.End(this.GetEditFormUrl(sPFile), SPRedirectFlags.Default, this.Context, null, base.IsDialogMode ? "window.frameElement.overrideDialogResult(1 /*ok*/);" : null);
                                }
                            }
                            catch (FileNotFoundException ex)
                            {
                                sPLongOperation.EndScript("window.frameElement.commitPopup();");
                                sPLongOperation.End(this.GetEditFormUrl(sPFile), SPRedirectFlags.Default, this.Context, null, base.IsDialogMode ? "window.frameElement.overrideDialogResult(1 /*ok*/);" : null);
                            }
                        }

                        if (base.IsDialogMode)
                        {
                            this.Context.Response.Write("<script type='text/javascript'>window.frameElement.commitPopup();</script>");
                            this.Context.Response.Flush();
                            this.Context.Response.End();
                            return;
                        }
                        else
                            return;
                    }
                }
            }
        }


        bool isFileSuccessfullyUploaded;

        SPContentTypeId m_ctId = SPContentTypeId.Empty;

        protected bool IsSimpleList
        {
            get
            {
                if (this.CurrentList == null)
                {
                    return false;
                }
                foreach (SPField sPField in this.CurrentList.Fields)
                {
                    if (sPField.InternalName != "Title" && sPField.CanBeDisplayedInEditForm && !sPField.DefaultListField && CultureInfo.InvariantCulture.CompareInfo.Compare(sPField.GetProperty("ShowInEditForm"), "FALSE", CompareOptions.IgnoreCase) != 0)
                    {
                        return false;
                    }
                }
                bool flag = false;
                PropertyInfo property = this.CurrentList.GetType().GetProperty("ForceDefaultContentType");
                if (property != null)
                {
                    MethodInfo getMethod = property.GetGetMethod();
                    if (getMethod != null)
                    {
                        flag = (bool)getMethod.Invoke(this.CurrentList, null);
                    }
                }
                return flag || !this.CurrentList.ContentTypesEnabled || this.CurrentList.ContentTypes.Count <= 1;
            }
        }

        string GetVersionedString15(string name, params object[] values)
        {
            if (base.Web.Site.CompatibilityLevel >= 15)
            {
                name += "_15";
            }
            return SPResource.GetString(name, values);
        }


        SPFile UploadFile(HttpPostedFile requestFile)
        {
            string text = "";
            string keyOrValueToEncode;
            SPVirusCheckStatus sPVirusCheckStatus;
            string text2;
            SPFile sPFile = this.UploadFile(requestFile, out keyOrValueToEncode, out sPVirusCheckStatus, out text2);
            if (text2 != null)
            {
                string[] array = text2.Split(new char[]
        {
            '\n'
        }, StringSplitOptions.RemoveEmptyEntries);
                switch (array.Length)
                {
                    case 1:
                        text = this.GetVersionedString15("FileScanError", new object[0]);
                        text2 = array[0];
                        break;
                    case 2:
                        text = array[0];
                        text2 = text2.Substring(array[0].Length);
                        break;
                    default:
                        text = this.GetVersionedString15("FileScanError", new object[0]);
                        break;
                }
                this.isFileSuccessfullyUploaded = false;
            }

            #region Virus check
            //if (this.IsAJAX && sPVirusCheckStatus != SPVirusCheckStatus.Clean)
            //{
            //    string strMessage = (!string.IsNullOrWhiteSpace(text2)) ? text2 : ((!string.IsNullOrWhiteSpace(text)) ? text : this.GetVersionedString15("FileScanError", new object[0]));
            //    throw new SPException(strMessage);
            //}
            //SPVirusReportStatus sPVirusReportStatus = SPVirusReportStatus.Clean;
            //if (sPVirusCheckStatus != SPVirusCheckStatus.Clean)
            //{
            //    if (sPVirusCheckStatus == SPVirusCheckStatus.Cleaned)
            //    {
            //        sPVirusReportStatus = SPVirusReportStatus.UploadCleaned;
            //    }
            //    else
            //    {
            //        if (sPVirusCheckStatus == SPVirusCheckStatus.Timeout)
            //        {
            //            sPVirusReportStatus = SPVirusReportStatus.Timeout;
            //        }
            //        else
            //        {
            //            sPVirusReportStatus = SPVirusReportStatus.UploadInfected;
            //        }
            //    }
            //    this.isFileSuccessfullyUploaded = false;
            //}
            //string str = sPVirusReportStatus.ToString("D");
            //if (sPVirusCheckStatus != SPVirusCheckStatus.Clean)
            //{
            //    string text3 = this.CurrentList.ParentWeb.Url + "/_layouts/AvReport.aspx?Status=" + str;
            //    string text4 = text3;
            //    text3 = string.Concat(new string[]
            //                {
            //                    text4,
            //                    "&Document=",
            //                    SPHttpUtility.UrlKeyValueEncode(keyOrValueToEncode),
            //                    "&Info=",
            //                    SPHttpUtility.UrlKeyValueEncode(text2)
            //                });
            //    text3 = text3 + "&Title=" + SPHttpUtility.UrlKeyValueEncode(text);
            //    string text5;
            //    if (sPFile != null && sPVirusCheckStatus == SPVirusCheckStatus.Cleaned)
            //    {
            //        text5 = this.GetEditFormUrl(sPFile);
            //    }
            //    else
            //    {
            //        text5 = SPUtility.GetRedirectUrl(base.Request, this.CurrentList);
            //    }
            //    text5 = SPHttpUtility.UrlPathEncode(text5, true);
            //    text3 = text3 + "&useNext=" + SPHttpUtility.UrlKeyValueEncode(text5);
            //    this.avReportURL = text3;
            //    SPUtility.Redirect(text3, SPRedirectFlags.Static, this.Context);
            //} 
            #endregion
            return sPFile;
        }


        bool CheckExists(SPFile sPFile)
        {
            bool result = false;
            try
            {
                string url = sPFile.Url;
                Guid uniqueId = sPFile.UniqueId;
                result = (url != null && uniqueId != Guid.Empty);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        SPFile UploadFile(HttpPostedFile requestFile, out string leafName, out SPVirusCheckStatus checkStatus, out string virusMessage)
        {
            HttpPostedFile httpPostedFile;
            if (requestFile == null)
            {
                httpPostedFile = this.InputFile.PostedFile;
                if (httpPostedFile == null && base.Request.Files.Count > 0)
                {
                    httpPostedFile = base.Request.Files[0];
                }
            }
            else
            {
                httpPostedFile = requestFile;
            }
            leafName = this.GetLeafName(httpPostedFile.FileName);
            string strUrl = this.CurrentFolderServerRelativeUrl + "/" + leafName;
            SPFile sPFile = base.Web.GetFile(strUrl);
            bool flag = false;
            if (sPFile != null)
            {
                flag = CheckExists(sPFile);
            }

            if (!flag)
            {
                SPFileCollectionAddParameters sPFileCollectionAddParameters = new SPFileCollectionAddParameters();
                sPFileCollectionAddParameters.Overwrite = this.OverwriteSingle.Checked;
                sPFileCollectionAddParameters.CheckRequiredFields = true;
                sPFileCollectionAddParameters.AutoCheckoutOnInvalidData = true;
                sPFileCollectionAddParameters.CheckInComment = this.CheckInComment.Text;
                sPFileCollectionAddParameters.ThrowOnVirusFound = false;
                sPFile = this.CurrentFolder.Files.Add(leafName, httpPostedFile.InputStream, sPFileCollectionAddParameters);
                checkStatus = sPFileCollectionAddParameters.OutVirusCheckStatus;
                virusMessage = sPFileCollectionAddParameters.OutVirusCheckMessage;
                SEtExtraFields(sPFile);
                return sPFile;
            }
            if (!this.OverwriteSingle.Checked)
            {
                throw new SPException(SPResource.GetString(CultureInfo.CurrentCulture, "FileAlreadyExistsError", new object[]
                                        {
                                            sPFile.Name,
                                            sPFile.ModifiedBy,
                                            SPUtility.FormatDate(base.Web, sPFile.TimeLastModified, SPDateFormat.DateTime)
                                        }));
            }
            if (sPFile.Item != null)
            {
                this.m_ctId = sPFile.Item.ContentTypeId;
            }
            bool onCheckout = false;
            string text = null;
            string text2 = null;
            if (sPFile.CheckOutType != SPFile.SPCheckOutType.None)
            {
                text = sPFile.CheckedOutByUser.LoginName;
                text2 = sPFile.CheckedOutByUser.Name;
            }
            if (!string.IsNullOrEmpty(text))
            {
                if (text != base.Web.CurrentUser.LoginName)
                {
                    throw new SPException(SPResource.GetString(CultureInfo.CurrentCulture, "FileAlreadyCheckedOutError", new object[]
                        {
                            sPFile.Name,
                            string.IsNullOrWhiteSpace(text2) ? text : text2,
                            SPUtility.FormatDate(base.Web, sPFile.CheckedOutDate, SPDateFormat.DateTime)
                        }));
                }
            }
            else
            {
                if (this.CurrentList.ForceCheckout)
                {
                    sPFile.CheckOut();
                    onCheckout = true;
                }
            }
            try
            {
                SPFileSaveBinaryParameters sPFileSaveBinaryParameters = new SPFileSaveBinaryParameters();
                sPFileSaveBinaryParameters.CheckInComment = this.CheckInComment.Text;
                sPFileSaveBinaryParameters.ThrowOnVirusFound = false;
                sPFile.SaveBinary(httpPostedFile.InputStream, sPFileSaveBinaryParameters);
                checkStatus = sPFileSaveBinaryParameters.OutVirusCheckStatus;
                virusMessage = sPFileSaveBinaryParameters.OutVirusCheckMessage;
                //SQM.DpIncrementOne(SQMDP.DATAID_NUMSINGLEFILEUPLOADS);
            }
            catch (Exception)
            {
                if (onCheckout)
                {
                    sPFile.UndoCheckOut();
                }
                throw;
            }
            SEtExtraFields(sPFile);

            return sPFile;
        }

        private void SEtExtraFields(SPFile sPFile)
        {
            foreach (FormField ff in Fields)
            {
                var item = sPFile.Item;
                if (item.Fields.ContainsField(ff.FieldName))
                {
                    if (item.Fields[ff.FieldName].Type == SPFieldType.Lookup && ff.Value != null)
                    {
                        var v = new SPFieldLookupValue(ff.Value.ToString());
                        item[ff.FieldName] = v;
                    }
                    else
                        item[ff.FieldName] = ff.Value;
                }
            }
            /* var v = new SPFieldLookupValue(UserObj.Value.ToString());
             var item = sPFile.Item;
             if (item.Fields.ContainsField("Категория"))
                 item["Категория"] = v;*/
            //if (item.Fields.ContainsField("Отображать первым"))
            //   item["Отображать первым"] = FstDisp.Checked;
            ListItemHelper.GracefulSPListItemUpdate(sPFile.Item, false);
        }

        string GetLeafName(string s)
        {
            int num = s.LastIndexOf('\\');
            if (num >= 0)
            {
                return s.Substring(num + 1);
            }
            return s;
        }

        protected bool IsAJAX
        {
            get
            {
                return base.Request.QueryString["IsAjax"] == "1";
            }
        }

        protected bool MultipleUploadMode
        {
            get
            {
                return !SPUtility.IsCompatibilityLevel15Up && base.Request.QueryString["MultipleUpload"] == "1";
            }
        }



        string m_folderUrl;
        protected virtual string CurrentFolderServerRelativeUrl
        {
            get
            {
                if (this.m_folderUrl == null)
                {
                    string text = base.Request.QueryString["RootFolder"];
                    if (!string.IsNullOrEmpty(text))
                    {
                        this.m_folderUrl = text;
                    }
                    else
                    {
                        this.m_folderUrl = this.CurrentList.RootFolder.Url;
                    }
                }
                return this.m_folderUrl;
            }
        }

        SPFolder m_folder;
        protected SPFolder CurrentFolder
        {
            get
            {
                if (this.m_folder == null)
                {
                    this.m_folder = base.Web.GetFolder(this.CurrentFolderServerRelativeUrl);
                }
                return this.m_folder;
            }
            set
            {
                this.m_folder = value;
                if (value != null)
                {
                    this.m_folderUrl = this.m_folder.ServerRelativeUrl;
                    return;
                }
                this.m_folderUrl = null;
            }
        }


        bool isGeneralUploadInit;
        bool isGeneralUploadValue;

        protected bool IsGeneralUpload
        {
            get
            {
                if (!this.isGeneralUploadInit)
                {
                    this.isGeneralUploadValue = (SPUtility.IsCompatibilityLevel15Up && base.Request.QueryString["List"] == null);
                    this.isGeneralUploadInit = true;
                }
                return this.isGeneralUploadValue;
            }
        }

        bool TryParseGuid(string convertToGuid, out Guid guid)
        {
            guid = Guid.Empty;
            if (string.IsNullOrEmpty(convertToGuid) || !IsGuid(convertToGuid))
            {
                return false;
            }
            try
            {
                guid = new Guid(convertToGuid);
            }
            catch (ArgumentNullException)
            {
                bool result = false;
                return result;
            }
            catch (FormatException)
            {
                bool result = false;
                return result;
            }
            catch (OverflowException)
            {
                bool result = false;
                return result;
            }
            return true;
        }

        bool IsGuid(string strId)
        {
            if (string.IsNullOrEmpty(strId))
            {
                return false;
            }
            strId = strId.Trim();
            bool result;
            if (strId.Length < 32)
            {
                result = false;
            }
            else
            {
                if (strId.Contains("x") || strId.Contains("X"))
                {
                    strId = strId.Replace(" ", "");
                    result = Regex.IsMatch(strId, "^\\{0[x|X][a-fA-F\\d]{8},(0[x|X][a-fA-F\\d]{4},){2}\\{(0[x|X][a-fA-F\\d]{2},){7}0[x|X][a-fA-F\\d]{2}\\}\\}$", RegexOptions.Compiled);
                }
                else
                {
                    result = Regex.IsMatch(strId, "^([a-fA-F\\d]{8}-([a-fA-F\\d]{4}-){3}[a-fA-F\\d]{12}|\\([a-fA-F\\d]{8}-([a-fA-F\\d]{4}-){3}[a-fA-F\\d]{12}\\)|\\{[a-fA-F\\d]{8}-([a-fA-F\\d]{4}-){3}[a-fA-F\\d]{12}\\}|[a-fA-F\\d]{32})$", RegexOptions.Compiled);
                }
            }
            return result;
        }



        SPDocumentLibrary m_list;// Microsoft.SharePoint.ApplicationPages.UploadPage
        protected virtual SPDocumentLibrary CurrentList
        {
            get
            {
                if (this.m_list == null)
                {
                    if (this.IsGeneralUpload)
                    {
                        //if (this.AvailableDocLibs != null && this.AvailableDocLibs.SelectedItem != null)
                        //{
                        //    string value = this.AvailableDocLibs.SelectedItem.Value;
                        //    if (!string.IsNullOrEmpty(value))
                        //    {
                        //        Guid guid;
                        //        if (!Guid.TryParse(value, out guid) || Guid.Empty == guid)
                        //        {
                        //            throw new SPException(SPResource.GetString("ListGone", new object[0]));
                        //        }
                        //        this.m_list = (base.Web.Lists[guid] as SPDocumentLibrary);
                        //        if (this.m_list == null)
                        //        {
                        //            throw new SPException(SPResource.GetString("NotInDocLib", new object[0]));
                        //        }
                        //        this.CurrentFolder = this.m_list.RootFolder;
                        //    }
                        //}
                    }
                    else
                    {
                        string text = base.Request.QueryString["List"];
                        if (text != null)
                        {
                            Guid uniqueID;
                            if (!TryParseGuid(text, out uniqueID))
                            {
                                throw new SPException(SPResource.GetString("ListGone", new object[0]));
                            }
                            SPDocumentLibrary sPDocumentLibrary = base.Web.Lists[uniqueID] as SPDocumentLibrary;
                            if (sPDocumentLibrary == null)
                            {
                                throw new SPException(SPResource.GetString("NotInDocLib", new object[0]));
                            }
                            this.m_list = sPDocumentLibrary;
                        }
                    }
                }
                return this.m_list;
            }
        }

        protected string RedirectUrl
        {
            get
            {
                return base.Web.Site.MakeFullUrl(this.CurrentList.DefaultViewUrl);
            }
        }

        protected string SourceUrl
        {
            get
            {
                return base.Request.QueryString["Source"];
            }
        }

        protected string QueryTitle
        {
            get
            {
                return base.Request.QueryString["Title"];
            }
        }

        string GetEditFormUrl(SPFile spfile)
        {
            return GetEditFormUrl(base.Web, this.CurrentList, this.CurrentFolder, this.m_ctId, this.CheckInComment.Text, spfile, this.SourceUrl, base.Request);
        }

        string GetEditFormUrl(SPWeb web, SPList currentList, SPFolder currentFolder, SPContentTypeId id, string comments, SPFile spfile, string sourceUrl, HttpRequest request)
        {
            StringBuilder stringBuilder = new StringBuilder();
            bool flag = false;
            stringBuilder.Append(web.Url);
            stringBuilder.Append("/");
            stringBuilder.Append(currentList.Forms[PAGETYPE.PAGE_EDITFORM].Url);
            stringBuilder.Append("?Mode=Upload");
            if (comments != null)
            {
                stringBuilder.Append("&CheckInComment=");
                stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(comments));
            }
            stringBuilder.Append("&ID=");
            stringBuilder.Append(spfile.Item.ID);
            stringBuilder.Append("&RootFolder=");
            stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(currentFolder.ServerRelativeUrl));
            if (id != SPContentTypeId.Empty)
            {
                stringBuilder.Append("&ContentTypeId=");
                stringBuilder.Append(id.ToString());
                flag = true;
            }
            if (sourceUrl != null)
            {
                stringBuilder.Append("&Source=");
                stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(sourceUrl));
            }
            string[] allKeys = request.QueryString.AllKeys;
            for (int i = 0; i < allKeys.Length; i++)
            {
                string text = allKeys[i];
                if (text != "Source" && text != "Mode" && text != "CheckInComment" && text != "RootFolder" && text != "List" && text != "MultipleUpload" && text != "ID" && (!flag || !(text == "ContentTypeId")))
                {
                    string[] values = request.QueryString.GetValues(text);
                    for (int j = 0; j < values.Length; j++)
                    {
                        string keyOrValueToEncode = values[j];
                        stringBuilder.Append("&");
                        stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(text));
                        stringBuilder.Append("=");
                        stringBuilder.Append(SPHttpUtility.UrlKeyValueEncode(keyOrValueToEncode));
                    }
                }
            }
            return stringBuilder.ToString();
        }

        protected void ValidateFile(object source, ServerValidateEventArgs args)
        {
            CustomValidator customValidator = (CustomValidator)source;
            HtmlInputFile htmlInputFile = (HtmlInputFile)customValidator.FindControl(customValidator.ControlToValidate);
            if (!SPUrlUtility.IsLegalFileName(GetLeafName(htmlInputFile.PostedFile.FileName)))
            {
                args.IsValid = false;
                return;
            }
            if (htmlInputFile.PostedFile != null && htmlInputFile.PostedFile.ContentLength > 0)
            {
                args.IsValid = true;
                return;
            }
            args.IsValid = false;
        }


    }
}
