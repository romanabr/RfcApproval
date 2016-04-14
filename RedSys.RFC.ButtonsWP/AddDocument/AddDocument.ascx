<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddDocument.ascx.cs" Inherits="ReportButton.AddDocument.AddDocument" %>
 
<Sharepoint:ScriptBlock runat="server">
    function CloseCallback() {
             
                     SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK);
                 }
    
    $(document).ready(function(){
        $("td[class='ms-addnew']:not([id])").remove();
    });
       
</Sharepoint:ScriptBlock>
<table runat="server" ID="buttonTable">
    <tr>
   <td class="ms-addnew" id="adddoc"  style="padding-bottom: 3px;"><span class="s4-clust" style="width: 10px; height: 10px; overflow: hidden; display: inline-block; position: relative;"><img style="left: 0px !important; top: -32px !important; position: absolute;" alt="" src="/_layouts/15/images/fgimg.png?rev=23"></span>&nbsp;<a class="ms-addnew" id="idHomePageNewDocument" onclick='NewItem2(event, "<%=SPUtility.ConcatUrls(SPUtility.ConcatUrls(SPContext.Current.Web.Url,SPUtility.ContextLayoutsFolder), "Upload.aspx") %>?List=<%=SPContext.Current.ListId.ToString("B")%>&amp;RootFolder=<%=SPEncode.UrlEncode(SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, SPContext.Current.ListItem.Folder.Url)) %>"); return false;' href="<%=SPUtility.ConcatUrls(SPUtility.ConcatUrls(SPContext.Current.Web.Url,SPUtility.ContextLayoutsFolder), "Upload.aspx") %>?List=<%=SPContext.Current.ListId.ToString("B")%>&amp;RootFolder=<%=SPEncode.UrlEncode(SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, SPContext.Current.ListItem.Folder.Url)) %>" target="_self" data-viewctr="4">Add document</a></td>
   <td class="ms-addnew" id="addlink" style="padding-bottom: 3px;"><span class="s4-clust" style="width: 10px; height: 10px; overflow: hidden; display: inline-block; position: relative;"><img style="left: 0px !important; top: -32px !important; position: absolute;" alt="" src="/_layouts/15/images/fgimg.png?rev=23"></span>&nbsp;<a class='ms-addnew' onclick="OpenPopUpPageWithTitle('<%=string.Format("{0}?List={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1", SPUtility.ConcatUrls(SPUtility.ConcatUrls(SPContext.Current.Web.Url,SPUtility.ContextLayoutsFolder), "NewLink2.aspx"), SPEncode.UrlEncode( SPContext.Current.ListId.ToString("B").ToUpper()),SPEncode.UrlEncode(SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, SPContext.Current.ListItem.Folder.Url)), SPContext.Current.List.ContentTypes["Link Doc"].Id.ToString(),SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>',CloseCallback,600,400,'Add link doc'); return false;" href='<%=string.Format("{0}?List={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1", SPUtility.ConcatUrls(SPUtility.ConcatUrls(SPContext.Current.Web.Url,SPUtility.ContextLayoutsFolder), "NewLink2.aspx"), SPEncode.UrlEncode( SPContext.Current.ListId.ToString("B").ToUpper()),SPEncode.UrlEncode(SPUtility.ConcatUrls(SPContext.Current.Web.ServerRelativeUrl, SPContext.Current.ListItem.Folder.Url)), SPContext.Current.List.ContentTypes["Link Doc"].Id.ToString(),SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>'>Add link</a></td>
</tr>

</table>
