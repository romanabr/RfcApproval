<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/15/ButtonSection.ascx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomUpload.aspx.cs" Inherits="PSE.CustomUpload.Layouts.PSE.CustomUpload.CustomUpload" DynamicMasterPageFile="~masterurl/default.master" %>


<asp:Content ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
        <asp:PlaceHolder id="MultipleUploadWarning" Visible="false" runat="server">
             <tr> <td height="8px"><img src="/_layouts/images/blank.gif" width=1 height=8 alt=""></td></tr>
             <tr><td colspan=2>
                <table cellpadding=2 cellspacing=1 width=100% class="ms-informationbar" style="margin-bottom: 5px;" border=0>
                <tr><td width=10 valign=center style="padding: 4px">
                    <img src="/_layouts/images/exclaim.gif" alt="<%$Resources:wss,exclaim_icon%>" runat="server"/></td><td class="ms-descriptiontext"><SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_document_required_multiple%>" EncodeMethod='HtmlEncode'/>
                </td></tr>
                </table>
              </tr></td>
       </asp:PlaceHolder>
</asp:Content>

<asp:Content contentplaceholderid="PlaceHolderPageTitle" runat="server">
    <% if (!string.IsNullOrEmpty(QueryTitle)) { %>
    <%= SPHttpUtility.HtmlEncode(QueryTitle) %>
    <% } else if (IsGeneralUpload) { %>
    <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_pagetitle_15%>" EncodeMethod='HtmlEncode'/>
    <% } else { %>
    <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_pagetitle_15%>" EncodeMethod='HtmlEncode'/>
    <% } %>
