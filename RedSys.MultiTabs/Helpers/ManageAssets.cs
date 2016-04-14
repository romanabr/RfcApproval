using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SharePoint Frontier Multi WebPart Tab Pages WebPart by Ashok Raja .T
// To get updated on latest happenings around SharePoint world , do visit my blog @ http://www.ashokraja.me
// Check out my free webparts for SharePoint 2013 and 2010 @ https://webpartgallery.codeplex.com/

namespace RedSys.MultiTabs
{
    /// <summary>
    /// To know more on binding scripts and styles in SharePoint 2013 check out my detailed blog post @ http://j.mp/sp2013css
    /// Full Url : http://www.ashokraja.me/post/Refer-Scripts-and-CSS-Style-Sheet-in-SharePoint-2013-Visual-Web-Part-and-Master-Page.aspx
    /// </summary>
    public class ManageAssets
    {
        public static string BindScript(string ScriptFileUrl, bool FromRootSite)
        {
            if (FromRootSite)
                ScriptFileUrl = SPUrlUtility.CombineUrl(SPContext.Current.Site.RootWeb.Url, ScriptFileUrl);
            else
                ScriptFileUrl = SPUrlUtility.CombineUrl(SPContext.Current.Web.Url, ScriptFileUrl);

            return string.Format(@"<script type=""text/javascript"" src=""{0}""></script>", ScriptFileUrl);
        }

        public static string BindStyle(string StyleFileUrl, bool FromRootSite)
        {
            if (FromRootSite)
                StyleFileUrl = SPUrlUtility.CombineUrl(SPContext.Current.Site.RootWeb.Url, StyleFileUrl);
            else
                StyleFileUrl = SPUrlUtility.CombineUrl(SPContext.Current.Web.Url, StyleFileUrl);

            return string.Format(@"<link rel=""stylesheet"" href=""{0}"" type=""text/css"" />", StyleFileUrl);
        }
    }
}
