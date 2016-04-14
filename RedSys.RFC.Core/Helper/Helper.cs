using CamlexNET;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Helper
{
	public static class Helper
	{
		private static bool IsTrue(this string value)
		{
			try
			{
				// 1
				// Avoid exceptions
				if (value == null)
				{
					return false;
				}

				// 2
				// Remove whitespace from string
				value = value.Trim();

				// 3
				// Lowercase the string
				value = value.ToLower();

				// 4
				// Check for word true
				if (value == "true")
				{
					return true;
				}

				// 5
				// Check for letter true
				if (value == "t")
				{
					return true;
				}

				// 6
				// Check for one
				if (value == "1")
				{
					return true;
				}

				// 7
				// Check for word yes
				if (value == "yes")
				{
					return true;
				}

				// 8
				// Check for letter yes
				if (value == "y")
				{
					return true;
				}

				// 9
				// It is false
				return false;
			}
			catch
			{
				return false;
			}
		}


		public static SPListItemCollection GetItemsByValue(SPWeb web, string listName, string displayFieldName, string filterValue)
		{
			SPQuery search = new SPQuery();
			search.ViewAttributes = "Scope=\"RecursiveAll\"";
			SPListItemCollection resultItems = null;
			if (filterValue.IndexOf(";#") > -1)
				filterValue = filterValue.Substring(filterValue.IndexOf("#") + 1);
			try
			{
				SPList tList = web.Lists[listName];
				search.Query = "<Where><Eq><FieldRef Name='" + tList.Fields[displayFieldName].InternalName + "' /><Value Type='Text'>" + filterValue + "</Value></Eq></Where>";
				resultItems = tList.GetItems(search);
			}
			catch (Exception ex)
			{
				ExceptionHelper.DUmpException(ex);
			}

			return resultItems;
		}


		public static SPListItemCollection GetItemsByValue(SPWeb web, string listName, string filterString)
		{
			var expressions = new List<Expression<Func<SPListItem, bool>>>();

			SPQuery search = new SPQuery();
			search.ViewAttributes = "Scope=\"RecursiveAll\"";
			SPListItemCollection resultItems = null;
			SPList tList = web.Lists[listName];

			string[] filterPairs = filterString.Split(';');

			try
			{
				foreach (string pair in filterPairs)
				{
					string fieldName = pair.Split('=')[0];
					string fieldValue = pair.Split('=')[1];
					if (fieldValue.IndexOf(";#") > -1)
						fieldValue = fieldValue.Split('#')[1];

					switch (fieldValue)
					{
						case "null": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == null); break;
						case "notnull": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) != null); break;
						default:
							if (fieldName.Contains('|'))
							{
								expressions.Add(f => f[tList.Fields[fieldName.Replace("|", "")].InternalName] == (DataTypes.LookupId)fieldValue);
							}
							else
								expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == fieldValue);
							break;
					}
				}
				search.Query = Camlex.Query().WhereAll(expressions).ToString();
				resultItems = tList.GetItems(search);
			}
			catch { }
			return resultItems;
		}


		static SPListItemCollection GetItemsByFilterExpression(SPWeb web, string listName, string filterString)
		{
			var expressions = new List<Expression<Func<SPListItem, bool>>>();

			SPQuery search = new SPQuery();
			search.ViewAttributes = "Scope=\"RecursiveAll\"";
			SPListItemCollection resultItems = null;
			SPList tList = web.Lists[listName];

			string[] filterPairs = filterString.Split(';');

			try
			{
				foreach (string pair in filterPairs)
				{
					string fieldName = pair.Split('=')[0];
					string fieldValue = pair.Split('=')[1];
					if (fieldValue.IndexOf(";#") > -1)
						fieldValue = fieldValue.Split('#')[1];

					switch (fieldValue)
					{
						case "null": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == null); break;
						case "notnull": expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) != null); break;
						default: expressions.Add(f => ((string)f[tList.Fields[fieldName].InternalName]) == fieldValue); break;
					}
				}

				search.Query = Camlex.Query().WhereAll(expressions).ToString();
				resultItems = tList.GetItems(search);
			}
			catch (Exception ex)
			{
				ExceptionHelper.DUmpException(ex);
			}

			return resultItems;
		}


		public static SPListItem ToArchive(SPListItem inItem, bool doArchiveStatus, bool toArchive, System.Action failureHandler)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

			SPListItem resultItem = null;
			SPSecurity.RunWithElevatedPrivileges(delegate ()
			{
				using (SPSite site = new SPSite(inItem.Web.Url))
				{
					using (SPWeb web = site.OpenWeb())
					{
						web.AllowUnsafeUpdates = true;
						SPList list = web.Lists[inItem.ParentList.Title];
						SPListItem item = null;

						try
						{
							item = list.GetItemById(inItem.ID);
						}
						catch (Exception ex)
						{
							ExceptionHelper.DUmpException(ex);
							return;
						}


						SPListItemCollection routs = Helper.GetItemsByValue(web, "Маршруты", "Тип контента", item.ContentType.Name);

						if (routs.Count == 0)
							return;
						if (routs.Count > 1)
							throw new NotSupportedException("Маршруты" + " : Too many routes found for " + item.ContentType.Name);

						string dpath;
						string dlib;
						if (toArchive)
						{
							dpath = "Путь архивирования";
							dlib = "Библиотека архивирования";
						}
						else
						{
							dpath = "Путь рабочий";
							dlib = "Библиотека рабочая";
						}

						string folderPathTemplate = routs[0][dpath].ToString();

						#region Convert folder's path template to a real folder's path
						string[] folders = folderPathTemplate.Split(';');
						foreach (string folder in folders)
						{
							string fieldvalue = string.Empty;


							if (item.Fields.ContainsField(folder))
							{
								if (item.Fields[folder] != null && item.Fields[folder].ToString() != "")
								{
									if (item.Fields[folder].TypeAsString == "PSELookup" || item.Fields[folder].TypeAsString == "LookupFieldWithPicker" || item.Fields[folder].Type == SPFieldType.Lookup)
									{
										SPFieldLookupValue val = (SPFieldLookupValue)item[folder];
										fieldvalue = val.LookupValue;
									}
									else if (item.Fields[folder].TypeAsString == "Calculated")
									{
										fieldvalue = Convert.ToString(item[folder]).Replace("string;#", "").Trim();
									}
									else
										fieldvalue = Convert.ToString(item[folder]);
								}
								else
									fieldvalue = folder;
							}
							else if (folder.ToLower() == "год")
							{
								fieldvalue = item.GetFieldValueDateTime(SPBuiltInFieldId.Created).Value.Year.ToString();
							}
							else
								fieldvalue = folder;

							if (fieldvalue != "")
							{
								fieldvalue = PrepareToMoveTo(fieldvalue);
								folderPathTemplate = folderPathTemplate.Replace(folder, fieldvalue);
							}
						}
						folderPathTemplate = folderPathTemplate.Replace(";", "/");
						var destinationPath = folderPathTemplate;
						#endregion

						using (EventReceiverManager erm = new EventReceiverManager(true))
						{
							SPList destList = web.Lists[routs[0][dlib].ToString()];
							web.GetFolder(destinationPath + "/item");
							item.Web.AllowUnsafeUpdates = true;
							item.Folder.MoveTo(destinationPath + "/" + item.DisplayName);
							item.Web.AllowUnsafeUpdates = false;

							SPFolder movedItem = web.GetFolder(destinationPath + "/" + item.DisplayName);
							movedItem.Item["ContentTypeId"] = destList.ContentTypes[item.ContentType.Name].Id;
							movedItem.Item["IsDocumentSet"] = true;
							if (doArchiveStatus)
								movedItem.Item["Статус рабочего процесса"] = "Архивный";
							movedItem.Item.ProgId = "SharePoint.DocumentSet";
							movedItem.Item.Web.AllowUnsafeUpdates = true;
							movedItem.Item.SystemUpdate(false);
							movedItem.Item.Web.AllowUnsafeUpdates = false;

							erm.StartEventReceiver();

							resultItem = movedItem.Item;
						}
					}
				}
			});

			return resultItem;
		}

		public static string PrepareToMoveTo(string fieldValue)
		{
			//fieldvalue = Regex.Replace(fieldvalue, @"[!@\'\""\x23\$\x25\x5F_\\]+", "", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant);
			fieldValue = Regex.Replace(fieldValue, @"[^a-zA-Z0-9.,а-яА-Я\x28\x29\x09\x20\x2D]+", "", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant);
			fieldValue = Regex.Replace(fieldValue, @"~|""|#|%|&|\*|:|<|>|\?|\/|\\|{|\||}|\W*$|^\W*", "", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.CultureInvariant);


			return fieldValue;
		}
	}
}
