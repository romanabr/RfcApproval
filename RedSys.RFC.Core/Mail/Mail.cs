using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RedSys.RFC.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Mail
{
	public class MailGenerator
	{
		public string Body;
		public string Subject;
		public List<SPUser> To;
		private SPListItem ListItem;
		public Dictionary<string,string> AdditionalProperty;

		public MailGenerator(List<SPUser> to, string subject, string body, SPListItem listItem)
		{
			To = to;
			Subject = subject;
			Body = body;
			ListItem = listItem;
			AdditionalProperty = new Dictionary<string, string>();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Если развернут шаблон списка MailTemplate</remarks>
		/// <param name="listItem"></param>
		/// <param name="templateName"></param>
		public MailGenerator(SPListItem listItem, string templateName)
		{
			SPWeb web = listItem.Web;
			SPList mailTemplateList = web.GetListExt(Const.Const.MailTemplateListUrl);


			SPListItem mailTemplate = mailTemplateList.GetListItemByTitle(templateName);
			if (mailTemplate == null)
			{
				//TODO. Обработать ошибку
				return;
			}

			Subject = mailTemplate.GetFieldValue(Const.Const.MailSubjectField);
			Body = mailTemplate.GetFieldValue(Const.Const.MailBodyField);
		}

		public MailGenerator(List<SPUser> to, string subject, string body, SPListItem listItem, bool generateSubjectAndBody)
		{
			To = to;
			Subject = subject;
			Body = body;
			ListItem = listItem;
			AdditionalProperty = new Dictionary<string, string>();
			if (generateSubjectAndBody) GenerateSubjectAndBody();
		}

		public MailGenerator(List<SPUser> to, string subject, string body, SPListItem listItem, Dictionary<string,string> additionalProperty)
		{
			To = to;
			Subject = subject;
			Body = body;
			ListItem = listItem;
			AdditionalProperty = additionalProperty;
		}

		public void GenerateSubjectAndBody()
		{
			Subject = GenerateText(Subject);
			Subject = GenerateText(Body);
		}

		protected string GenerateText(string text)
		{
			Regex regex = new Regex("(%%Item:).*?(%%)|(%%Step:).*?(%%)|(%%Common:).*?(%%)");
			//SPListItem stepItem = web.GetListExt(Constant.StageListUrl).GetItemById(this.ID);

			MatchCollection mc = regex.Matches(text);
			foreach (Match m in mc)
			{
				string matchReplace = string.Empty;
				string fieldstring = m.Value.Trim(new char[] { '%' }).Split(new char[] { ':' })[1];

				string fieldvaluestring = string.Empty;
				if (fieldstring.Contains('|'))
				{
					string[] fieldstrings = fieldstring.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
					fieldstring = fieldstrings[0];
					fieldvaluestring = fieldstrings[1];
				}
				try
				{
					if (m.Value.ToLower().StartsWith("%%common"))
					{
						switch (fieldstring)
						{
							case ("TaskUrl"): matchReplace = string.Format("{0}", ListItem.Url); break;
							case ("ItemUrl"): matchReplace = string.Format("{0}", ListItem.Url); break;
							case ("DocSetUrl"): matchReplace = GenerateDocSetUrl(ListItem); break;
							case ("ViewUrl"): matchReplace = SPUtility.ConcatUrls(ListItem.Web.Url, ListItem.ParentList.Views[fieldvaluestring].Url); break;
							case ("Result"): matchReplace = ((AdditionalProperty.ContainsKey(fieldstring) && AdditionalProperty[fieldstring] == "approve") ? fieldvaluestring.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0] : fieldvaluestring.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1]); break;
							case ("DateTimeNow"): DateTime.Now.ToString("yyyy-MM-dd hh:mm"); break;
							case ("DateTimeToday"): matchReplace = DateTime.Today.ToShortDateString(); break;
							default:matchReplace = AdditionalProperty.ContainsKey(fieldstring) ? AdditionalProperty[fieldstring] : string.Empty; break;
							
						}
					}

					else if (m.Value.ToLower().StartsWith("%%item"))
					{
						matchReplace = ListItem.GetFieldValueByType(fieldstring, fieldvaluestring);
					}

					text = text.Replace(m.Value, matchReplace);
				}
				catch(Exception ex)
				{
					//TODO логировать ошибки
				}
			}

			return text;
		}

		private static string GenerateDocSetUrl(SPListItem item)
		{
			string returnString;
			try
			{
				DocumentSet documentSet = DocumentSet.GetDocumentSet(item.Folder);
				returnString = documentSet.WelcomePageUrl;
			}
			catch (Exception ex)
			{
				returnString = string.Format("Error: {0}", ex.Message);
			}
			return returnString;
		}

		public void SendMail()
		{
			SPSecurity.RunWithElevatedPrivileges(delegate ()
			{
				using (SPSite spSite = new SPSite(ListItem.Web.Site.ID))
				{
					foreach (string s in To.Select(u => u.LoginName).Distinct())//Email).Distinct())
					{
						//if(!string.IsNullOrEmpty(s))
						//Microsoft.SharePoint.Utilities.SPUtility.SendEmail(spSite.RootWeb, true, IsBodyHtml, s, Subject, Body);

						String log = "subject\r" + Subject + "\rtext\r" + Body + "\rto\r" + s;
						ExceptionHelper.DUmpMessage(log);

					}
				}
			});
		
	}	

	}
}
