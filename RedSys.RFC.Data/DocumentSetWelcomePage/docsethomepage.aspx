<%@ Register TagPrefix="WpNs2" Namespace="AiT.LiveDocs.ExtendedPageViewer" Assembly="AiT.LiveDocs.ExtendedPageViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=afd9084a84961df9"%>
<%@ Register TagPrefix="WpNs1" Namespace="EA_Web_Parts_2010.ParentList" Assembly="EA.WebParts2010, Version=1.0.0.0, Culture=neutral, PublicKeyToken=096ca705a5953659"%>
<%@ Register TagPrefix="WpNs0" Namespace="CustomDispFormWebPart.CustomDispWebPart" Assembly="CustomDispFormWebPart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e4d459427013a989"%>
<%@ Register TagPrefix="wssuc" TagName="LinksTable" src="/_controltemplates/15/LinksTable.ascx" %> <%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/15/InputFormSection.ascx" %> <%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/15/InputFormControl.ascx" %> <%@ Register TagPrefix="wssuc" TagName="LinkSection" src="/_controltemplates/15/LinkSection.ascx" %> <%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/15/ButtonSection.ascx" %> <%@ Register TagPrefix="wssuc" TagName="ActionBar" src="/_controltemplates/15/ActionBar.ascx" %> <%@ Register TagPrefix="wssuc" TagName="ToolBar" src="/_controltemplates/15/ToolBar.ascx" %> <%@ Register TagPrefix="wssuc" TagName="ToolBarButton" src="/_controltemplates/15/ToolBarButton.ascx" %> <%@ Register TagPrefix="wssuc" TagName="Welcome" src="/_controltemplates/15/Welcome.ascx" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page language="C#" MasterPageFile="~masterurl/default.master"  inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" meta:progid="SharePoint.WebPartPage.Document"     %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Import Namespace="Microsoft.SharePoint" %> <%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="OfficeServer" Namespace="Microsoft.Office.Server.WebControls" Assembly="Microsoft.Office.DocumentManagement, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<asp:Content ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	<SharePoint:EncodedLiteral runat="server" text="<%$Resources:dlcdm, DocSetHomepage_Title%>" EncodeMethod='HtmlEncode' __designer:Preview="Набор документов" __designer:Values="&lt;P N=&#39;Text&#39; Bound=&#39;True&#39; T=&#39;Resources:dlcdm, DocSetHomepage_Title&#39; /&gt;&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl00&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
</asp:Content>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
	<span id="idParentFolderName">&#160;</span>
	<span class="ms-pageTitle-separatorSpan">
		<SharePoint:ClusteredDirectionalSeparatorArrow runat="server" __designer:Preview="&lt;span&gt;&lt;/span&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;ctl01&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
	</span>
	<span id="idDocsetName">&#160;</span>
</asp:Content>
<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
	<OfficeServer:DocSetWelcomePageControl runat="server" ID="idDocSet" __designer:Preview="[ DocSetWelcomePageControl &quot;idDocSet&quot; ]" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;idDocSet&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;"/>
	<table width="100%">
		<tr>
			<td width="15%" valign="top">
				<WebPartPages:WebPartZone runat="server" PartChromeType="None" id="WebPartZone_TopLeft" LayoutOrientation="vertical" AllowPersonalization="false" AllowCustomization="true" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;WebPartZone_TopLeft&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Зона&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;WebPartZone_TopLeft&#39; /&gt;&lt;P N=&#39;AllowPersonalization&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;FrameType&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;PartChromeType&#39; E=&#39;2&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate><WpNs0:CustomDispWebPart runat="server" ShowWorkflowStatus="False" DisplayFieldList="Вид документа;Источник;Номер;Дата документа;Контрагент;Договор;Сумма;Валюта;Юр. лицо;Подразделение;Ответственный;Оператор;Оригинал получен;Статус документа;Комментарий;Сотрудник;Штрих-код" EnableArch="False" Description="My WebPart" ImportErrorMessage="Не удается импортировать эту веб-часть." Title="CustomDispWebPart" Width="350px" ID="g_c7e6373e_ee99_4e87_b3a2_103956835afa" style="padding:0px;" __designer:Values="&lt;P N=&#39;ShowWorkflowStatus&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;DisplayFieldList&#39; T=&#39;Вид документа;Источник;Номер;Дата документа;Контрагент;Договор;Сумма;Валюта;Юр. лицо;Подразделение;Ответственный;Оператор;Оригинал получен;Статус документа;Комментарий;Сотрудник;Штрих-код&#39; /&gt;&lt;P N=&#39;EnableArch&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Description&#39; T=&#39;My WebPart&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; ID=&#39;1&#39; T=&#39;CustomDispWebPart&#39; /&gt;&lt;P N=&#39;ImportErrorMessage&#39; T=&#39;Не удается импортировать эту веб-часть.&#39; /&gt;&lt;P N=&#39;IsShared&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;IsStandalone&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;IsStatic&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Title&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;WebBrowsableObject&#39; R=&#39;0&#39; /&gt;&lt;P N=&#39;Width&#39; T=&#39;350px&#39; /&gt;&lt;P N=&#39;HasAttributes&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;ID&#39; T=&#39;g_c7e6373e_ee99_4e87_b3a2_103956835afa&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Preview="&lt;div class=&quot;ms-webpart-chrome ms-webpart-chrome-vertical &quot; style=&quot;width:350px&quot;&gt;
	&lt;div WebPartID=&quot;&quot; HasPers=&quot;false&quot; id=&quot;WebPartWebPartZone_TopLeft_g_c7e6373e_ee99_4e87_b3a2_103956835afa&quot; class=&quot;ms-WPBody noindex &quot; allowDelete=&quot;false&quot; allowExport=&quot;false&quot; style=&quot;width:350px;overflow:auto;&quot; &gt;&lt;div id=&quot;WebPartContent&quot;&gt;
		&lt;div id=&quot;WebPartZone_TopLeft_g_c7e6373e_ee99_4e87_b3a2_103956835afa&quot; style=&quot;width:350px;padding:0px;&quot;&gt;
	&lt;span&gt;Элемент не существует. Возможно, он был удален другим пользователем.&lt;/span&gt;
