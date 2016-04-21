<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KEEffectListView.ascx.cs" Inherits="RedSys.KEWP.KEEffectListView.KEEffectListView" %>
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
          }	</style>

<script>
   var ang1 = angular.module('SharePointAngApp1', []);
    ang1.controller('spkeitemController1', function ($scope, $http) {
        $http({
            method: 'GET',
            url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('KE%20по%20RFC')/items?$select=Title,RFCInteraptionFlag,KeKeLink/Title,RFCKeLink/Id,KeKeLink/RFCKeMnemonica,RFCKeType&$expand=RFCKeLink,KeKeLink&$filter=((RFCKeLink/Id eq <%=ItemId%>) and (RFCKeType eq 'I'))",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (keitemData, status, headers, config) {
            $scope.keitems = keitemData.d.results;
        }).error(function (keitemData, status, headers, config) {
       
        });
    });
    angular.element(document).ready(function () {
        angular.bootstrap(document.getElementById('SharePointAngApp1'), ['SharePointAngApp1']);
    });
</script>
    <%--<link data-require="bootstrap-css@*" data-semver="3.0.0" rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" />--%>
<div ng-app="SharePointAngApp1" id="SharePointAngApp1">
     <div ng-controller="spkeitemController1" class="ketable">
        <table class="table table-striped table-hover">
            <tr>
                <th>Название КЕ</th>
                <th>Мнемоника</th>
                 <th>Флаг прерывания</th>
            </tr>
            <tr ng-repeat="keitem in keitems">
                <td>{{keitem.KeKeLink.Title}}</td>
                 <td>{{keitem.KeKeLink.RFCKeMnemonica}}</td>
                 <td><input type="checkbox" disabled="disabled" checked="{{keitem.RFCInteraptionFlag}}" /></td>
                </tr>
        </table>
     </div>
</div>