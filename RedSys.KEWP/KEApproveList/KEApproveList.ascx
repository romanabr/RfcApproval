<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="KEApproveList.ascx.cs" Inherits="RedSys.KEWP.KEApproveList.KEApproveList" %>
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
   var approve = angular.module('approve', []);
    approve.controller('approve1', function ($scope, $http) {
        $http({
            method: 'GET',
            url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getByTitle('Задачи%20согласования%20KE')/items?$select=Title,RFCKeLink/Title,KeKeLink/Title,RFCKeLink/Id,RFCUserType,KeManager/Title,RFCKeApprove,RFCKeApproveDate,RFCKeComment&$expand=RFCKeLink,KeKeLink,KeManager&$filter=RFCKeLink/Id eq <%=ItemId%>",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (taskitemsData, status, headers, config) {
            $scope.taskitems = taskitemsData.d.results;
        }).error(function (taskitemsData, status, headers, config) {
       
        });
    });
    angular.element(document).ready(function () {
        angular.bootstrap(document.getElementById('approve'), ['approve']);
    });
</script>
    <%--<link data-require="bootstrap-css@*" data-semver="3.0.0" rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap.min.css" />--%>
<div ng-app="approve" id="approve">
     <div ng-controller="approve1" class="ketable">
        <table class="table table-striped table-hover">
            <tr>
                <th>Связанный КЕ</th>
                <th>Ответственный</th>
                <th>Статус согласования</th>
                <th>Дата согласования</th>
                <th>Комментарий</th>
            </tr>
            <tr ng-repeat="taskitem in taskitems">
                <td>{{taskitem.KeKeLink.Title}}</td>
                <td>{{taskitem.KeManager.Title}}</td>
                <td>{{taskitem.RFCKeApprove}}</td>
                <td>{{taskitem.RFCKeApproveDate}}</td>
                <td>{{taskitem.RFCKeComment}}</td>
                </tr>
        </table>
     </div>
</div>
