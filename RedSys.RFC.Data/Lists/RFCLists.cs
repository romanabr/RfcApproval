using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Lists
{
	public  class RFCLists
	{
        public static ListDefinition VariablesList = new ListDefinition
        {
            Title = "Variables",
            CustomUrl = "Lists/Variables",
            ContentTypesEnabled = true,
            EnableVersioning = true,
            TemplateType = BuiltInListTemplateTypeId.GenericList,
        };




        public static ListDefinition RFCManagerList = new ListDefinition
		{
			Title = "Менеджеры RFC",
			CustomUrl = "Lists/RFCManagerList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments=false,
			EnableFolderCreation = false,
			OnQuickLaunch= false
		};

		public static ListDefinition RfcCategoryList = new ListDefinition
		{
			Title = "Категории RFC",
			CustomUrl = "Lists/RfcCategoryList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false
		};

		public static ListDefinition KETypeList = new ListDefinition
		{
			Title = "Тип RFC",
			CustomUrl = "Lists/RfcTypeList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false
		};

		public static ListDefinition RfcKeList = new ListDefinition
		{
			Title = "KE по RFC",
			CustomUrl = "Lists/RfcKeList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false
		};

		public static ListDefinition KECatalogueList = new ListDefinition
		{
			Title = "Справочник KE",
			CustomUrl = "Lists/RfcKeCatalogueList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false
		};

		public static ListDefinition RFCListDefinition = new ListDefinition
		{
			Title = "Управление изменениями",
			Description = "",
			TemplateType = BuiltInListTemplateTypeId.DocumentLibrary,
			ContentTypesEnabled = true,
			CustomUrl = "RFCCenter"
		};

		public static ListDefinition RfcUserList = new ListDefinition
		{
			Title = "Участники RFC",
			Description = "",
			ContentTypesEnabled= true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false,
			CustomUrl = "Lists/RfcUserList"

		};

		public static ListDefinition RfcGroupKe = new ListDefinition
		{
			Title = "Группы KE",
			Description = "",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false,
			CustomUrl = "Lists/RfcKeKEList"

		};

		public static ListDefinition KEResponsibleList = new ListDefinition
		{
			Title = "Ответстенные за KE",
			Description = "",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false,
			CustomUrl = "Lists/RfcKEResponsibleList"
		};

		public static ListDefinition KeEffectList = new ListDefinition
		{
			Title = "Влияние KE",
			Description = "",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false,
			CustomUrl = "Lists/RfcKEEffectList"
		};

		public static ListDefinition KeApproveTaskList = new  ListDefinition
		{
			Title = "Задачи согласования KE",
			Description = "",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
			EnableVersioning = true,
			EnableAttachments = false,
			EnableFolderCreation = false,
			OnQuickLaunch = false,
			CustomUrl = "Lists/RfcKEApproveTaskList"
		};
}
}
