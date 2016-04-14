using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.Utilities;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.ContentTypes;
using RedSys.RFC.Data.Lists;
using SPMeta2.BuiltInDefinitions;
using SPMeta2.Definitions;
using SPMeta2.SSOM.Services;
using SPMeta2.Syntax.Default;
using SPMeta2.Syntax.Default.Utils;
using SPMeta2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Models
{
	public class RFCWebModel:SPModel
	{
		public RFCWebModel(SPWeb web): base(web)
		{
			
		}

		public override void Deploy()
		{
			ClearQuickLaunch();
			SetDefaultPage();
			DeployHierarchicalQuickLaunchNavigation();
			DeployUserCustomActionWithJquery();
		}

		private void ClearQuickLaunch()
		{
			SPNavigationNodeCollection nodes = currentWeb.Navigation.QuickLaunch;
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				nodes[i].Delete();
			}
		}

		private void SetDefaultPage()
		{
			SPFolder folder = currentWeb.RootFolder;
			SPList list = currentWeb.GetListExt(RFCLists.RFCListDefinition.CustomUrl);
			folder.WelcomePage = list.DefaultView.Url;
			folder.Update();

			SPList docLibrary = currentWeb.GetListExt(BuiltInListDefinitions.StyleLibrary.CustomUrl);
			currentWeb.SiteLogoUrl = SPUtility.ConcatUrls(docLibrary.RootFolder.ServerRelativeUrl, "RFC/SiteLogo.png");
			currentWeb.SiteLogoDescription = "Билайн. Управление изменениями";
			currentWeb.Update();

			
			SPFile colorPaletteFile = currentWeb.GetFile("/_catalogs/theme/15/palette032.spcolor");
			if (null == colorPaletteFile || !colorPaletteFile.Exists)
			{
				return;
			}
			
			SPFile fontSchemeFile = currentWeb.GetFile("/_catalogs/theme/15/SharePointPersonality.spfont");
			if (null == fontSchemeFile || !fontSchemeFile.Exists)
			{
				return;
			}

			SPTheme theme = SPTheme.Open("RFCTheme", colorPaletteFile, fontSchemeFile);
			theme.ApplyTo(currentWeb, true);
		}

		private void DeployHierarchicalQuickLaunchNavigation()
		{
			SPList list = currentWeb.GetListExt(RFCLists.RFCListDefinition.CustomUrl);
			SPList document = currentWeb.GetListExt(BuiltInListDefinitions.SharedDocuments.CustomUrl);
			// top level departments node
			var departments = new QuickLaunchNavigationNodeDefinition
			{
				Title = "Управление изменениями",
				Url = list != null ? list.DefaultViewUrl : "test.aspx",
				IsExternal = true
			};

			var hr = new QuickLaunchNavigationNodeDefinition
			{
				Title = "Документация",
				Url = document !=null ? document.DefaultViewUrl : "test.aspx",
				IsExternal = true
			};

			var it = new QuickLaunchNavigationNodeDefinition
			{
				Title = "Мои запросы",
				Url = list != null ? list.Views[RFCViews.MyRFC.Title].Url : "test.aspx",
				IsExternal = true
			};

			// top level clients node
			var clients = new QuickLaunchNavigationNodeDefinition
			{
				Title = "Все запросы",
				Url = list != null ? list.DefaultViewUrl: "test.aspx",
				IsExternal = true
			};
			var onapprove = new QuickLaunchNavigationNodeDefinition
			{
				Title = "На согласовании",
				Url = list != null ? list.Views[RFCViews.OnApprove.Title].Url : "test.aspx",
				IsExternal = true
			};
			var create = new QuickLaunchNavigationNodeDefinition
			{
				Title = "Создать запрос на изменение",
				Url = list != null ?  "/_layouts/15/NewDocSet.aspx?List=" + list.ID.ToString("B")+  "&ContentTypeId="+ list.ContentTypes[RFCContentType.RfcDocSet.Name].Id.ToString() +"&RootFolder=" +SPEncode.UrlEncode(list.RootFolder.Url) : "test.aspx",
				IsExternal = true
			};

			var model = SPMeta2Model.NewWebModel(web =>
			{
				web
					.AddQuickLaunchNavigationNode(departments, node =>
					{
						node
							.AddQuickLaunchNavigationNode(create)
							.AddQuickLaunchNavigationNode(hr)
							.AddQuickLaunchNavigationNode(it)
							.AddQuickLaunchNavigationNode(clients)
							.AddQuickLaunchNavigationNode(onapprove);
					});
			});

			DeployModel(model);
		}


		private void DeployUserCustomActionWithJquery()
		{
			

			var appScriptsFolder = new FolderDefinition
			{
				Name = "JSLink"
			};

			var jQueryCustomAction = new UserCustomActionDefinition
			{
				Name = "m2jQuery",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"jquery-1.12.2.min.js"
				}),
				Sequence = 1500
			};

			var jQueryMigrateCustomAction = new UserCustomActionDefinition
			{
				Name = "m2jQueryMigrate",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"jquery-migrate.js"
				}),
				Sequence = 1501
			};

			var SPUtilityCustomAction = new UserCustomActionDefinition
			{
				Name = "SPUtility",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"SPUtility.js"
				}),
				Sequence = 1502
			};

			var jQueryUICustomAction = new UserCustomActionDefinition
			{
				Name = "m2jQueryUI",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"jquery-ui-1.11.4.min.js"
				}),
				Sequence = 1503
			};

			var SPPlusCustomAction = new UserCustomActionDefinition
			{
				Name = "SPPlus",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"SPPlus.js"
				}),
				Sequence = 1504
			};

			var CamlJsCustomAction = new UserCustomActionDefinition
			{
				Name = "CamlJs",
				Location = "ScriptLink",
				ScriptSrc = UrlUtility.CombineUrl(new string[]
				{
					"~sitecollection",
					BuiltInListDefinitions.StyleLibrary.CustomUrl,
					appScriptsFolder.Name,
					"CamlJs.js"
				}),
				Sequence = 1505
			};

			var siteModel = SPMeta2Model.NewSiteModel(site =>
			{
				site
				  .AddUserCustomAction(jQueryCustomAction)
				  .AddUserCustomAction(jQueryMigrateCustomAction)
				  .AddUserCustomAction(jQueryUICustomAction)
				  .AddUserCustomAction(SPUtilityCustomAction)
				  .AddUserCustomAction(SPPlusCustomAction)
				  .AddUserCustomAction(CamlJsCustomAction);
			});

			DeployModel(siteModel);
		}

	}
}
