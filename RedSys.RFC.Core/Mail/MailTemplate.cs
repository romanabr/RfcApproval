using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;
using SPMeta2.SSOM.Services;

namespace RedSys.RFC.Core.Mail
{
	public class MailTemplate
	{
		private SPWeb currentWeb;
		public MailTemplate(SPWeb web)
		{
			currentWeb = web;
		}

		public static NoteFieldDefinition MailBodyField = new NoteFieldDefinition
		{
			Id = new Guid("8b6f91a6-e376-43a6-9dc3-b23f98b4d200"),
			Title = "Mail Body Template",
			InternalName = Const.Const.MailBodyField,
			Group = Const.Const.PSECommon,
			RichText = false,
			Required = true,
		};

		public static TextFieldDefinition MailSubjectField = new TextFieldDefinition
		{
			Id = new Guid("8b6f91a6-e376-43a6-9dc3-b23f98b4d201"),
			Title = "Mail Subject Template",
			InternalName =Const.Const.MailSubjectField,
			Group = Const.Const.PSECommon,
			Required = true,
		};

		public static ListDefinition MailTemplateList = new ListDefinition
		{
			Title = "Mail Template",
			CustomUrl = "Lists/MailTemplateList",
			ContentTypesEnabled = true,
			TemplateType = BuiltInListTemplateTypeId.GenericList,
		};

		public void Deploy()
		{
			var sitemodel = SPMeta2Model.NewSiteModel(site =>
			{
				site.AddField(MailSubjectField)
					.AddField(MailBodyField);
			});

			var webmodel = SPMeta2Model.NewWebModel(web =>
			{
				web.AddList(MailTemplateList);
			});

			DeployModel(sitemodel);
			DeployModel(webmodel);
		}

		private void DeployModel(WebModelNode webMoldel)
		{
			if (currentWeb != null)
			{
				var ssomProvisionService = new SSOMProvisionService();
				ssomProvisionService.DeployModel(SPMeta2.SSOM.ModelHosts.WebModelHost.FromWeb(currentWeb), webMoldel);

			}
		}

		private void DeployModel(SiteModelNode model)
		{
			if (currentWeb != null)
			{
				using (var site = new SPSite(currentWeb.Site.Url))
				{
					var ssomProvisionService = new SSOMProvisionService();
					ssomProvisionService.DeployModel(SPMeta2.SSOM.ModelHosts.SiteModelHost.FromSite(site), model);
				}
			}
		}

	}
}
