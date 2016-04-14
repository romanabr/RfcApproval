using RedSys.RFC.Core.Helper;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RedSys.KEWP.KEListView
{
	public partial class KEListViewUserControl : UserControl
	{
		public string ItemId = "0";

		protected void Page_Load(object sender, EventArgs e)
		{
			QueryString qs = new QueryString(HttpContext.Current.Request.Url.PathAndQuery);
			if (qs.AllParameters.ContainsKey("ID"))
				ItemId = qs["ID"];
		}
	}
}
