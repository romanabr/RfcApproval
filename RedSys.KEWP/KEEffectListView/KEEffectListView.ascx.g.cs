﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RedSys.KEWP.KEEffectListView {
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
    
    
    public partial class KEEffectListView {
        
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebPartCodeGenerator", "14.0.0.0")]
        public static implicit operator global::System.Web.UI.TemplateControl(KEEffectListView target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private global::Microsoft.SharePoint.WebControls.ScriptLink @__BuildControl__control2() {
            global::Microsoft.SharePoint.WebControls.ScriptLink @__ctrl;
            @__ctrl = new global::Microsoft.SharePoint.WebControls.ScriptLink();
            @__ctrl.Name = "~sitecollection/style library/jslink/angular.js";
            @__ctrl.OnDemand = false;
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void @__BuildControlTree(global::RedSys.KEWP.KEEffectListView.KEEffectListView @__ctrl) {
            global::Microsoft.SharePoint.WebControls.ScriptLink @__ctrl1;
            @__ctrl1 = this.@__BuildControl__control2();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        [GeneratedCodeAttribute("Microsoft.VisualStudio.SharePoint.ProjectExtensions.CodeGenerators.SharePointWebP" +
            "artCodeGenerator", "14.0.0.0")]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            parameterContainer.Controls[0].RenderControl(@__w);
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
   var ang1 = angular.module('SharePointAngApp1', []);
    ang1.controller('spkeitemController1', function ($scope, $http) {
        $http({
            method: 'GET',
            url: _spPageContextInfo.webAbsoluteUrl + ""/_api/web/lists/getByTitle('KE%20по%20RFC')/items?$select=Title,RFCInteraptionFlag,KeKeLink/Title,RFCKeLink/Id,KeKeLink/RFCKeMnemonica,RFCKeType&$expand=RFCKeLink,KeKeLink&$filter=((RFCKeLink/Id eq ");
                                                                                                                                                                                                                                                    @__w.Write(ItemId);

            @__w.Write(@") and (RFCKeType eq 'I'))"",
            headers: { ""Accept"": ""application/json;odata=verbose"" }
        }).success(function (keitemData, status, headers, config) {
            $scope.keitems = keitemData.d.results;
        }).error(function (keitemData, status, headers, config) {
       
        });
    });
    angular.element(document).ready(function () {
        angular.bootstrap(document.getElementById('SharePointAngApp1'), ['SharePointAngApp1']);
    });
</script>
    
<div ng-app=""SharePointAngApp1"" id=""SharePointAngApp1"">
     <div ng-controller=""spkeitemController1"" class=""ketable"">
        <table class=""table table-striped table-hover"">
            <tr>
                <th>Название КЕ</th>
                <th>Мнемоника</th>
                 <th>Флаг прерывания</th>
            </tr>
            <tr ng-repeat=""keitem in keitems"">
                <td>{{keitem.KeKeLink.Title}}</td>
                 <td>{{keitem.KeKeLink.RFCKeMnemonica}}</td>
                 <td><input type=""checkbox"" disabled=""disabled"" checked=""{{keitem.RFCInteraptionFlag}}"" /></td>
                </tr>
        </table>
     </div>
</div>");
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
