<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KEListViewUserControl.ascx.cs" Inherits="RedSys.KEWP.KEListView.KEListViewUserControl" %>

<SharePoint:ScriptLink runat="server" Name="~sitecollection/style library/jslink/angular.js" OnDemand="false" />
<style>.ketable table, .ketable th , .ketable td {
            border: 1px solid grey;
            border-collapse: collapse;
            padding: 5px;
         }
         
         .ketable table tr:nth-child(odd) {
            background-color: #f2f2f2;
         }
         
         .ketaable table tr:nth-child(even) {
            background-color: #ffffff;
          }	

</style>  
<script>
    var ke =  angular.module('SharePointAngApp', []);
    ke.controller('spkeitemController', function ($scope, $http) {
        $http({
            method: 'GET',
            url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('KE%20по%20RFC')/items?$select=ID,Title,RFCInteraptionFlag,KeKeLink/Title,RFCKeLink/Id&$expand=RFCKeLink,KeKeLink&$filter=((RFCKeLink/Id eq <%=ItemId%>) and (RFCKeType eq 'W'))",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (keitemData, status, headers, config) {
            if (keitemData.d.results.length == 0) {
            }
            else {
                $scope.keitems = keitemData.d.results;
            }
        }).error(function (keitemData, status, headers, config) {

        });
    });

    function DeleteKEItem(el) {
        $SP().list('<%=ListName%>').remove({ ID: parseInt(el.getAttribute('item-id')) });
        window.location.reload();
    }

</script>
 <div ng-app="SharePointAngApp">
     <div ng-controller="spkeitemController" class="ketable">
        <table class="table table-striped table-hover">
            <tr>
                <th>Название КЕ</th>
                 <th>Флаг прерывания</th>
                <th></th>
            </tr>
            <tr ng-repeat="keitem in keitems">
                <td>{{keitem.KeKeLink.Title}}</td>
                 <td><input type="checkbox" disabled="disabled" checked="{{keitem.RFCInteraptionFlag}}" /></td>
                <td><a href="" onclick="javascript:DeleteKEItem(this);" item-id="{{keitem.ID}}"><img id="ctl00_PlaceHolderMain_Toolbar_RptControls_ButtonDeleteItems_ImageOfButton" src="/_layouts/15/images/delitem.gif?rev=23" alt="Delete Selection" style="border-width:0px;height:16px;width:16px;" /></a></td>
             </tr>
        </table>
    </div>
 </div>
<div>
    <table runat="server" ID="buttonTable">
     <tr>
         <td class="ms-addnew" id="addlink" style="padding-bottom: 3px;"><span class="s4-clust" style="width: 10px; height: 10px; overflow: hidden; display: inline-block; position: relative;"><img style="left: 0px !important; top: -32px !important; position: absolute;" alt="" src="/_layouts/15/images/fgimg.png?rev=23"></span>&nbsp;<a class='ms-addnew' onclick="OpenPopUpPageWithTitle('<%=string.Format("{0}?ItemID={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1&KeType=W", NewFormUrl, ItemId, RootWebUrl, ContentTypeId ,SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>',RefreshOnDialogClose,600,400,'Добавить КЕ проведения'); return false;" href='<%=string.Format("{0}?ItemID={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1&KeType=W",NewFormUrl, ItemId, RootWebUrl,ContentTypeId,SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>'>Добавить КЕ проведения</a></td>
    </tr>
</table>

</div>
