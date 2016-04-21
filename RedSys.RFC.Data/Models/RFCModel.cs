using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Core.Mail;
using RedSys.RFC.Data.Code;
using RedSys.RFC.Data.ContentTypes;
using RedSys.RFC.Data.Fields;
using RedSys.RFC.Data.Lists;
using SPMeta2.BuiltInDefinitions;
using SPMeta2.Definitions;
using SPMeta2.Definitions.ContentTypes;
using SPMeta2.Enumerations;
using SPMeta2.SSOM.Services;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RedSys.RFC.Data.Models
{
	public class RFCModel : SPModel
	{
		public RFCModel(SPWeb web) : base(web)
		{

		}

		public override void Deploy()
		{
			MailTemplate mailTemplate = new MailTemplate(currentWeb);
			mailTemplate.Deploy();

			var lookupModel = SPMeta2Model.NewSiteModel(site =>
			{
				site
					.AddField(RFCFields.Type)
					.AddField(RFCFields.Category)
					.AddField(RFCFields.RfcToKeLink)
					.AddField(RFCFields.KeToKeLink)
					.AddField(RFCFields.KeToTypeLink)
				.AddField(RFCFields.KeParentLink)
				.AddField(RFCFields.KeChildLink);
			});


			var sitemodel = SPMeta2Model.NewSiteModel(site =>
			{
			site.AddFields(RFCFieldsCollection.RFCSiteFields);

			site.AddContentType(RFCContentType.RfcDocSet, ct =>
			{
				ct
				.AddContentTypeFieldLink(RFCFields.Category)
				.AddContentTypeFieldLink(RFCFields.Type)
				.AddContentTypeFieldLink(RFCFields.Description)
				.AddContentTypeFieldLink(RFCFields.FullDescription)
				.AddContentTypeFieldLink(RFCFields.EndDateFact)
				.AddContentTypeFieldLink(RFCFields.EndDatePlan)
				.AddContentTypeFieldLink(RFCFields.InteraptionFlag)
				.AddContentTypeFieldLink(RFCFields.Manager)
				.AddContentTypeFieldLink(RFCFields.StartDateFact)
				.AddContentTypeFieldLink(RFCFields.StartDatePlan)
				.AddContentTypeFieldLink(RFCFields.Status)
				.AddHideContentTypeFieldLinks(new HideContentTypeFieldLinksDefinition
				{
					Fields = new List<FieldLinkValue>
					 {
							  new FieldLinkValue { InternalName = BuiltInInternalFieldNames.Description },
							  new FieldLinkValue { Id = WorkflowFields.WorkflowCurrentUser.FieldId },
							  new FieldLinkValue { Id = WorkflowFields.WorkflowStage.FieldId},
							  new FieldLinkValue { Id = WorkflowFields.WorkflowWFData.FieldId}
					 }
				});

				ct.AddUniqueContentTypeFieldsOrder(new UniqueContentTypeFieldsOrderDefinition
				{
					Fields = new List<FieldLinkValue>
					 {
							 new FieldLinkValue { InternalName = BuiltInInternalFieldNames.Name },
							 new FieldLinkValue { InternalName = RFCFields.Category.InternalName},
							 new FieldLinkValue { InternalName = RFCFields.Type.InternalName},
							 new FieldLinkValue { InternalName = RFCFields.Description.InternalName },
							 new FieldLinkValue { InternalName = RFCFields.FullDescription.InternalName },
							new FieldLinkValue { InternalName = RFCFields.Manager.InternalName},
							new FieldLinkValue { InternalName = RFCFields.InteraptionFlag.InternalName},
							new FieldLinkValue { InternalName = RFCFields.StartDatePlan.InternalName},
							new FieldLinkValue { InternalName = RFCFields.StartDateFact.InternalName},
							new FieldLinkValue { InternalName = RFCFields.EndDatePlan.InternalName},
							new FieldLinkValue { InternalName = RFCFields.EndDateFact.InternalName},
							new FieldLinkValue { InternalName = RFCFields.Status.InternalName}
				}
				});

				ct.AddEventReceivers(RFCEventReceiver.Receiver);
			});

			site.AddContentType(RFCContentType.RfcCategory, ct =>
			{
			});

			site.AddContentType(RFCContentType.RfcType, ct =>
			{
				ct
				.AddContentTypeFieldLink(RFCFields.Category);
			});

			site.AddContentType(RFCContentType.RfcManager, ct =>
			{
				ct
				.AddContentTypeFieldLink(RFCFields.Type)
				.AddContentTypeFieldLink(RFCFields.Manager);
			});

				site.AddContentType(RFCContentType.KEGroup, ct =>
				{

				});

				site.AddContentType(RFCContentType.KEResponsible, ct =>
				{
					ct
					.AddContentTypeFieldLink(RFCFields.KeToKeLink)
					.AddContentTypeFieldLink(RFCFields.KeManager);
				});

			site.AddContentType(RFCContentType.KECatalogue, ct => {
				ct
				.AddContentTypeFieldLink(RFCFields.InteraptionFlag)
				.AddContentTypeFieldLink(RFCFields.KeMnemonica)
				.AddContentTypeFieldLink(RFCFields.KeToTypeLink);
				
			});

				site.AddContentType(RFCContentType.KEEffect, ct =>
				{
					ct
						.AddContentTypeFieldLink(RFCFields.KeParentLink)
						.AddContentTypeFieldLink(RFCFields.KeChildLink);
				});

			site.AddContentType(RFCContentType.RfcKe, ct => {
				ct
				.AddContentTypeFieldLink(RFCFields.RfcToKeLink)
				.AddContentTypeFieldLink(RFCFields.KeToKeLink)
				.AddContentTypeFieldLink(RFCFields.InteraptionFlag)
				.AddContentTypeFieldLink(RFCFields.KeType);

				ct.AddEventReceivers(RFCKeEventReceiver.Receiver);
			});

				site.AddContentType(RFCContentType.RfcUser, ct =>
				{
					ct
						.AddContentTypeFieldLink(RFCFields.RfcToKeLink)
						.AddContentTypeFieldLink(RFCFields.RFCUserType)
						.AddContentTypeFieldLink(RFCFields.RFCBusinessRole)
						.AddContentTypeFieldLink(RFCFields.RFCUser);
				});

				site.AddContentType(RFCContentType.KEApproveTask, ct =>
				{
					ct
						.AddContentTypeFieldLink(RFCFields.RfcToKeLink)
						.AddContentTypeFieldLink(RFCFields.KeToKeLink)
						.AddContentTypeFieldLink(RFCFields.RFCUserType)
						.AddContentTypeFieldLink(RFCFields.KeManager)
						.AddContentTypeFieldLink(RFCFields.RFCKeApproveDate)
						.AddContentTypeFieldLink(RFCFields.RFCKeApprove)
						.AddContentTypeFieldLink(RFCFields.RFCKeComment);
				});
				
			});

			

			var model = SPMeta2Model.NewWebModel(web =>
			{
				web.AddList(RFCLists.RfcCategoryList, list =>
				 {
					 list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					 list.AddContentTypeLink(RFCContentType.RfcCategory);
					 //list.AddListItems(listItemDefinitions);
					 //list.AddListView(RFCViews.RfcCategoryListView);
				 });


				web.AddList(RFCLists.KETypeList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcType);
				});



				web.AddList(RFCLists.RFCManagerList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcManager);
					
				});

				web.AddList(RFCLists.RfcGroupKe, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEGroup);
				});

				web.AddList(RFCLists.KEResponsibleList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEResponsible);
				});

				web.AddList(RFCLists.KECatalogueList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KECatalogue);
				});

				web.AddList(RFCLists.KeEffectList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEEffect);
				});

				web.AddList(RFCLists.RfcKeList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcKe);
				});

				web.AddList(RFCLists.RfcUserList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcUser);
				});

				web.AddList(RFCLists.KeApproveTaskList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEApproveTask);

				});

                web.AddList(RFCLists.RFCListDefinition, list =>
                {
                list.AddContentTypeLink(RFCContentType.RfcDocSet);
                list.AddListFieldLink(BuiltInFieldDefinitions.Title.Inherit(
                    f => { f.Title = "Номер изменения"; }
                ));
                list.AddListView(RFCViews.MainView);
                list.AddListView(RFCViews.MyRFC);
                list.AddListView(RFCViews.OnApprove);
                list.AddListView(RFCViews.RFCFiles);

               
				});
			});

			DeployModel(lookupModel);
			DeployModel(sitemodel);
			DeployModel(model);

			SetPSELookupFields();
			RFCFields.RfcToKeLink.LookupListTitle = RFCLists.RFCListDefinition.Title;
			RFCFields.KeToKeLink.LookupListTitle = RFCLists.KECatalogueList.Title;
			RFCFields.KeToTypeLink.LookupListTitle = RFCLists.RfcGroupKe.Title;
			RFCFields.KeParentLink.LookupListTitle = RFCLists.KECatalogueList.Title;
			RFCFields.KeChildLink.LookupListTitle = RFCLists.KECatalogueList.Title;

            SPList docSetList = currentWeb.GetListExt(RFCLists.RFCListDefinition.CustomUrl);
            SPContentType ctreport = currentWeb.ContentTypes[RFCContentType.RfcDocSet.Name];
			DocumentSetTemplate ds = DocumentSetTemplate.GetDocumentSetTemplate(ctreport);

			
			ds.WelcomePageView  = docSetList.Views["Файлы RFC"];
			ds.Update(true);
			ctreport.Update(true);
			currentWeb.Update();

           

			DeployModel(lookupModel);
		}

		

		private void SetPSELookupFields()
		{

			SPList categoryList = currentWeb.GetListExt(RFCLists.RfcCategoryList.CustomUrl);
			SPList typeList = currentWeb.GetListExt(RFCLists.KETypeList.CustomUrl);
			SPField field = currentWeb.Fields.GetFieldByInternalName(RFCFields.Category.InternalName);

			XDocument xdoc = XDocument.Parse(field.SchemaXml);
			XElement xroot = xdoc.Root;
			xroot.SetAttributeValue("ListOfSites", currentWeb.Site.ID.ToString("D"));
			xroot.SetAttributeValue("WebId", currentWeb.ID.ToString("D"));
			xroot.SetAttributeValue("List", categoryList.ID.ToString("D"));
			xroot.SetAttributeValue("ListOfLists", categoryList.ID.ToString("D"));
			field.SchemaXml = xdoc.ToString();
			field.Update(true);

			SPField field2 = currentWeb.Fields.GetFieldByInternalName(RFCFields.Type.InternalName);
			XDocument xdoc2 = XDocument.Parse(field2.SchemaXml);
			XElement xroot2 = xdoc2.Root;
			xroot2.SetAttributeValue("ListOfSites", currentWeb.Site.ID.ToString("D"));
			xroot2.SetAttributeValue("WebId", currentWeb.ID.ToString("D"));
			xroot2.SetAttributeValue("List", typeList.ID.ToString("D"));
			xroot2.SetAttributeValue("ListOfLists", typeList.ID.ToString("D"));
			field2.SchemaXml = xdoc2.ToString();
			field2.Update(true);
		}

	}
}
