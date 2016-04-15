using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.ContentTypes;
using RedSys.RFC.Data.Lists;
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
		public string NewFormUrl;
		public string ContentTypeId;
		public string RootWebUrl;

		public string ListName = string.Empty;

		protected void Page_Load(object sender, EventArgs e)
		{
			QueryString qs = new QueryString(HttpContext.Current.Request.Url.PathAndQuery);
			if (qs.AllParameters.ContainsKey("ID"))
				ItemId = qs["ID"];

			SPWeb web = SPContext.Current.Web;
			NewFormUrl = SPUtility.ConcatUrls(SPUtility.ConcatUrls(web.Url, RFCLists.RfcKeList.CustomUrl), "NewForm.aspx");
			SPContentType ct = web.ContentTypes[RFCContentType.RfcKe.Name];
			ContentTypeId = ct.Id.ToString();
			RootWebUrl = SPEncode.UrlEncode(RFCLists.RfcKeList.CustomUrl);
			SPList list = web.GetListExt(RFCLists.RfcKeList.CustomUrl);
			if (list != null)
				ListName = list.Title;
		}
	}
}
