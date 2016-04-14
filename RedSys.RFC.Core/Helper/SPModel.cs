using Microsoft.SharePoint;
using SPMeta2.SSOM.Services;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Helper
{
	public class SPModel
	{
		protected SPWeb currentWeb;
		public SPModel(SPWeb web)
		{
			currentWeb = web;
		}

		public virtual void Deploy()
		{

		}

		protected void DeployModel(WebModelNode webMoldel)
		{
			if (currentWeb != null)
			{
				var ssomProvisionService = new SSOMProvisionService();
				ssomProvisionService.DeployModel(SPMeta2.SSOM.ModelHosts.WebModelHost.FromWeb(currentWeb), webMoldel);

			}
		}

		protected void DeployModel(SiteModelNode model)
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
