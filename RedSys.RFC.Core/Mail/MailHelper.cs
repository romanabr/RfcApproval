using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Mail
{
	public static class MailHelper
	{
		public static bool SendMail(string subject, string body, bool isBodyHtml, string to, Guid siteId)
		{
			bool retBool = false;
			SPSecurity.RunWithElevatedPrivileges(delegate
			{
				using (SPSite spSite = new SPSite(siteId))
				{
					string[] logins = to.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (string s in logins)
					{
						//SPUser u = spSite.RootWeb.SiteUsers[s];
						//string am = u.Email;
						//if(!string.IsNullOrEmpty(am))
						//    retBool =  SPUtility.SendEmail(spSite.RootWeb, true, false, am, subject, body);
						String log = "subject\r" + subject + "\rtext\r" + body + "\rto\r" + s;
						ExceptionHelper.DUmpMessage(log);
					}
				}
			});
			return retBool;
		}
	}
}