</asp:Content>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:HyperLink id="ListTitle" runat="server"/>
    <% if (!string.IsNullOrEmpty(QueryTitle)) { %>
    <%= SPHttpUtility.HtmlEncode(QueryTitle) %>
    <% } else if (IsGeneralUpload) { %>
    <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_pagetitle_15%>" EncodeMethod='HtmlEncode'/>
        <% } else { %>
    : <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_pagetitle_15%>" EncodeMethod='HtmlEncode'/>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:ScriptBlock runat="server">
function _spBodyOnLoad()
{
    var controlId;
    controlId = "<%= InputFile.ClientID %>";
    var c = document.getElementById(controlId);
    if (c != null)
        c.focus();
    var linkName = "<%=OpenWithExplorerLink.ClientID%>";
    var linkElement = document.getElementById(linkName);
    var showLink = (Boolean(linkElement) && (linkElement.style.display != 'none')) && SupportsNavigateHttpFolder();
    if (!showLink)
    {
        if( linkElement != null)
            linkElement.style.display = "none";
    }
}
function LaunchOpenInExplorer()
{
    vCurrentListID = "<%= CurrentList.ID %>";
    vCurrentListUrlAsHTML = "<%= Web.Url + "/" + (CurrentList.RootFolder.Url.Length > 0 ? CurrentList.RootFolder.Url + "/" : "") %>";
    vCurrentWebUrl = "<%= Web.Url %>";
    var destinationFolder;
    var rootFolder = "";
    var destinationUrl;
    if (document.getElementById("destination") != null)
        destinationFolder = document.getElementById("destination").value;
    if (destinationFolder == null || destinationFolder == "")
        rootFolder = GetUrlKeyValue("RootFolder", true);
    if (Boolean(destinationFolder) && destinationFolder != "")
    {
        destinationUrl = destinationFolder;
    }
    else if (rootFolder != "")
    {
        destinationUrl = rootFolder;
    }
    else
    {
        destinationUrl = vCurrentListUrlAsHTML;
    }
    CoreInvoke('NavigateHttpFolder', destinationUrl , '_blank');
    return false;
}
function VerifyCommentLength()
{
<% if (VersionCommentSection.Visible) { %>
    var commentId = "<%= CheckInComment.ClientID %>";
    var verComment = document.getElementById(commentId).value;
    if (escapeProperly(verComment).length > 1023)
    {
        window.alert("<SharePoint:EncodedLiteral runat='server' text='<%$Resources:wss,upload_comment_sizelimitexceeded_err%>' EncodeMethod='EcmaScriptStringLiteralEncode'/>");
        document.getElementById(commentId).focus();
        return false;
    }
<% } %>
    return true;
}
function btnDisabled(set)
{
    if(set)
        document.getElementById("<%= btnOK.ClientID %>").disabled = true;
    else
        document.getElementById("<%= btnOK.ClientID %>").disabled = false;
}
function processInput()
{
    if(!VerifyCommentLength())
        return false;
    if(!document.getElementById("<%= InputFile.ClientID %>"))
        return true;
    if(document.getElementById("<%= InputFile.ClientID %>").value == "")
        return false;
    btnDisabled(true);
}
function ResetSpFormOnSubmitCalled()
{
    _spFormOnSubmitCalled = false;
    return true;
}
</SharePoint:ScriptBlock>
</asp:Content>



<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <input type="hidden" name="destination" id="destination" value="<asp:Literal ID='Destination' runat='server'/>" />
    <table class="propertysheet" border="0" cellspacing="0" cellpadding="0">
    <Control id="SingleItemSection" runat="server">
    <wssuc:InputFormSection runat="server" TopPadding="false">
        <Template_Title>
            <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,upload_document_title_15%>" EncodeMethod='HtmlEncode'/>
        </Template_Title>
        <Template_Description>
                <SharePoint:EncodedLiteral runat="server" text="<%$Resources:wss,DevSite_upload_app_description%>" EncodeMethod='HtmlEncode'/>
        </Template_Description>

        <Template_InputFormControls>
            <wssuc:InputFormControl runat="server" SmallIndent="true">
              <Template_Control>
                   <table class="ms-authoringcontrols" width="100%">
                        <tr><td>
                            <input type="file" id="InputFile" runat="server" class="ms-fileinput ms-fullWidth" size="35" onfocus="ResetSpFormOnSubmitCalled();" />
                        </td></tr>
                        <tr><td>
                        <wssawc:InputFormRequiredFieldValidator ControlToValidate="InputFile"
                            Display="Dynamic" ErrorMessage="<%$Resources:wss,upload_document_file_missing%>" Runat="server"
                            BreakBefore="false" BreakAfter="false" />
                        <asp:CustomValidator ControlToValidate="InputFile"
                            Display = "Dynamic"
                            ErrorMessage = "<%$Resources:wss,upload_document_file_invalid%>"
                            OnServerValidate="ValidateFile"
                            runat="server"/>
                            </td></tr><tr><td>
                            <asp:HyperLink id="OpenWithExplorerLink" runat="server" visible="false"
                                    ACCESSKEY="<%$Resources:wss,openwithexplorer_accesskey%>" Text="<%$SPHtmlEncodedResources:wss,upload_document_with_explorer%>" href='#'
                                    onclick="javascript:return !LaunchOpenInExplorer();"/>
                        </td></tr>
                        <tr><td><asp:CheckBox id="OverwriteSingle" Checked="true" Text="<%$Resources:wss,upload_document_overwrite_file%>" runat="server" CssClass="ms-upload-overwrite-cb" /></td></tr>
                       
                   </table>
                </Template_Control>
            </wssuc:InputFormControl>
        </Template_InputFormControls>
    </wssuc:InputFormSection>
        <tr><td colspan="2">
                            <asp:panel runat="server" ID="ctlPanelFields" Width="100%">
                            </asp:panel>
                       </td>

        </tr>
    <wssuc:InputFormSection runat="server"
        id="VersionCommentSection"
        Title="<%$Resources:wss,upload_version_title%>"
        TopPadding="false">
        <Template_InputFormControls>
            <wssuc:InputFormControl runat="server" SmallIndent="true">
            <Template_Control>
                <asp:TextBox TextMode="MultiLine" Rows="5" Columns="45" id="CheckInComment" class="ms-fullWidth" runat="server" />
            </Template_Control>
            </wssuc:InputFormControl>
        </Template_InputFormControls>
    </wssuc:InputFormSection>
    </Control>
    <wssuc:ButtonSection runat="server" ControlToShowProgress="btnOK">
        <Template_Buttons>
            <input id="btnOK" runat="server" type="button" accesskey="<%$Resources:wss,multipages_okbutton_accesskey%>" class="ms-ButtonHeightWidth" value="<%$Resources:wss,multipages_okbutton_text%>" onclick="processInput();" onserverclick="OnSubmit" />
        </Template_Buttons>
    </wssuc:ButtonSection>

            </table>

</asp:Content>



