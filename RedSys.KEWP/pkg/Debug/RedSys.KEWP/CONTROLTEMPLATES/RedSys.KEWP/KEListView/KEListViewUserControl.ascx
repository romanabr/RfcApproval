<%@ Assembly Name="RedSys.KEWP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3363b1b9c91f7cbe" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KEListViewUserControl.ascx.cs" Inherits="RedSys.KEWP.KEListView.KEListViewUserControl" %>

<script src="//ajax.googleapis.com/ajax/libs/angularjs/1.2.15/angular.min.js"></script>  
  
<script>
    var angApp = angular.module('SharePointAngApp', []);
    angApp.controller('spCustomerController', function ($scope, $http) {
        $http({
            method: 'GET',
        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('KE%20по%20RFC')/items?$select=Title,RFCInteraptionFlag,KeManagers/Title,RFCKeApprove,RFCKeApproveDate,RFCKeComment,RFCKeLink/Id&$expand=KeManagers,RFCKeLink&$filter=RFCKeLink/Id eq <%=ItemId%>",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (customerData, status, headers, config) {
            $scope.customers = customerData.d.results;
        }).error(function (customerData, status, headers, config) {
       
        });
    });
    
</script>
    <%--<link data-require="bootstrap-css@*" data-semver="3.0.0" rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" />--%>
     <div ng-app="SharePointAngApp">
     <div ng-controller="spCustomerController">
     <br/>
        <table class="table table-striped table-hover">
            <tr>
                <th>Название</th>
                <th>Флаг прерывания</th>
                <th>Статус согласования</th>
                 <th>Дата согласования</th>
                 <th>Комментарий</th>
               
            </tr>
            <tr ng-repeat="customer in customers">
                <td>{{customer.Title}}</td>
                <td><input type="checkbox" disabled="disabled" checked="{{customer.RFCInteraptionFlag}}" /></td>
                <td>{{customer.RFCKeApprove}}</td>
                <td>{{customer.RFCKeApproveDate}}</td>
                <td>{{customer.RFCKeComment}}</td>
                </tr>
        </table>
         <table runat="server" ID="buttonTable">
    <tr>
   <td class="ms-addnew" id="addlink" style="padding-bottom: 3px;"><span class="s4-clust" style="width: 10px; height: 10px; overflow: hidden; display: inline-block; position: relative;"><img style="left: 0px !important; top: -32px !important; position: absolute;" alt="" src="/_layouts/15/images/fgimg.png?rev=23"></span>&nbsp;<a class='ms-addnew' onclick="OpenPopUpPageWithTitle('<%=string.Format("{0}?ItemID={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1", "http://portal.psdev.com/Lists/RfcKeList/NewForm.aspx", ItemId,"%2FLists%2FRfcKeList","0x0100AAC93B98F7764D5C9E6E66F2DC45A40700FB57D1082C0B3A4E9751E6934258B4E6",SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>',CloseCallback,600,400,'Add link doc'); return false;" href='<%=string.Format("{0}?ItemID={1}&RootFolder={2}&ContentTypeId={3}&Source={4}&IsDlg=1", "http://portal.psdev.com/Lists/RfcKeList/NewForm.aspx", ItemId,"%2FLists%2FRfcKeList","0x0100AAC93B98F7764D5C9E6E66F2DC45A40700FB57D1082C0B3A4E9751E6934258B4E6",SPEncode.UrlEncode ( HttpContext.Current.Request.Url.PathAndQuery))%>'>Add link</a></td>
</tr>

</table>
    </div>
