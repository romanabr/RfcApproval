<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/15/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/15/InputFormSection.ascx" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PSELookupProperties.ascx.cs" Inherits="PSELookup.CONTROLTEMPLATES.PSELookup" %>

<wssuc:InputFormSection runat="server" id="InputFormSection1" Title="Основные настройки">
    <template_inputformcontrols>
        <wssuc:InputFormControl runat="server"
        LabelText="<%$Resources:wss,fldedit_getinfofrom%>">
            <Template_Control>
            <asp:Label id="ListOfListsLabel" runat="server" Visible="False"/>
            <asp:DropDownList runat="server" Style="width:auto" ID="ListOfListsComboBox" AutoPostBack="true" OnSelectedIndexChanged="ListOfListsComboBox_SelectedIndexChanged"  />       
            </Template_Control>
        </wssuc:InputFormControl>
        <wssuc:InputFormControl runat="server"
        LabelText="<%$Resources:wss,fldedit_inthiscolumn%>">
            <Template_Control>
            <asp:DropDownList runat="server" Style="width:auto" ID="ListOfFieldsComboBox"  /><br />
            <asp:CheckBox ID="MultipleValuesChck" runat="server" CssClass="ms-input" Text="<%$Resources:wss,fldedit_allowmultivalue%>" TextAlign="Right" />
            </Template_Control>
        </wssuc:InputFormControl>
        <wssuc:InputFormControl runat="server" LabelText="<%$Resources:wss,fldedit_dependentcolumns%>" >
        <Template_Control>
            <asp:CheckBoxList ID="drpdFieldList" runat="server" SelectionMode="Multiple" class="ms-authoringcontrols" />
        </Template_Control>
        </wssuc:InputFormControl>
        <wssuc:InputFormControl runat="server" LabelText="Статический фильтр (and:DisplayName1=value1,DisplayName2=value2,...)" >
        <Template_Control>
            <SharePoint:InputFormTextBox ID="staticFilterTextBox" Size="100" runat="server" TextMode="SingleLine" CssClass="ms-input" />
        </Template_Control>
        </wssuc:InputFormControl>
        <wssuc:InputFormControl runat="server" LabelText="Поля для сортировки" >
        <Template_Control>
            <SharePoint:InputFormTextBox ID="orderByTextBox" Size="100" runat="server" TextMode="SingleLine" CssClass="ms-input" />
            <asp:CheckBox ID="OrderByASC" runat="server" CssClass="ms-input" Text="Сортировать по убыванию" TextAlign="Right" />  
        </Template_Control>
        </wssuc:InputFormControl>
        <wssuc:InputFormControl runat="server" LabelText="Каскадность - родительское поле" >
        <Template_Control>
            <SharePoint:InputFormTextBox ID="CascadeParentList" Size="100" runat="server" TextMode="SingleLine" CssClass="ms-input" />
        </Template_Control>
        </wssuc:InputFormControl>
    </template_inputformcontrols>
</wssuc:InputFormSection>

<wssuc:InputFormSection runat="server" id="InputFormSection2" Title="Расширенные настройки">
  <template_inputformcontrols>
        <wssuc:InputFormControl runat="server"
            LabelText="Внешний вид поля:">
            <Template_Control>
                <asp:DropDownList runat="server" Style="width:auto" ID="LookupTypeDDL"  /><br />
            </Template_Control>
        </wssuc:InputFormControl>
      <wssuc:InputFormControl runat="server"
            LabelText="Поля для поиска">
            <Template_Control>
                <SharePoint:InputFormTextBox ID="descriptionFieldsTextBox"   Size="35" runat="server"  CssClass="ms-input" />
            </Template_Control>
        </wssuc:InputFormControl>
      <wssuc:InputFormControl runat="server"
            LabelText="Минимальная длина для автозаполнения (2, по умолчанию): ">
            <Template_Control>
                <SharePoint:InputFormTextBox ID="minLengthTextBox"  Size="35" runat="server"  CssClass="ms-input" />
            </Template_Control>
        </wssuc:InputFormControl>
      <wssuc:InputFormControl runat="server"
            LabelText="Максимальное число значений (10, по умолчанию):">
            <Template_Control>
                <SharePoint:InputFormTextBox ID="maxRowsTextBox"  Size="35" runat="server"  CssClass="ms-input" />
            </Template_Control>
        </wssuc:InputFormControl>

  </template_inputformcontrols>
</wssuc:InputFormSection>