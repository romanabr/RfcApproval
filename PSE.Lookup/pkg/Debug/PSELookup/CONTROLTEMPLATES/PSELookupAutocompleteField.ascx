<%@ Assembly Name="PSELookup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=74e57c0574f9b73c" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Import Namespace="PSELookup.Field" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PSELookupAutocompleteField.ascx.cs" Inherits="PSELookup.CONTROLTEMPLATES.PSELookupAutocompleteField" %>

<script type="text/javascript">
    if (!window.jQuery) {
        document.write('<script src="/_layouts/15/PSELookup/AutocompleteLookup/jquery/js/jquery-1.11.1.min.js">\x3C/script>');
    } 
 </script>


 <script type="text/javascript" src="/_layouts/15/PSELookup/AutocompleteLookup/rich-controls.js"></script>
<SharePoint:CssRegistration ID="CssRegistration1" runat="server" Name="/_layouts/15/PSELookup/AutocompleteLookup/jquery/js/jquery-ui.min.css"/>
<SharePoint:CssRegistration ID="CssRegistration2" runat="server" Name="/_layouts/15/PSELookup/AutocompleteLookup/rich-controls.css"/>


<table title="<%= Field.Title%>">
    <tr>
        <td style="vertical-align:middle; text-align:center;">
            <div class="con">
                <asp:TextBox ID="lookupTextBox" runat="server" CssClass="lookupTextBox" />
                <img runat="server" id="clearButton" title='<%# GetLocalizedString("richLookupShowAll") %>' src="/_layouts/images/DELETEGRAY.gif" />
            </div>
        </td>
        <td style="vertical-align:middle; text-align:center;">
            <img runat="server" id="lookupButton" title='<%# GetLocalizedString("richLookupShowAll") %>' src="/_layouts/15/PSELookup/AutocompleteLookup/Images/lupa.png" style="margin-left:5px; cursor:pointer;" />
        </td>
    </tr>
</table>



<!-- <%if(!IsViewHidden) {%>
    <a runat="server" id="viewButton" title='<%# GetLocalizedString("richLookupView") %>' class="lookupButton ui-button ui-widget ui-state-default ui-button-icon-only ui-button-icon-primary ui-icon ui-icon-search" href="<%# GetDisplayFormUrl() %>" target="_blank">&nbsp;</a>
<%} %>
<%if(!IsAddHidden) {%>
    <a title="<%= GetLocalizedString("richLookupAddNew") %>" class="lookupButton ui-button ui-widget ui-state-default ui-button-icon-only ui-button-icon-primary ui-icon ui-icon-circle-plus" href="<%= GetNewFormUrl() %>" target="_blank">&nbsp;</a>
<%} %>-->

<div runat="server" id="selection" class="lookupSelection"></div>

<% if(!string.IsNullOrEmpty(MaxHeight)) {%>
<style>
    .ui-autocomplete {
        max-height: <%= MaxHeight %>px;
        overflow-y: auto;
        overflow-x: hidden;
    }
</style>
<%} %>

<script type="text/javascript">
    $(document).ready(function () {
<% var df = PSELookupFieldControl.FindRecursiveControl(this.Page.Form, DynamicFilter); if (df != null) { DynamicFilterClientID = df.ClientID; }%>
        var tb = $('#<%= lookupTextBox.ClientID%>');
        tb.addClass('<%= Field.Title%>');
            tb.richlookup(
            {
                'viewButton':'#<%= viewButton.ClientID%>',
                'lookupButton':'#<%= lookupButton.ClientID%>',
                'clearButton':'#<%= clearButton.ClientID%>',
                'selectionID' :'#<%= selection.ClientID%>',
                'list':'<%=ListGuid %>',
                'valueField':'<%=ValueField %>',
                'titleField':'<%=TitleField %>',
                'descFields':'<%=DescriptionFields %>',
                'maxRows':'<%=MaxRows %>',
                'minLength':'<%=MinLength %>',
                'isMultiple':<%=AllowMultipleValues.ToString().ToLower() %>,
                'filter':'<%=Filter.Replace("\'","\\\'").Replace("\"","\\\'").Replace("\r", "").Replace("\n", "") %>',
                'siteID':'<%=SiteID %>',
                'thisStaticName': '<%=thisStaticName%>',
                'thisName': '<%= Field.Title%>',
                'webID':'<%=WebID %>',
                'autoPostBack': '<%=AutoPostBack %>',
                'dynamicFilter': '<%=DynamicFilter %>',
                'dynamicFilterSourceField': '<%=DynamicFilterSourceField %>',
                'dynamicFilterClientID': '<%=DynamicFilterClientID %>',
                'orderBy':'<%=OrderBy%>',
                'OrderByASC':'<%=OrderByASC%>'
            });

            <%
            if (DynamicFilter != "")
            {
                Response.Write(
                    "if (typeof DynamicFilterList['" + DynamicFilter + "'] == 'undefined'){DynamicFilterList['" +
                    DynamicFilter + "'] = new Array();}DynamicFilterList['" + DynamicFilter + "'][DynamicFilterList['" + DynamicFilter + 
                    "'].length] = new Array('" + lookupTextBox.ClientID + "','" + Field.Title + "');");
            }
            %>
                            
            <% foreach (SPFieldLookupValue value in Values)
               {
                   Response.Write(string.Format("tb.addValueToRichlookup('{0}', '{1}', false);", value.LookupId, value.LookupValue));
               }
               Response.Write("tb.autocomplete('close');");
            %>
        });
</script>
