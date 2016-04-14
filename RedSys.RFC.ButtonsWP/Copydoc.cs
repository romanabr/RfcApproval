using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace WP_Button.Start_WorkFlow
{
	internal static class Copydoc
	{
		public static int CopyDocSet(string webUrl, string listName, int ID)
		{
			var result = 0;
			try
			{
				if (ID > 0)
				{
					using (var site = new SPSite(webUrl))
					{
						using (var oWeb = site.OpenWeb())
						{
							var oList = oWeb.Lists[listName];
							var oldItem = oList.GetItemById(ID);

							string titleSuffix;
							var run = 0;
							do
							{
								titleSuffix = run == 0 ? "(Копия)" : "(Копия " + run + ")";
								run++;
							} while (oWeb.GetFolder(oWeb.Url + "/" + oldItem.Folder.Url + titleSuffix).Item != null);
							var newUrl = oWeb.Url + "/" + oldItem.Folder.Url + titleSuffix;
							oWeb.AllowUnsafeUpdates = true;

							//SPListItem newItem = oList.AddItem(oldItem.Folder.ParentFolder.Url, oldItem.FileSystemObjectType, oldItem.Title + titleSuffix);                

							var newItemId = CreateNewDocSet(oWeb, oldItem.Title + titleSuffix, "", oList.Title, oldItem.ContentType.Name,
								oldItem.Folder.ParentFolder.ServerRelativeUrl);
							var newItem = oList.GetItemById(newItemId);

							foreach (SPField f in oldItem.ParentList.Fields)
							{
								try
								{
									if (oldItem[f.Title] != null && !f.ReadOnlyField) // && 
										//(f.ShowInEditForm == true || f.ShowInDisplayForm == true || f.ShowInNewForm == true))
									{
										switch (f.Title)
										{
											case "Номер":
												newItem[f.Title] = GetNewNumber(oldItem);
												break;
											case "Штрих-код":
												newItem[f.Title] = "";
												break;
											case "Связанные документы":
												break;
											case "Имя":
												newItem[f.Title] = oldItem[f.Title] + titleSuffix;
												break;
											default:
												newItem[f.Title] = oldItem[f.Title];
												break;
										}
									}
								}
								catch
								{
								}
							}
							newItem.Properties["docset_LastRefresh"] = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.UtcNow);
							newItem["Название"] = oldItem.Title + titleSuffix;
							newItem.Web.AllowUnsafeUpdates = true;
							newItem.SystemUpdate(false);
							foreach (SPFile oFile in oldItem.Folder.Files)
							{
								oFile.Web.AllowUnsafeUpdates = true;
								oFile.CopyTo(newUrl + "/" + oFile.Name);
							}
							Thread.Sleep(1000);
							result = newItem.ID;
							//Page.Response.Redirect(Page.Request.UrlReferrer == null ? newUrl : Page.Request.UrlReferrer.ToString());   
						}
					}
				}
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry("CopyCardService", "Error: " + ex.Message + "\nStacktrace: " + ex.StackTrace,
					EventLogEntryType.Error);
			}
			return result;
		}

		public static int CreateNewDocSet(SPWeb inWeb, string docSetName, string bCode, string listName,
			string contentTypeName, string subfolderName)
		{
			var itemId = 0;
			using (var site = new SPSite(inWeb.Url))
			{
				using (var web = site.OpenWeb())
				{
					var list = web.Lists[listName];
					var docSetCT = list.ContentTypes[contentTypeName];
					var props = new Hashtable();
					props.Add("BarCode", bCode);
					props.Add("IsDocumentSet", 1);
					var destinationPath = subfolderName + "/item";
					EnsureParentFolder(web, destinationPath, list);
					var incFolder = web.GetFolder(subfolderName);
					web.AllowUnsafeUpdates = true;
					web.Update();
					var docSet = DocumentSet.Create(incFolder, docSetName, docSetCT.Id, props, true);
					itemId = docSet.Item.ID;
				}
			}
			return itemId;
		}

		private static string EnsureParentFolder(SPWeb parentWeb, string destinUrl, SPList list)
		{
			destinUrl = parentWeb.GetFile(destinUrl).Url;
			var index = destinUrl.LastIndexOf("/");
			var parentFolderUrl = string.Empty;
			if (index > -1)
			{
				parentFolderUrl = destinUrl.Substring(0, index);
				var parentFolder
					= parentWeb.GetFolder(parentFolderUrl);
				if (!parentFolder.Exists)
				{
					var fld = list.ParentWeb.RootFolder.Url;
					foreach (var folder in parentFolderUrl.Split('/'))
					{
						fld += "/" + folder;
						if (!parentWeb.GetFolder(fld).Exists)
						{
							list.ParentWeb.AllowUnsafeUpdates = true;
							if (parentWeb.GetFolder(fld).ParentListId != null &&
							    parentWeb.GetFolder(fld).ParentWeb.Lists[parentWeb.GetFolder(fld).ParentListId] is SPDocumentLibrary)
							{
								parentWeb.GetFolder(fld).ParentFolder.SubFolders.Add(fld);
							}
							else
							{
								var fold = list.Folders.Add(parentWeb.GetFolder(fld).ParentFolder.ServerRelativeUrl,
									SPFileSystemObjectType.Folder, folder);
								fold.Update();
								fold.Folder.Update();
							}
						}
					}
				}
			}
			return parentFolderUrl;
		}

		private static string GetNewNumber(SPListItem oItem)
		{
			var oWeb = oItem.Web;
			var NumList = oWeb.Lists["Нумератор"];
			var oQuery = new SPQuery();
			oQuery.Query = "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + oItem.ContentType.Name +
			               "</Value></Eq></Where>";
			var Numerators = NumList.GetItems(oQuery);
			if (Numerators.Count > 0)
			{
				var Numerator = Numerators[0];
				var newNumber = Numerator["Следующий номер"].ToString();
				Numerator["Следующий номер"] = int.Parse(newNumber) + 1;
				Numerator.Update();
				return newNumber + "-" + DateTime.Now.Day + "." + DateTime.Now.Month + "." +
				       DateTime.Now.Year.ToString().Substring(2);
			}
			return oItem["Номер"].ToString();
		}
	}
}