using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using RedSys.RFC.Core.Helper;
using CamlexNET;

namespace PSE.CustomUpload
{
    public class URLRedirectHttpModule : IHttpModule
    {
        private HttpApplication app;

        public void Init(HttpApplication context)
        {
            app = context;
            app.PreRequestHandlerExecute += context_PreRequestHandlerExecute;
        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpResponse res = app.Response;
            HttpRequest req = app.Request;

            var mainQS = new QueryString(HttpContext.Current.Request.Url.AbsoluteUri);
            if (mainQS["List"] != "" && req.Url.LocalPath.ToUpper() == @"/_LAYOUTS/15/UPLOAD.ASPX")
            {
                var listGuid = Guid.Parse(mainQS["List"]);
                var lst = SPContext.Current.Web.Lists[listGuid];


                var web = SPContext.Current.Web;
                var lstVar = web.Lists["Variables"];
                SPQuery search = new SPQuery();
                search.ViewAttributes = "Scope=\"RecursiveAll\"";
                SPListItemCollection resultItems = null;
                search.Query = Camlex.Query().Where(x => (string)x["Title"] == "PSE.CustomUpload - Список").ToString();
                resultItems = lstVar.GetItems(search);
                string listSTR = "";
                if (resultItems.Count == 1)
                    listSTR = Convert.ToString(resultItems[0]["PSEValue"]);
                if (listSTR.Contains(lst.Title))
                {
                    QueryString customUploadPage;
                    customUploadPage = new QueryString("PSE.CustomUpload/CustomUpload.aspx");
                    foreach (var pars in mainQS.AllParameters)
                    {
                        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!
                        /// Don't use pars.Value - it's encoded
                        customUploadPage[pars.Key] = mainQS[pars.Key];
                    }

                    var newUrl = customUploadPage.ToString();
                    SPUtility.Redirect(newUrl, SPRedirectFlags.Trusted | SPRedirectFlags.Static | SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);
                }
            }
        }
        public void Dispose() { }

    }
}
