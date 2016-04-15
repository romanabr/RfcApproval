﻿using RedSys.RFC.Core.Helper;
using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls.WebParts;

namespace RedSys.KEWP.KEEffectListView
{
	[ToolboxItemAttribute(false)]
	public partial class KEEffectListView : WebPart
	{
		// Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
		// using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
		// for production. Because the SecurityPermission attribute bypasses the security check for callers of
		// your constructor, it's not recommended for production purposes.
		// [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
		public string ItemId = "0";

		public KEEffectListView()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			InitializeControl();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			QueryString qs = new QueryString(HttpContext.Current.Request.Url.PathAndQuery);
			if (qs.AllParameters.ContainsKey("ID"))
				ItemId = qs["ID"];
		}
	}
}