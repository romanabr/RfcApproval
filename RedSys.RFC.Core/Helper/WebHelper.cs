using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace RedSys.RFC.Core.Helper
{
	public static class WebHelper
	{
		public static SPList GetListExt(this SPWeb spWeb, string url)
		{
			if (spWeb == null) return null;
			SPList spList = null;
			try
			{
				spList = spWeb.GetList(SPUtility.ConcatUrls(spWeb.Url, url));
			}
			catch
			{
			}
			return spList;
		}
	}
}