&lt;/div&gt;
	&lt;/div&gt;&lt;div class=&quot;ms-clear&quot;&gt;&lt;/div&gt;&lt;/div&gt;
&lt;/div&gt;" __MarkupType="vsattributemarkup" __WebPartId="{C7E6373E-EE99-4E87-B3A2-103956835AFA}" WebPart="true" __designer:IsClosed="false"></WpNs0:CustomDispWebPart>


<OfficeServer:DocumentSetContentsWebPart runat="server" __MarkupType="xmlmarkup" WebPart="true" __WebPartId="{FC612CB5-14FF-48BF-8676-8C0A8F447FEF}" __Preview="&lt;div class=&quot;ms-webpart-chrome ms-webpart-chrome-fullWidth &quot;&gt;
	&lt;div WebPartID=&quot;fc612cb5-14ff-48bf-8676-8c0a8f447fef&quot; HasPers=&quot;false&quot; id=&quot;WebPart&quot; width=&quot;100%&quot; class=&quot;ms-WPBorderBorderOnly noindex &quot; allowDelete=&quot;false&quot; style=&quot;&quot; &gt;&lt;div id=&quot;WebPartContent&quot; PreviewAvailable=&quot;false&quot;&gt;&lt;/div&gt;&lt;div class=&quot;ms-clear&quot;&gt;&lt;/div&gt;&lt;/div&gt;
&lt;/div&gt;" >
<WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/WebPart/v2">
  <Title>Файлы</Title>
  <FrameType>BorderOnly</FrameType>
  <Description>Отображает содержимое набора документов.</Description>
  <IsIncluded>true</IsIncluded>
  <ZoneID>WebPartZone_TopLeft</ZoneID>
  <PartOrder>1</PartOrder>
  <FrameState>Normal</FrameState>
  <Height />
  <Width />
  <AllowRemove>true</AllowRemove>
  <AllowZoneChange>true</AllowZoneChange>
  <AllowMinimize>true</AllowMinimize>
  <AllowConnect>true</AllowConnect>
  <AllowEdit>true</AllowEdit>
  <AllowHide>true</AllowHide>
  <IsVisible>true</IsVisible>
  <DetailLink />
  <HelpLink />
  <HelpMode>Modeless</HelpMode>
  <Dir>Default</Dir>
  <PartImageSmall />
  <MissingAssembly>Невозможно импортировать эту веб-часть.</MissingAssembly>
  <PartImageLarge>/_layouts/15/images/msimagel.gif</PartImageLarge>
  <IsIncludedFilter />
  <ExportControlledProperties>true</ExportControlledProperties>
  <ConnectionID>00000000-0000-0000-0000-000000000000</ConnectionID>
  <ID>idDocSetContentsWebPart</ID>
  <DisplayText>
  </DisplayText>
