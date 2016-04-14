
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSys.RFC.Data.Const;

namespace RedSys.RFC.Data.ContentTypes
{
	public static class RFCContentType
	{
		public static ContentTypeDefinition RfcDocSet = new ContentTypeDefinition
		{
			Name = "Запрос на изменение",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A400"),
			ParentContentTypeId = BuiltInContentTypeId.DocumentSet_Correct,
			Group = RFCConst.RFCGroup,
			JSLink = "~sitecollection/_layouts/15/RedSys.RFC.Data/rfccontenttype.js"
		};
		

		public static ContentTypeDefinition RfcCategory = new ContentTypeDefinition
		{

			Name = "RFC Категория",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A403"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition RfcType = new ContentTypeDefinition
		{
			Name = "KE Тип",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A404"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition RfcManager = new ContentTypeDefinition
		{
			Name = "KE Менеджер",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A405"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition KECatalogue = new ContentTypeDefinition
		{
			Name = "Справочный KE",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A406"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition RfcKe = new ContentTypeDefinition
		{
			Name = "Связь KE и RFC",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A407"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition RfcUser = new ContentTypeDefinition
		{
			Name = "Пользователи RFC",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A408"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition KEGroup = new ContentTypeDefinition
		{
			Name = "Группа KE",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A409"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition KEResponsible = new ContentTypeDefinition
		{
			Name = "Ответстенный за KE",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A410"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition KEEffect = new ContentTypeDefinition
		{
			Name = "KE влияния",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A411"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};

		public static ContentTypeDefinition KEApproveTask = new ContentTypeDefinition
		{
			Name = "Задача согласование KE",
			Id = new Guid("AAC93B98-F776-4D5C-9E6E-66F2DC45A412"),
			ParentContentTypeId = BuiltInContentTypeId.Item,
			Group = RFCConst.RFCGroup
		};
	}
}
