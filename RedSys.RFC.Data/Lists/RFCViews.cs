using SPMeta2.BuiltInDefinitions;
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Lists
{
	public class RFCViews
	{

		public static ListViewDefinition MyRFC = new ListViewDefinition
		{
			Title = "Мои запросы",
			RowLimit = 30,
			Url = "MyRFC.aspx",
			Fields = new Collection<string>
			{
				BuiltInInternalFieldNames.DocIcon,
				BuiltInInternalFieldNames.LinkFilename,
				BuiltInInternalFieldNames.Editor,
				BuiltInInternalFieldNames._UIVersionString,
				BuiltInInternalFieldNames.ContentType
			},
			Query = "<Where><Or><FieldRef Name='RFCManager'/><Value>[Me]</Value><FieldRef Name='" + BuiltInInternalFieldNames.Author + "'/><Value>[Me]</Value></Or></Where>"
		};

		public static ListViewDefinition OnApprove = new ListViewDefinition
		{
			Title = "На согласовании",
			RowLimit = 30,
			Url = "OnApprove.aspx",
			Fields = new Collection<string>
			{
				BuiltInInternalFieldNames.DocIcon,
				BuiltInInternalFieldNames.LinkFilename,
				BuiltInInternalFieldNames.Editor,
				BuiltInInternalFieldNames._UIVersionString,
				BuiltInInternalFieldNames.ContentType
			},
			Query = "<Where><Or><FieldRef Name='RFCManager'/><Value>[Me]</Value><FieldRef Name='" + BuiltInInternalFieldNames.Author + "'/><Value>[Me]</Value></Or></Where>"
		};

		public static ListViewDefinition RfcUserListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(listView =>
		{
			listView.Fields = new Collection<string>
			{
						BuiltInInternalFieldNames.LinkTitle,
						RFCFields.RfcToKeLink.InternalName,
						RFCFields.RFCUserType.InternalName,
						RFCFields.RFCBusinessRole.InternalName,
						RFCFields.RFCUser.InternalName,
						BuiltInInternalFieldNames.Author,
						BuiltInInternalFieldNames.Created
			};
		});

		public static ListViewDefinition RfcKeListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(listView =>
		{
			listView.Fields = new Collection<string>
			{
						BuiltInInternalFieldNames.LinkTitle,
						RFCFields.RfcToKeLink.InternalName,
						RFCFields.KeToKeLink.InternalName,
						RFCFields.InteraptionFlag.InternalName,
						RFCFields.KeType.InternalName,
						BuiltInInternalFieldNames.Author,
						BuiltInInternalFieldNames.Created
			};
		});

		public static ListViewDefinition RfcKeCatalogueListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
						BuiltInInternalFieldNames.LinkTitle,
						RFCFields.InteraptionFlag.InternalName,
						RFCFields.Type.InternalName,
						RFCFields.KeMnemonica.InternalName,
						BuiltInInternalFieldNames.Author,
						BuiltInInternalFieldNames.Created
			};
			});

		public static ListViewDefinition RFCManagerListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
					BuiltInInternalFieldNames.LinkTitle,
				RFCFields.Type.InternalName,
				RFCFields.Manager.InternalName,
				BuiltInInternalFieldNames.Author,
				BuiltInInternalFieldNames.Created
				};
			});


		public static ListViewDefinition RfcTypeListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
					BuiltInInternalFieldNames.LinkTitle,
				RFCFields.Category.InternalName,
				BuiltInInternalFieldNames.Author,
				BuiltInInternalFieldNames.Created
				};
			});

		public static ListViewDefinition RfcCategoryListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
					BuiltInInternalFieldNames.LinkTitle,
				BuiltInInternalFieldNames.Author,
				BuiltInInternalFieldNames.Created
				};
			});

		public static ListViewDefinition RFCKEEffectListView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
					BuiltInInternalFieldNames.LinkTitle,
					RFCFields.KeParentLink.InternalName,
					RFCFields.KeChildLink.InternalName,
				BuiltInInternalFieldNames.Author,
				BuiltInInternalFieldNames.Created
				};
			});

		public static ListViewDefinition RfcKeApproveTaskView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(
			listView =>
			{
				listView.Fields = new Collection<string> {
					BuiltInInternalFieldNames.LinkTitle,
					RFCFields.RfcToKeLink.InternalName,
					RFCFields.KeToKeLink.InternalName,
						RFCFields.RFCUserType.InternalName,
						RFCFields.KeManager.InternalName,
					RFCFields.RFCKeApprove.InternalName,
					RFCFields.RFCKeApproveDate.InternalName,
					RFCFields.RFCKeComment.InternalName,
				BuiltInInternalFieldNames.Author,
				BuiltInInternalFieldNames.Created
				};
			});

		public static ListViewDefinition RFCFiles = new ListViewDefinition
		{
			Title = "Файлы RFC",
			RowLimit = 30,
			ViewStyleId = BuiltInViewStyleId.BasicTable,
			Url = "rfcfiles.aspx",
			Fields = new Collection<string>
			{
				BuiltInInternalFieldNames.DocIcon,
				BuiltInInternalFieldNames.LinkFilename,
				RFCFields.Description.InternalName,
				BuiltInInternalFieldNames.Editor,
				BuiltInInternalFieldNames._UIVersionString,
				BuiltInInternalFieldNames.ContentType
			}
		};

        public static ListViewDefinition MainView = BuiltInListViewDefinitions.Lists.AllItems.Inherit(listView =>
        {
            listView.ViewStyleId = BuiltInViewStyleId.BasicTable;
            listView.Fields = new Collection<string>
            {
                        BuiltInInternalFieldNames.LinkFilename,
                        RFCFields.Status.InternalName,
                        RFCFields.Category.InternalName,
                        RFCFields.Type.InternalName,
                        RFCFields.Description.InternalName,
                        RFCFields.InteraptionFlag.InternalName,
                        BuiltInInternalFieldNames.Created,
                        BuiltInInternalFieldNames.Author,
                        RFCFields.StartDatePlan.InternalName,
                       RFCFields.StartDateFact.InternalName,
                        RFCFields.EndDatePlan.InternalName,
                         RFCFields.EndDateFact.InternalName
            };
        });
    }

}