</WebPart>
</OfficeServer:DocumentSetContentsWebPart>
</ZoneTemplate></WebPartPages:WebPartZone>
			</td>
			<td width="85%" valign="top">
				<WebPartPages:WebPartZone runat="server" PartChromeType="None" id="WebPartZone_Top" LayoutOrientation="vertical" AllowPersonalization="false" AllowCustomization="true" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;WebPartZone_Top&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Зона&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;WebPartZone_Top&#39; /&gt;&lt;P N=&#39;AllowPersonalization&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;FrameType&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;PartChromeType&#39; E=&#39;2&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate><WpNs1:ParentList runat="server" ListName="Рабочие документы" ViewName="Связанные документы" UseViewQuery="False" ParentValue="Родительский документ" StaticFilter="IsDocumentSet=1" SortDESC="False" HowToWork="Синхронно" UrlName="Связать с родительским" IconUrl="/_layouts/15/images/CALADD.gif" LinkScript="javascript:openDialog2(GetBaseOptions(&#39;Добавить связи&#39;,&#39;{SiteURL}/SitePages/RefDoc.aspx?ItemID={ItemID}&amp;ListID={ListID}&#39;,800, 800));return false;" ChromeType="TitleAndBorder" Description="Родительские документы" ImportErrorMessage="Не удается импортировать эту веб-часть." Title="Связанные документы" ID="g_b76765b5_0808_4c65_b678_28771ca8394f" __designer:Values="&lt;P N=&#39;ListName&#39; T=&#39;Рабочие документы&#39; /&gt;&lt;P N=&#39;ViewName&#39; ID=&#39;1&#39; T=&#39;Связанные документы&#39; /&gt;&lt;P N=&#39;ParentValue&#39; T=&#39;Родительский документ&#39; /&gt;&lt;P N=&#39;StaticFilter&#39; T=&#39;IsDocumentSet=1&#39; /&gt;&lt;P N=&#39;HowToWork&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;UrlName&#39; T=&#39;Связать с родительским&#39; /&gt;&lt;P N=&#39;IconUrl&#39; T=&#39;/_layouts/15/images/CALADD.gif&#39; /&gt;&lt;P N=&#39;LinkScript&#39; T=&#39;javascript:openDialog2(GetBaseOptions(&amp;apos;Добавить связи&amp;apos;,&amp;apos;{SiteURL}/SitePages/RefDoc.aspx?ItemID={ItemID}&amp;amp;ListID={ListID}&amp;apos;,800, 800));return false;&#39; /&gt;&lt;P N=&#39;ChromeType&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;Description&#39; T=&#39;Родительские документы&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;ImportErrorMessage&#39; T=&#39;Не удается импортировать эту веб-часть.&#39; /&gt;&lt;P N=&#39;IsShared&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;IsStandalone&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;IsStatic&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;Title&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;WebBrowsableObject&#39; R=&#39;0&#39; /&gt;&lt;P N=&#39;ID&#39; T=&#39;g_b76765b5_0808_4c65_b678_28771ca8394f&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Preview="&lt;div class=&quot;ms-webpart-chrome ms-webpart-chrome-vertical ms-webpart-chrome-fullWidth &quot;&gt;
	&lt;div class=&quot;ms-webpart-chrome-title&quot; id=&quot;WebPartWebPartZone_Top_g_b76765b5_0808_4c65_b678_28771ca8394f_ChromeTitle&quot;&gt;
		&lt;span title=&quot;Связанные документы - Родительские документы&quot; id=&quot;WebPartTitleWebPartZone_Top_g_b76765b5_0808_4c65_b678_28771ca8394f&quot; class=&quot;js-webpart-titleCell&quot;&gt;&lt;div class=&quot;ms-webpart-titleText&quot;&gt;&lt;nobr&gt;&lt;span&gt;Связанные документы&lt;/span&gt;&lt;span id=&quot;WebPartCaptionWebPartZone_Top_g_b76765b5_0808_4c65_b678_28771ca8394f&quot;&gt;&lt;/span&gt;&lt;/nobr&gt;&lt;/div&gt;&lt;/span&gt;
	&lt;/div&gt;&lt;div WebPartID=&quot;&quot; HasPers=&quot;false&quot; id=&quot;WebPartWebPartZone_Top_g_b76765b5_0808_4c65_b678_28771ca8394f&quot; width=&quot;100%&quot; class=&quot;ms-WPBody ms-WPBorder noindex ms-wpContentDivSpace &quot; allowDelete=&quot;false&quot; allowExport=&quot;false&quot; style=&quot;&quot; &gt;&lt;div id=&quot;WebPartContent&quot;&gt;
		&lt;table cellpadding=&quot;4&quot; cellspacing=&quot;0&quot; style=&quot;font: messagebox; color: buttontext; background-color: buttonface; border: solid 1px; border-top-color: buttonhighlight; border-left-color: buttonhighlight; border-bottom-color: buttonshadow; border-right-color: buttonshadow&quot;&gt;
                &lt;tr&gt;&lt;td nowrap&gt;&lt;span style=&quot;font-weight: bold; color: red&quot;&gt;Error Rendering Control&lt;/span&gt; - g_b76765b5_0808_4c65_b678_28771ca8394f&lt;/td&gt;&lt;/tr&gt;
                &lt;tr&gt;&lt;td&gt;An unhandled exception has occurred.&lt;br /&gt;Response is not available in this context.&lt;/td&gt;&lt;/tr&gt;
              &lt;/table&gt;
	&lt;/div&gt;&lt;div class=&quot;ms-clear&quot;&gt;&lt;/div&gt;&lt;/div&gt;
