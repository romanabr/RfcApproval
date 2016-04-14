using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Enumerations;
using RedSys.RFC.Data.Const;
using SPMeta2.Syntax.Default;

namespace RedSys.RFC.Data
{
	public static class RFCFields
	{
		///<summary>Номер изменения</summary>

		///<summary>Статус</summary>

		public static TextFieldDefinition Status = new TextFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C11}"),
			Title = "Статус",
			InternalName = "RFCStatus",
			Group = RFCConst.RFCGroup,
			Required = false,
			ShowInNewForm = false,
			ShowInEditForm = false,
			DefaultValue = RFCStatus.NEW
		};

		///<summary>
		///	Категория изменения(название)
		///</summary>
		public static FieldDefinition Category = new FieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C12}"),
			Title = "Категория изменения",
			InternalName = "RFCCategory",
			Group = RFCConst.RFCGroup,
			Required = true,
			FieldType = "PSELookup",
			AdditionalAttributes = new List<FieldAttributeValue>
				{
							new FieldAttributeValue{ Name="MultipleValues",Value ="false"},
							new FieldAttributeValue{ Name="ValueField", Value="Title"},
							new FieldAttributeValue{ Name="LookupType", Value="0"},
							new FieldAttributeValue{ Name="CascadeParent", Value=""},
							new FieldAttributeValue{ Name="DynamicFilter", Value=""},
							new FieldAttributeValue{ Name="DynamicFilterSourceField", Value=""},
							new FieldAttributeValue{ Name="staticFilter", Value=""},
							new FieldAttributeValue{ Name="Filter", Value=""},
							new FieldAttributeValue{ Name="ListOfFields", Value="Title"},
							new FieldAttributeValue{ Name="TitleField", Value="Title"},
							new FieldAttributeValue{ Name="OrderBy", Value=""},
							new FieldAttributeValue{ Name="OrderByASC", Value="false"},
							new FieldAttributeValue{ Name="ShowField", Value="Title"},
							new FieldAttributeValue{ Name="DependentLookUp", Value=""},
							new FieldAttributeValue{ Name="BaseRenderingType", Value="Lookup"},
							new FieldAttributeValue{ Name="OldDependentLookUp", Value=""},
							new FieldAttributeValue{ Name="Mult",Value="FALSE" }
				}
		};

		///<summary>
		///	Тип изменения(название)
		///	</summary>
		public static FieldDefinition Type = new FieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C13}"),
			Title = "Тип изменения",
			InternalName = "RFCType",
			Group = RFCConst.RFCGroup,
			Required = true,
			AddToDefaultView = true,
			FieldType = "PSELookup",
			AdditionalAttributes = new List<FieldAttributeValue>
				{
							new FieldAttributeValue{Name = "MultipleValues",Value ="false"},
							new FieldAttributeValue{Name="ValueField", Value="Title"},
							new FieldAttributeValue{Name="LookupType", Value="0"},
							new FieldAttributeValue{ Name="CascadeParent", Value="Категория изменения"},
							new FieldAttributeValue{ Name="DynamicFilter", Value="Категория изменения"},
							new FieldAttributeValue{ Name="DynamicFilterSourceField", Value="Категория изменения"},
							new FieldAttributeValue{ Name="staticFilter", Value=""},
							new FieldAttributeValue{ Name="Filter", Value=""},
							new FieldAttributeValue{ Name="ListOfFields", Value="Title"},
							new FieldAttributeValue{ Name="TitleField", Value="Title"},
							new FieldAttributeValue{ Name="OrderBy", Value=""},
							new FieldAttributeValue{ Name="OrderByASC", Value="false"},
							new FieldAttributeValue{ Name="ShowField", Value="Title"},
							new FieldAttributeValue{ Name="DependentLookUp", Value=""},
							new FieldAttributeValue{ Name="BaseRenderingType", Value="Lookup"},
							new FieldAttributeValue{ Name="OldDependentLookUp", Value=""},
							new FieldAttributeValue{ Name="Mult",Value="FALSE" }
				}
		};


		///<summary>Краткое описание</summary>
		public static NoteFieldDefinition Description = new NoteFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C14}"),
			Title = "Краткое описание",
			InternalName = "RFCDescription",
			Group = RFCConst.RFCGroup,
			Required = true,
			RichText = false,
			RichTextMode = BuiltInRichTextMode.Compatible
		};

	

		///<summary>Менеджер</summary>
		public static UserFieldDefinition Manager = new UserFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C16}"),
			Title = "Менеджер",
			InternalName = "RFCManager",
			Group = RFCConst.RFCGroup,
			Required = true,
			SelectionMode = BuiltInFieldUserSelectionMode.PeopleOnly
		};

		///<summary>Флаг прерывания сервиса</summary>

		public static BooleanFieldDefinition InteraptionFlag = new BooleanFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C17}"),
			Title = "Флаг прерывания сервиса",
			InternalName = "RFCInteraptionFlag",
			Group = RFCConst.RFCGroup,
			Required = true,
			DefaultValue = "1"
		};

		///<summary>Дата и время начала работы(план)</summary>

		public static DateTimeFieldDefinition StartDatePlan = new DateTimeFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C18}"),
			Title = "Начала работы(план)",
			InternalName = "RFCStartDatePlan",
			Group = RFCConst.RFCGroup,
			Required = true,
			DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime
		};

		///<summary>Дата и время начала работы(факт)</summary>
		public static DateTimeFieldDefinition StartDateFact = new DateTimeFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C19}"),
			Title = "Начала работы(факт)",
			InternalName = "RFCStartDateFact",
			Group = RFCConst.RFCGroup,
			Required = true,
			DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
			ShowInNewForm = false
		};

		///<summary>Дата и время окончания работы(план)</summary>
		public static DateTimeFieldDefinition EndDatePlan = new DateTimeFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C20}"),
			Title = "Окончания работы(план)",
			InternalName = "RFCEndDatePlan",
			Group = RFCConst.RFCGroup,
			Required = true,
			DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime
		};

		///<summary>Дата и время окончания работы(факт)</summary>
		public static DateTimeFieldDefinition EndDateFact = new DateTimeFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C21}"),
			Title = "Окончания работы(факт)",
			InternalName = "RFCEndDateFact",
			Group = RFCConst.RFCGroup,
			Required = true,
			DisplayFormat = BuiltInDateTimeFieldFormatType.DateTime,
			ShowInNewForm = false
		};

		///<summary>
		///Тип
		///</summary>
		public static TextFieldDefinition KeType = new TextFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C26}"),
			Title = "Тип",
			InternalName = "RFCKeType",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		///<summary>
		///Мнемоника
		///</summary>
		public static TextFieldDefinition KeMnemonica = new TextFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C27}"),
			Title = "Мнемонике",
			InternalName = "RFCKeMnemonica",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		public static LookupFieldDefinition RfcToKeLink = new LookupFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C30}"),
			Title = "Связь с RFC",
			InternalName = "RFCKeLink",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		public static LookupFieldDefinition KeToKeLink = new LookupFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C31}"),
			Title = "Связь с КЕ",
			InternalName = "KeKeLink",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		public static UserFieldDefinition KeManager= new UserFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C32}"),
			Title = "Ответвенный",
			InternalName = "KeManager",
			Group = RFCConst.RFCGroup,
			Required = true,
			SelectionMode = BuiltInFieldUserSelectionMode.PeopleOnly,
			AllowMultipleValues = false
		};


		public static ChoiceFieldDefinition RFCKeApprove = new ChoiceFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C33}"),
			Title = "Статус согласования",
			InternalName = "RFCKeApprove",
			Group = RFCConst.RFCGroup,
			Required = false,
			Choices = new Collection<string>
			{
			   "Согласовано",
			   "Отклонено",
			   "В работе"
			}
		};


		public static DateTimeFieldDefinition RFCKeApproveDate = new DateTimeFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C34}"),
			Title = "Дата согласования",
			InternalName = "RFCKeApproveDate",
			Group = RFCConst.RFCGroup,
			Required = false
		};


		public static NoteFieldDefinition RFCKeComment = new NoteFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C35}"),
			Title = "Комментарий",
			InternalName = "RFCKeComment",
			Group = RFCConst.RFCGroup,
			Required = false
		};

		public static NoteFieldDefinition FullDescription = new NoteFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C36}"),
			Title = "Полное описание",
			InternalName = "RFCFullDescription",
			Group = RFCConst.RFCGroup,
			Required = true,
			RichText = true,
			RichTextMode = BuiltInRichTextMode.FullHtml
		};


		public static ChoiceFieldDefinition RFCUserType = new ChoiceFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C37}"),
			Title = "Тип участника",
			InternalName = "RFCUserType",
			Group = RFCConst.RFCGroup,
			Required = true,
			Choices	= new Collection<string>
			{
				RFCUserTypeConst.INITIATOR, RFCUserTypeConst.APPROVER, RFCUserTypeConst.ASSIGNTO, RFCUserTypeConst.MANAGER
			}
		};


		public static TextFieldDefinition RFCBusinessRole = new TextFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C38}"),
			Title = "Бизнес-роль",
			InternalName = "RFCBusinessRole",
			Group = RFCConst.RFCGroup,
			Required = false
		};

		public static UserFieldDefinition RFCUser = new UserFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C39}"),
			Title = "Участник",
			InternalName = "RFCUser",
			Group = RFCConst.RFCGroup,
			Required = true,
			SelectionMode = BuiltInFieldUserSelectionMode.PeopleOnly
		};


		public static LookupFieldDefinition KeToTypeLink = new LookupFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C40}"),
			Title = "Группа КЕ",
			InternalName = "KeToType",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		public static LookupFieldDefinition KeParentLink = new LookupFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C41}"),
			Title = "Родительский КЕ",
			InternalName = "KeParent",
			Group = RFCConst.RFCGroup,
			Required = true
		};

		public static LookupFieldDefinition KeChildLink = new LookupFieldDefinition
		{
			Id = new Guid("{4D364429-3DFA-4DFA-B805-725A48379C42}"),
			Title = "Дочерний КЕ",
			InternalName = "KeChild",
			Group = RFCConst.RFCGroup,
			Required = true
		};

	}
}
