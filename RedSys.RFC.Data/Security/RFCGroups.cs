using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Security
{
	public static class RFCGroups
	{
		/// <summary>
		/// Администратор АРР
		/// </summary>
		public static SecurityGroupDefinition AdminAPP = new SecurityGroupDefinition
		{
			Name = "Администратор АРР",
			Description = "Администратор АРР",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///	<summary>
		/// Администратор БД
		/// </summary>
		public static SecurityGroupDefinition AdminDB = new SecurityGroupDefinition
		{
			Name = "Администратор БД",
			Description = "Администратор БД",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};


		///<summary>
		///Администратор телефонного оборудования
		///</summary>
		public static SecurityGroupDefinition AdminPhone = new SecurityGroupDefinition
		{
			Name = "Администратор телефонного оборудования",
			Description = "Администратор телефонного оборудования",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///<summary>
		///Релизменеджер АРР
		///</summary>

		public static SecurityGroupDefinition ReliseManagerAPP = new SecurityGroupDefinition
		{
			Name = "Релизменеджер АРР",
			Description = "Релизменеджер АРР",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///<summary>
		///Менеджер по тестированию АРР
		///</summary>

		public static SecurityGroupDefinition ManagerTestingAPP = new SecurityGroupDefinition
		{
			Name = "Менеджер по тестированию АРР",
			Description = "Менеджер по тестированию АРР",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///<summary>
		///Ключевой пользователь АРР
		///</summary>	

		public static SecurityGroupDefinition KeyUserAPP = new SecurityGroupDefinition
		{
			Name = "Ключевой пользователь АРР",
			Description = "Ключевой пользователь АРР",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///<summary>
		///Менеджер-владелец бизнес-приложений
		///</summary>

		public static SecurityGroupDefinition ManagerOWnerBusinessApplication = new SecurityGroupDefinition
		{
			Name = "Менеджер-владелец бизнес-приложений",
			Description = "Менеджер-владелец бизнес-приложений",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		/// <summary>
		/// Менеджер-владелец БД
		/// </summary>

		public static SecurityGroupDefinition ManagerOwnerDB = new SecurityGroupDefinition
		{
			Name = "Менеджер-владелец БД",
			Description = "Менеджер-владелец БД",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

		///<summary>
		///Менеджер-владелец ATS
		///</summary>

		public static SecurityGroupDefinition ManagerOWnerATS = new SecurityGroupDefinition
		{
			Name = "Менеджер-владелец ATS",
			Description = "Менеджер-владелец ATS",
			AllowMembersEditMembership = false,
			OnlyAllowMembersViewMembership = false
		};

	}
}