&lt;/div&gt;" __MarkupType="vsattributemarkup" __WebPartId="{B76765B5-0808-4C65-B678-28771CA8394F}" WebPart="true" __designer:IsClosed="false"></WpNs1:ParentList>

<WpNs2:ExtendedPageViewer runat="server" UseCurrentList="False" SearchLookup="False" SearchDocInSet="True" ListTitle="Рабочие документы" HowToWork="Асинхронно" Title="ExtendedPageViewer" FrameType="Default" SuppressWebPartChrome="False" Description="LiveDocs ExtendedPageViewer" IsIncluded="True" ZoneID="WebPartZone_Top" PartOrder="1" FrameState="Normal" AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" DetailLink="" HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Не удается импортировать эту веб-часть." ImportErrorMessage="Не удается импортировать эту веб-часть." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="True" ConnectionID="00000000-0000-0000-0000-000000000000" ID="g_552278bc_932f_45fe_9e0e_dbfa2ca04d22" ExportMode="All" __designer:Values="&lt;P N=&#39;SearchDocInSet&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;ListTitle&#39; T=&#39;Рабочие документы&#39; /&gt;&lt;P N=&#39;HowToWork&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;Title&#39; ID=&#39;1&#39; T=&#39;ExtendedPageViewer&#39; /&gt;&lt;P N=&#39;Description&#39; T=&#39;LiveDocs ExtendedPageViewer&#39; /&gt;&lt;P N=&#39;ZoneID&#39; T=&#39;WebPartZone_Top&#39; /&gt;&lt;P N=&#39;PartOrder&#39; T=&#39;1&#39; /&gt;&lt;P N=&#39;MissingAssembly&#39; ID=&#39;2&#39; T=&#39;Не удается импортировать эту веб-часть.&#39; /&gt;&lt;P N=&#39;ImportErrorMessage&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;ID&#39; T=&#39;g_552278bc_932f_45fe_9e0e_dbfa2ca04d22&#39; /&gt;&lt;P N=&#39;StorageKey&#39; T=&#39;552278bc-932f-45fe-9e0e-dbfa2ca04d22&#39; /&gt;&lt;P N=&#39;Qualifier&#39; T=&#39;WPQ1&#39; /&gt;&lt;P N=&#39;ClientName&#39; T=&#39;varPartWPQ1&#39; /&gt;&lt;P N=&#39;Permissions&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;EffectiveTitle&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;EffectiveStorage&#39; E=&#39;2&#39; /&gt;&lt;P N=&#39;EffectiveFrameType&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;DisplayTitle&#39; R=&#39;1&#39; /&gt;&lt;P N=&#39;ExportMode&#39; E=&#39;1&#39; /&gt;&lt;P N=&#39;IsShared&#39; T=&#39;True&#39; /&gt;&lt;P N=&#39;IsStandalone&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;IsStatic&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;WebBrowsableObject&#39; R=&#39;0&#39; /&gt;&lt;P N=&#39;ZoneIndex&#39; T=&#39;1&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;3&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;3&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Preview="&lt;div class=&quot;ms-webpart-chrome ms-webpart-chrome-vertical ms-webpart-chrome-fullWidth &quot;&gt;
	&lt;div WebPartID=&quot;552278bc-932f-45fe-9e0e-dbfa2ca04d22&quot; HasPers=&quot;false&quot; id=&quot;WebPartWPQ1&quot; width=&quot;100%&quot; class=&quot;ms-WPBody noindex &quot; allowDelete=&quot;false&quot; style=&quot;&quot; &gt;&lt;div id=&quot;WebPartContent&quot;&gt;
		&lt;div id=&quot;WebPartZone_Top_g_552278bc_932f_45fe_9e0e_dbfa2ca04d22&quot;&gt;
	&lt;div id=&quot;WebPartZone_Top_g_552278bc_932f_45fe_9e0e_dbfa2ca04d22_g_552278bc_932f_45fe_9e0e_dbfa2ca04d22_panel&quot;&gt;
		&lt;div id=&quot;WebPartZone_Top_g_552278bc_932f_45fe_9e0e_dbfa2ca04d22_ctl02&quot; style=&quot;display:none;&quot;&gt;
			&lt;div style=&quot;text-align:center;&quot;&gt;
				&lt;img Src=&quot;/_layouts/15/images/progressbar.gif&quot; align=&quot;middle&quot; /&gt;
			&lt;/div&gt;
		&lt;/div&gt;&lt;span id=&quot;WebPartZone_Top_g_552278bc_932f_45fe_9e0e_dbfa2ca04d22_ctl05&quot; style=&quot;visibility:hidden;display:none;&quot;&gt;&lt;/span&gt;
	&lt;/div&gt;
