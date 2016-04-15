using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using RedSys.RFC.Data.Models;
using RedSys.RFC.Data.Lists;
using RedSys.RFC.Data.ContentTypes;
using Microsoft.Office.DocumentManagement.DocumentSets;
using RedSys.RFC.Data;
using RedSys.RFC.Core.Helper;

namespace Console
{
	class Program
	{
		static void Main(string[] args)
		{
			using (SPSite site = new SPSite("http://portal.psdev.com"))
			{
				using (SPWeb web = site.OpenWeb())
				{

                    RFCModel rfcModel = new RFCModel(web);
                    rfcModel.Deploy();
                    RFCWebModel rfcWebModel = new RFCWebModel(web);
                    rfcWebModel.Deploy();
                    RFCListViewModel rlv = new RFCListViewModel(web);
                    rlv.Deploy();
                    VariablesModel vm = new VariablesModel(web);
                    vm.Deploy();
					//SPWeb web = (SPWeb)properties.Feature.Parent;
					//RFCModel rfcModel = new RFCModel(web);
					//rfcModel.Deploy();

					//RFCWebModel rfcWebModel = new RFCWebModel(web);
					//rfcWebModel.Deploy();
				}
			}
		}
	}
}
