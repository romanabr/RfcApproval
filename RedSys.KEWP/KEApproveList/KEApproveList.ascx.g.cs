﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RedSys.KEWP.KEApproveList {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    using System.CodeDom.Compiler;
    
    
    public partial class KEApproveList {
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebPartCodeGenerator", "14.0.0.0")]
        public static implicit operator global::System.Web.UI.TemplateControl(KEApproveList target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void @__BuildControlTree(global::RedSys.KEWP.KEApproveList.KEApproveList @__ctrl) {
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write(@"
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
            url: _spPageContextInfo.webAbsoluteUrl + ""/_api/web/lists/getByTitle('Задачи%20согласования%20KE')/items?$select=Title,RFCKeLink/Title,KeKeLink/Title,RFCKeLink/Id,RFCKeType,KeManager/Title,RFCKeApprove,RFCKeApproveDate,RFCKeComment&$expand=RFCKeLink,KeKeLink,KeManager&$filter=RFCKeLink/Id eq ");
                                                                                                                                                                                                                                                                                                         @__w.Write(ItemId);

            @__w.Write(@""",
            headers: { ""Accept"": ""application/json;odata=verbose"" }
        }).success(function (taskitemsData, status, headers, config) {
            $scope.taskitems = taskitemsData.d.results;
        }).error(function (taskitemsData, status, headers, config) {
       
        });
    });
    angular.element(document).ready(function () {
        angular.bootstrap(document.getElementById('approve'), ['approve']);
    });
</script>
    
<div ng-app=""approve"" id=""approve"">
     <div ng-controller=""approve1"" class=""ketable"">
        <table class=""table table-striped table-hover"">
            <tr>
                <th>Связанный КЕ</th>
                <th>Ответственный</th>
                <th>Статус согласования</th>
                <th>Дата согласования</th>
                <th>Комментарий</th>
            </tr>
            <tr ng-repeat=""taskitem in taskitems"">
                <td>{{taskitem.KeKeLink.Title}}</td>
                <td>{{taskitem.KeManager.Title}}</td>
                <td>{{taskitem.RFCKeApprove}}</td>
                <td>{{taskitem.RFCKeApproveDate}}</td>
                <td>{{taskitem.RFCKeComment}}</td>
                </tr>
        </table>
     </div>
</div>
");
        }
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}