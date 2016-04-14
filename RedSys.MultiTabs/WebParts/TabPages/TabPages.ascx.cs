using Microsoft.SharePoint.WebControls;
using RedSys.MultiTabs;
using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI.WebControls.WebParts;

// SharePoint Frontier Multi WebPart Tab Pages WebPart by Ashok Raja .T
// To get updated on latest happenings around SharePoint world , do visit my blog @ http://www.ashokraja.me
// Check out my free webparts for SharePoint 2013 and 2010 @ https://webpartgallery.codeplex.com/

namespace RedSys.MultiTabs.WebParts.TabPages
{
    [ToolboxItemAttribute(false)]
    public partial class TabPages : WebPart
    {
		public TabPages()
        {
        }
        public enum AssetLocation
        {
            RootSite,
            CurrentSite
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                BindAssets( LocationOfAssets == AssetLocation.RootSite);
                RenderTabContainer();
            }
        }

     


        [WebBrowsable(true),
         WebDisplayName("Расположение скриптов"),
         WebDescription("Расположение скриптов и стилей"),
         Personalizable(PersonalizationScope.Shared),
         Category("Вкладки")]
        public AssetLocation LocationOfAssets { get; set; }


        [WebBrowsable(true),
         WebDisplayName("Названия веб-частей"),
         WebDescription("Название веб-частей через ';'"),
         Personalizable(PersonalizationScope.Shared),
         Category("Вкладки")]
        public string WebPartTitles { get; set; }

        protected void BindAssets(bool FromRootSite)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ManageAssets.BindScript("Style Library/RedSys.MultiTabs/jquery.ba-hashchange.min.js", FromRootSite));
            sb.AppendLine(ManageAssets.BindScript("Style Library/RedSys.MultiTabs/jquery.easytabs.min.js", FromRootSite));
            sb.AppendLine(ManageAssets.BindStyle("Style Library/RedSys.MultiTabs/tabs.css", FromRootSite));
            ltBaseScripts.Text = sb.ToString();
        }


        protected void RenderTabContainer()
        {
            StringBuilder sb = new StringBuilder();
            if (IsInputValid())
            {
                string[] Tabs = WebPartTitles.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                sb.AppendLine(@"<div id=""tab-container"" class='tab-container'>");
                sb.AppendLine(@"    <ul class='etabs'>");
                sb.AppendLine(RenderTabHeaders(Tabs));
                sb.AppendLine(@"    </ul>");
                sb.AppendLine(@"    <div class='panel-container'>");
                sb.AppendLine(RenderTabContent(Tabs.Length));
                sb.AppendLine(@"    </div>");
                sb.AppendLine(@"</div>"); 
                ltTabs.Text = sb.ToString();
                ltTabScripts.Text = RenderTabScripts(Tabs);
            }
        }

        protected string RenderTabHeaders(string[] Tabs)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Tabs.Length; i++)
            {
                sb.AppendLine(string.Format(@"        <li class='tab'><a href=""#{0}"">{1}</a></li>", "tab" + i.ToString(), Tabs[i]));
            }
            return sb.ToString();
        }

        protected string RenderTabContent(int TabCount)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < TabCount; i++)
            {
                sb.AppendLine(string.Format(@"        <div id=""{0}""></div>", "tab" + i.ToString()));
            }
            return sb.ToString();
        }

        protected string RenderTabScripts(string[] Tabs)
        {
            string TabParts = RenderTabParts(Tabs);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"" charset=""utf-8"">");
            sb.AppendLine(@"    $(document).ready(function ($) {");
            sb.AppendLine(string.Format(@"        jQTabs([{0}]);", TabParts));
            sb.AppendLine(@"    });");
            sb.AppendLine(@"    function jQTabs(wps) {");
            sb.AppendLine(@"        for (wp in wps) {");
            sb.AppendLine(@"            var tb = wps[wp];");
            sb.AppendLine(@"            $(""span:contains('"" + tb + ""')"").each(function () {");
            sb.AppendLine(@"                if ($(this).text() == tb) {");
            sb.AppendLine(@"                    $(this).closest(""span"").hide().closest(""[id^='MSOZoneCell_WebPart']"").appendTo($(""#tab"" + wp));");
            sb.AppendLine(@"                }");
            sb.AppendLine(@"            });");
            sb.AppendLine(@"        }");
            sb.AppendLine(@"        $(document).ready(function () {");
            sb.AppendLine(@"            $('#tab-container').easytabs();");
            sb.AppendLine(@"        });");
            sb.AppendLine(@"    }");
            sb.AppendLine(@"</script>");
            return sb.ToString();
        }

        protected string RenderTabParts(string[] Tabs)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Tabs.Length; i++)
            {
                sb.Append(string.Format(@"""{0}""{1}", Tabs[i], i == Tabs.Length - 1 ? "" : ","));
            }
            return sb.ToString();
        }

        protected bool IsInputValid()
        {
            string Msg = "Пожалуйста, откройте настройки веб-части и укажите список названий веб-частей, которые необходимо отображать";
            if (string.IsNullOrWhiteSpace(WebPartTitles))
            {
                ltTabs.Text = Msg;
                return false;
            }
            string[] Tabs = WebPartTitles.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (Tabs.Length == 0)
            {
                ltTabs.Text = Msg;
                return false;
            }

            return true;
        }
	}
}