&lt;/div&gt;
	&lt;/div&gt;&lt;div class=&quot;ms-clear&quot;&gt;&lt;/div&gt;&lt;/div&gt;
&lt;/div&gt;" __MarkupType="vsattributemarkup" __WebPartId="{552278BC-932F-45FE-9E0E-DBFA2CA04D22}" WebPart="true" Height="" Width=""></WpNs2:ExtendedPageViewer>

</ZoneTemplate></WebPartPages:WebPartZone>
			</td>
		</tr>
	</table>
	<table width="100%">
		<tr>
			<td>
				<WebPartPages:WebPartZone runat="server" PartChromeType="None" id="WebPartZone_CenterMain" LayoutOrientation="vertical" AllowPersonalization="false" AllowCustomization="true" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;WebPartZone_CenterMain&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Зона&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;WebPartZone_CenterMain&#39; /&gt;&lt;P N=&#39;AllowPersonalization&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;FrameType&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;PartChromeType&#39; E=&#39;2&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
			</td>
		</tr>
	</table>
	<table width="100%">
		<tr>
			<td>
				<WebPartPages:WebPartZone runat="server" PartChromeType="None" id="WebPartZone_Bottom" LayoutOrientation="vertical" AllowPersonalization="false" AllowCustomization="true" __designer:Preview="&lt;Regions&gt;&lt;Region Name=&quot;0&quot; Editable=&quot;True&quot; Content=&quot;&quot; NamingContainer=&quot;True&quot; /&gt;&lt;/Regions&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;0&quot; border=&quot;0&quot; id=&quot;WebPartZone_Bottom&quot;&gt;
	&lt;tr&gt;
		&lt;td style=&quot;white-space:nowrap;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
			&lt;tr&gt;
				&lt;td style=&quot;white-space:nowrap;&quot;&gt;Зона&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;&lt;tr&gt;
		&lt;td style=&quot;height:100%;&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;border-color:Gray;border-width:1px;border-style:Solid;width:100%;height:100%;&quot;&gt;
			&lt;tr valign=&quot;top&quot;&gt;
				&lt;td _designerRegion=&quot;0&quot;&gt;&lt;table cellspacing=&quot;0&quot; cellpadding=&quot;2&quot; border=&quot;0&quot; style=&quot;width:100%;&quot;&gt;
					&lt;tr&gt;
						&lt;td style=&quot;height:100%;&quot;&gt;&lt;/td&gt;
					&lt;/tr&gt;
				&lt;/table&gt;&lt;/td&gt;
			&lt;/tr&gt;
		&lt;/table&gt;&lt;/td&gt;
	&lt;/tr&gt;
&lt;/table&gt;" __designer:Values="&lt;P N=&#39;ID&#39; ID=&#39;1&#39; T=&#39;WebPartZone_Bottom&#39; /&gt;&lt;P N=&#39;AllowPersonalization&#39; T=&#39;False&#39; /&gt;&lt;P N=&#39;FrameType&#39; E=&#39;0&#39; /&gt;&lt;P N=&#39;PartChromeType&#39; E=&#39;2&#39; /&gt;&lt;P N=&#39;Page&#39; ID=&#39;2&#39; /&gt;&lt;P N=&#39;TemplateControl&#39; R=&#39;2&#39; /&gt;&lt;P N=&#39;AppRelativeTemplateSourceDirectory&#39; R=&#39;-1&#39; /&gt;" __designer:Templates="&lt;Group Name=&quot;ZoneTemplate&quot;&gt;&lt;Template Name=&quot;ZoneTemplate&quot; Content=&quot;&quot; /&gt;&lt;/Group&gt;"><ZoneTemplate></ZoneTemplate></WebPartPages:WebPartZone>
			</td>
		</tr>
	</table>
</asp:Content>
