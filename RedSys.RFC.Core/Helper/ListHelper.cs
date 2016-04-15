using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace RedSys.RFC.Core.Helper
{
	public static class ListHelper
	{
		public static SPListItem GetListItemByTitle(this SPList list, string title)
		{
			SPQuery query = new SPQuery();
			query.Query = string.Format("<Where><Eq><FieldRef Name=\"Title\" /><Value Type=\"Text\">{0}</Value></Eq></Where>",
				title);
			query.RowLimit = 1;
			SPListItemCollection items = list.GetItems(query);
			if (items == null) return null;
			SPListItem item = items[0];
			return item;
		}

		public static SPListItem GetListItemByTitle(this SPList list, string title, string viewfields)
		{
			SPQuery query = new SPQuery();
			query.Query = string.Format("<Where><Eq><FieldRef Name=\"Title\" /><Value Type=\"Text\">{0}</Value></Eq></Where>",
				title);
			query.RowLimit = 1;
			query.ViewFields = viewfields;
			SPListItemCollection items = list.GetItems(query);
			if (items == null) return null;
			SPListItem item = items[0];
			return item;
		}

		public static SPFolder GetFolder(this SPList targetList, string folderUrl)
		{
			if (string.IsNullOrEmpty(folderUrl))
				return targetList.RootFolder;

			SPFolder folder = targetList.ParentWeb.GetFolder(targetList.RootFolder.Url + "/" + folderUrl);

			if (!folder.Exists)
			{
				if (!targetList.EnableFolderCreation)
				{
					targetList.EnableFolderCreation = true;
					targetList.Update();
				}

				// We couldn't find the folder so create it
				string[] folders = folderUrl.Trim('/').Split('/');

				string folderPath = string.Empty;
				for (int i = 0; i < folders.Length; i++)
				{
					folderPath += "/" + folders[i];
					folder = targetList.ParentWeb.GetFolder(targetList.RootFolder.Url + folderPath);
					if (!folder.Exists)
					{
						SPListItem newFolder = targetList.Items.Add("", SPFileSystemObjectType.Folder, folderPath.Trim('/'));
						newFolder.Update();
						folder = newFolder.Folder;
					}
				}
			}
			// Still no folder so error out
			if (folder == null)
				throw new SPException(string.Format("The folder '{0}' could not be found.", folderUrl));
			return folder;
		}


	}
}
