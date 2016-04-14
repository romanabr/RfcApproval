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
				.AddContentTypeFieldLink(RFCFields.InteraptionFlag);
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
						.AddContentTypeFieldLink(RFCFields.KeType)
						.AddContentTypeFieldLink(RFCFields.KeManager)
						.AddContentTypeFieldLink(RFCFields.RFCKeApproveDate)
						.AddContentTypeFieldLink(RFCFields.RFCKeApprove)
						.AddContentTypeFieldLink(RFCFields.RFCKeComment);
				});
				
			});

			var listItemDefinitions = GetListItemDefinitions();
			var typeDefinitions = GetTypeDefinitions(listItemDefinitions);
			var managerItemDefinitions = GetManagerItemDefinition();
			var groupItemDefinitions = GetGroupItemDefinition();
			var responsibleItemDefinitions = GetResponsibleItemDefinitions();
			var catalogueItemDefinitions = GetCatalogueItemDefinitions();
			var effectItemDefinitions = GetEffectItemDefinitions();

			var model = SPMeta2Model.NewWebModel(web =>
			{
				web.AddList(RFCLists.RfcCategoryList, list =>
				 {
					 list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					 list.AddContentTypeLink(RFCContentType.RfcCategory);
					 
					 list.AddListItems(listItemDefinitions);
					 list.AddListView(RFCViews.RfcCategoryListView);
				 });


				web.AddList(RFCLists.KETypeList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcType);
					list.AddListItems(typeDefinitions);
					list.AddListView(RFCViews.RfcTypeListView);
				});



				web.AddList(RFCLists.RFCManagerList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcManager);
					list.AddListItems(managerItemDefinitions);
					list.AddListView(RFCViews.RFCManagerListView);
				});

				web.AddList(RFCLists.RfcGroupKe, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEGroup);
					list.AddListItems(groupItemDefinitions);
				});

				web.AddList(RFCLists.KEResponsibleList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEResponsible);
					list.AddListItems(responsibleItemDefinitions);
				});

				web.AddList(RFCLists.KECatalogueList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KECatalogue);
					list.AddListItems(catalogueItemDefinitions);
					list.AddListView(RFCViews.RfcKeCatalogueListView);
				});

				web.AddList(RFCLists.KeEffectList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEEffect);
					list.AddListItems(effectItemDefinitions);
					list.AddListView(RFCViews.RFCKEEffectListView);
				});

				web.AddList(RFCLists.RfcKeList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcKe);
					list.AddListView(RFCViews.RfcKeListView);
				});

				web.AddList(RFCLists.RfcUserList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.RfcUser);
					list.AddListView(RFCViews.RfcUserListView);
				});

				web.AddList(RFCLists.KeApproveTaskList, list =>
				{
					list.AddRemoveContentTypeLinks(new RemoveContentTypeLinksDefinition { ContentTypes = new List<ContentTypeLinkValue> { new ContentTypeLinkValue { ContentTypeName = BuiltInContentTypeNames.Item } } });
					list.AddContentTypeLink(RFCContentType.KEApproveTask);
					list.AddListView(RFCViews.RfcKeApproveTaskView);

				});

				web.AddList(RFCLists.RFCListDefinition, list =>
				{
					list.AddContentTypeLink(RFCContentType.RfcDocSet);
					list.AddListFieldLink(BuiltInFieldDefinitions.Title.Inherit(
						f => { f.Title = "Номер изменения"; }
					));

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

			SPContentType ctreport = currentWeb.ContentTypes[RFCContentType.RfcDocSet.Name];
			DocumentSetTemplate ds = DocumentSetTemplate.GetDocumentSetTemplate(ctreport);

			SPList docSetList = currentWeb.GetListExt(RFCLists.RFCListDefinition.CustomUrl);
			ds.WelcomePageView  = docSetList.Views["Файлы RFC"];
			ds.SharedFields.Add(currentWeb.Fields.GetFieldByInternalName(RFCFields.Description.InternalName));
			ds.Update(true);

			DeployModel(lookupModel);
		}

		private List<ListItemDefinition> GetEffectItemDefinitions()
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEEffect.Name,
					Title = "1-2", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "1;#База данных управления конфигурациями" },
						 new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "2;#Управление жизненным циклом приложений" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEEffect.Name,
					Title = "2-3", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "2;#Управление жизненным циклом приложений" },
						 new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "3;#Согласование по IT" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEEffect.Name,
					Title = "1-4", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "1;#База данных управления конфигурациями" },
						 new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "4;#Резервация ресурсов" }
					}
				}
			};
		}

		private List<ListItemDefinition> GetCatalogueItemDefinitions()
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KECatalogue.Name,
					Title = "База данных управления конфигурациями", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "CMDB" },
						 new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "1;#База данных" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KECatalogue.Name,
					Title = "Управление жизненным циклом приложений", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "ALMI" },
						 new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KECatalogue.Name,
					Title = "Согласование по IT", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "SPAPPROVALS" },
						 new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KECatalogue.Name,
					Title = "Резервация ресурсов", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "RESOURCERESERVATION" },
						 new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
					}
				}
			};
		}

		private List<ListItemDefinition> GetResponsibleItemDefinitions()
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEResponsible.Name,
					Title = "ALMI", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "2;#Управление жизненным циклом приложений" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEResponsible.Name,
					Title = "CMDB", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "1;#База данных управления конфигурациями" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEResponsible.Name,
					Title = "SPAPPROVALS", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "3;#Согласование по IT" }
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEResponsible.Name,
					Title = "SPAPPROVALS1", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new List<FieldValue>
					{
						 new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "3;#Согласование по IT" }
					}
				}
			};
		}

		private List<ListItemDefinition> GetGroupItemDefinition()
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEGroup.Name,
					Title = "База данных", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.KEGroup.Name,
					Title = "Приложение", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					
				}
			};
		}

		private List<ListItemDefinition> GetManagerItemDefinition()
		{
			SPUser user = currentWeb.EnsureUser("psdev\\pushkinse");
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcManager.Name,
					Title = "Менеджер 1", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Type.InternalName,
							Value = "1;#Обычное"
						},
						new FieldValue
						{
							FieldName = RFCFields.Manager.InternalName,
							Value =  new SPFieldUserValue(currentWeb,user.ID,user.LoginName)
						}
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcManager.Name,
					Title = "Менеджер 2", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Type.InternalName,
							Value = "2;#Аварийное"
						},
						new FieldValue
						{
							FieldName = RFCFields.Manager.InternalName,
							Value =  new SPFieldUserValue(currentWeb,user.ID,user.LoginName)
						}
					}
				}
			};
			
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

		private static List<ListItemDefinition> GetTypeDefinitions(List<ListItemDefinition> listItemDefinitions)
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcType.Name,
					Title = "Обычное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Category.InternalName,
							Value =  new SPFieldLookupValue(1,"Телекоммуникационные услуги")
						}
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcType.Name,
					Title = "Аварийное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Category.InternalName,
							Value =  new SPFieldLookupValue(1,"Телекоммуникационные услуги")
						}
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcType.Name,
					Title = "Обычное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Category.InternalName,
							Value = new SPFieldLookupValue(2,"Внутренней автоматизации")
						}
					}
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcType.Name,
					Title = "Глобальное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Category.InternalName,
							Value =  new SPFieldLookupValue(2,"Внутренней автоматизации")
						}
					}
				},new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcType.Name,
					Title = "Аварийное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
					Values = new   List<FieldValue>
					{
						new FieldValue
						{
							FieldName = RFCFields.Category.InternalName,
							Value =  new SPFieldLookupValue(2,"Внутренней автоматизации")
						}
					}
				}

			};
		}

		private static List<ListItemDefinition> GetListItemDefinitions()
		{
			return new List<ListItemDefinition>
			{
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcCategory.Name,
					Title = "Телекоммуникационные услуги", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
				},
				new ListItemDefinition
				{
					ContentTypeName = RFCContentType.RfcCategory.Name,
					Title = "Внутренняя автоматизация", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
				}
			};
		}
	}
}
