using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Fields
{
	public static class WorkflowFields
	{
		public static ContentTypeFieldLinkDefinition WorkflowWFData = new ContentTypeFieldLinkDefinition
		{
			FieldId = new Guid("54E6DD1C-5AD5-453F-8D15-8256DE77E99F"),
			DisplayName = "WFData",
			FieldInternalName = "WFData",
			Required = false,
		};

		public static ContentTypeFieldLinkDefinition WorkflowStage = new ContentTypeFieldLinkDefinition
		{
			FieldId = new Guid("F0D9617F-67A7-4BD1-AED3-7256DE77E99F"),
			DisplayName = "Статус рабочего процесса",
			FieldInternalName = "Stage"
		};

        public static NoteFieldDefinition WorkflowValue = new NoteFieldDefinition
        {
            Id = new Guid("8b6f91a6-e376-43a6-9dc3-b23f98b4d133"),
            Title = "Variables Value",
            InternalName = "PSEValue",
            Group = "PSE.Common",
            Required = true,
            RichText = false,
            RichTextMode = BuiltInRichTextMode.Compatible
        };


        public static ContentTypeFieldLinkDefinition WorkflowCurrentUser = new ContentTypeFieldLinkDefinition
		{
			FieldId = new Guid("8B6F91A6-E376-43A6-9DC3-B23F98B4D132"),
			DisplayName = "Текущий исполнитель",
			FieldInternalName = "WFCurrentUser"
		};

	}
}
