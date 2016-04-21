using CamlexNET;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Helper
{
	public static class ListItemHelper
	{
		

		#region SetFieldValue

		public static void SetStatus(SPListItem item, string status)
		{
			item.Web.AllowUnsafeUpdates = true;
			item["Stage"] = status;
		}

		public static void SetFieldValueUser(this SPListItem item,
		  string fieldName, IEnumerable<SPPrincipal> principals)
		{
			if (item != null)
			{
				SPFieldUserValueCollection fieldValues =
				  new SPFieldUserValueCollection();

				foreach (SPPrincipal principal in principals)
				{
					fieldValues.Add(
					  new SPFieldUserValue(
						item.Web, principal.ID, principal.Name));
				}
				item[fieldName] = fieldValues;
			}
		}

		public static void SetFieldValueUser(this SPListItem item,
		  Guid fieldName, List<SPUser> principals)
		{
			if (item != null)
			{
				SPFieldUserValueCollection fieldValues =
				  new SPFieldUserValueCollection();

				foreach (SPUser principal in principals)
				{
					fieldValues.Add(
					  new SPFieldUserValue(
						item.Web, principal.ID, principal.Name));
				}
				item[fieldName] = fieldValues;
			}
		}

		public static void SetFieldValueUser(this SPListItem item,
		 string fieldName, List<SPUser> principals)
		{
			if (item != null)
			{
				SPFieldUserValueCollection fieldValues =
				  new SPFieldUserValueCollection();

				foreach (SPUser principal in principals)
				{
					fieldValues.Add(
					  new SPFieldUserValue(
						item.Web, principal.ID, principal.Name));
				}
				item[fieldName] = fieldValues;
			}
		}
		#endregion

		#region GetFieldValue
		public static string GetFieldValueByType(this SPListItem listItem, string fieldName, string type)
		{
			string retString = string.Empty;
			try
			{
				if (listItem[fieldName] != null)
				{
					switch (type)
					{
						case ("string"): retString = listItem.GetFieldValue(fieldName); break;
						case ("boolean"): retString = (listItem.GetFieldValueBoolean(fieldName) ? "Да" : "Нет"); break;
						case ("calc"): retString = listItem.GetFieldValueCalc(fieldName); break;
						case ("shortdate"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToShortDateString() : string.Empty); break;
						case ("shorttime"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToShortTimeString() : string.Empty); break;
						case ("formatyyyy-MM-dd"): retString = (listItem.GetFieldValueDateTime(fieldName).HasValue ? listItem.GetFieldValueDateTime(fieldName).Value.ToString(type.Replace("format", "")) : string.Empty); break;
						case ("double"): retString = listItem.GetFieldValueDouble(fieldName).ToString(); break;
						case ("int"): retString = listItem.GetFieldValueInt(fieldName).ToString(); break;
						case ("lookupid"): retString = listItem.GetFieldValueLookup(fieldName).LookupId.ToString(); break;
						case ("lookupvalue"): retString = listItem.GetFieldValueLookup(fieldName).LookupValue; break;
						case ("userlogin"): retString = listItem.GetFieldValueUser(fieldName).LoginName; break;
						case ("username"): retString = listItem.GetFieldValueUser(fieldName).Name; break;
						case ("useremail"): retString = listItem.GetFieldValueUser(fieldName).Email; break;
						case ("listusername"): retString = listItem.GetFieldValueUserCollection(fieldName).Select(i => i.Name).Aggregate((i, j) => i + " ," + j); break;
						default: retString = listItem.GetFieldValue(fieldName); break;

					}
				}
			}
			catch (Exception)
			{
			}

			return retString;
		}

		public static string GetFieldValueCalc(this SPListItem item, string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return string.Empty;
			}
			try
			{
				if (item != null)
				{
					SPFieldCalculated fieldCalc = (SPFieldCalculated)item.Fields.GetField(fieldName);
					if (fieldCalc == null) return string.Empty;
					return fieldCalc.GetFieldValueAsText(item[fieldName]);
				}
				else
				{
					return string.Empty; ;
				}
			}
			catch
			{
				return null;
			}
		}

		public static List<SPUser> GetFieldValueUserCollection(this SPListItem item, string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}
			try
			{
				if (item != null)
				{
					List<SPUser> users = new List<SPUser>();
					SPFieldUserValueCollection userValue = new SPFieldUserValueCollection(item.Web, item[fieldName].ToString());
					if (userValue != null && userValue.Count > 0)
					{

						foreach (SPFieldUserValue user in userValue)
						{
							if (user.LookupId != 0)
							{
								users.Add(item.Web.SiteUsers.GetByID(user.LookupId));
							}
							else
								users.Add(user.User);
						}

					}
					return users;
				}
				else
				{
					return null;
				}
			}
			catch
			{
				return null;
			}
		}


		public static string GetFieldValue(this SPListItem listItem, string fieldName)
		{
			string text = string.Empty;
			if (fieldName == string.Empty)
			{
				return text;
			}
			try
			{
				object myObj = listItem[fieldName];
				return ((myObj != null) ? myObj.ToString() : string.Empty);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string GetFieldValue(this SPListItem listItem, Guid fieldName)
		{
			string text = string.Empty;
			if (fieldName == Guid.Empty)
			{
				return text;
			}
			try
			{
				object myObj = listItem[fieldName];
				return ((myObj != null) ? myObj.ToString() : string.Empty);
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static SPFieldLookupValue GetFieldValueLookup(this SPListItem listItem, Guid fieldName)
		{
			if (fieldName == Guid.Empty)
			{
				return null;
			}
			try
			{
				SPFieldLookupValue spFieldLookupValue = new SPFieldLookupValue(listItem[fieldName].ToString());
				return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static SPFieldLookupValueCollection GetFieldValueLookupMulti(this SPListItem listItem, Guid fieldName)
		{
			if (fieldName == Guid.Empty)
			{
				return null;
			}
			try
			{
				SPFieldLookupValueCollection spFieldLookupValue = new SPFieldLookupValueCollection(listItem[fieldName].ToString());
				return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static SPFieldLookupValueCollection GetFieldValueLookupMulti(this SPListItem listItem, string fieldName)
		{
			if (fieldName == String.Empty)
			{
				return null;
			}
			try
			{
				SPFieldLookupValueCollection spFieldLookupValue = new SPFieldLookupValueCollection(listItem[fieldName].ToString());
				return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static SPFieldLookupValue GetFieldValueLookup(this SPListItem listItem, string fieldName)
		{
			if (fieldName == string.Empty)
			{
				return null;
			}
			try
			{
				SPFieldLookupValue spFieldLookupValue = new SPFieldLookupValue(listItem[fieldName].ToString());
				return ((spFieldLookupValue != null) ? spFieldLookupValue : null);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool GetFieldValueBoolean(this SPListItem listItem, Guid fieldName)
		{
			bool retBool = false;
			if (fieldName == Guid.Empty)
			{
				return retBool;
			}
			try
			{
				if (listItem != null)
				{
					if (listItem[fieldName] == null) return retBool;
					retBool = (bool)listItem[fieldName];
				}
				else
				{
					return retBool;
				}
			}
			catch (Exception)
			{
				return retBool;
			}
			return retBool;

		}

		public static bool GetFieldValueBoolean(this SPListItem listItem, string fieldName)
		{
			bool retBool = false;
			if (fieldName == string.Empty)
			{
				return retBool;
			}
			try
			{
				if (listItem != null)
				{
					if (listItem[fieldName] == null) return retBool;
					retBool = (bool)listItem[fieldName];
				}
				else
				{
					return retBool;
				}
			}
			catch (Exception)
			{
				return retBool;
			}
			return retBool;

		}

		public static SPUser GetFieldValueUser(this SPListItem item, Guid fieldName)
		{
			if (fieldName == Guid.Empty)
			{
				return null;
			}
			try
			{
				if (item != null)
				{
					SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[fieldName] as string);
					return userValue.User;
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static SPUser GetFieldValueUser(this SPListItem item, string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}
			try
			{
				if (item != null)
				{
					SPFieldUserValue userValue = new SPFieldUserValue(item.Web, item[fieldName] as string);
					if (userValue != null && userValue.User == null && userValue.LookupId != 0)
					{
						return item.Web.SiteUsers.GetByID(userValue.LookupId);
					}
					return userValue.User;
				}
				else
				{
					return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static DateTime? GetFieldValueDateTime(this SPListItem listItem, Guid fieldName)
		{
			if (fieldName == Guid.Empty)
			{
				return null;
			}
			try
			{
				if (listItem != null)
				{
					object myObj = listItem[fieldName];
					if (myObj == null)
					{
						return null;
					}
					else
					{
						return SPUtility.CreateDateTimeFromISO8601DateTimeString(listItem[fieldName].ToString());
					}
				}
			}
			catch (Exception)
			{
				DateTime res;
				bool r = DateTime.TryParse(listItem[fieldName].ToString(), out res);
				if (r)
					return res;
				else
					return null;
			}
			return null;
		}

		public static DateTime? GetFieldValueDateTime(this SPListItem listItem, string fieldName)
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}
			try
			{
				if (listItem != null)
				{
					object myObj = listItem[fieldName];
					if (myObj == null)
					{
						return null;
					}
					else
					{
						return SPUtility.CreateDateTimeFromISO8601DateTimeString(listItem[fieldName].ToString());
					}
				}
			}
			catch (Exception)
			{
				DateTime res;
				bool r = DateTime.TryParse(listItem[fieldName].ToString(), out res);
				if (r)
					return res;
				else
					return null;
			}
			return null;
		}

		public static int GetFieldValueInt(this SPListItem listItem, Guid fieldName)
		{
			string retString = GetFieldValue(listItem, fieldName);
			int retInt = 0;
			Int32.TryParse(retString, out retInt);
			return retInt;
		}
		public static int GetFieldValueInt(this SPListItem listItem, string fieldName)
		{
			string retString = GetFieldValue(listItem, fieldName);
			int retInt = 0;
			Int32.TryParse(retString, out retInt);
			return retInt;
		}

		public static double GetFieldValueDouble(this SPListItem listItem, Guid fieldName)
		{
			string retString = GetFieldValue(listItem, fieldName);
			double retInt = 0;
			double.TryParse(retString, out retInt);
			return retInt;
		}

		public static double GetFieldValueDouble(this SPListItem listItem, string fieldName)
		{
			string retString = GetFieldValue(listItem, fieldName);
			double retInt = 0;
			double.TryParse(retString, out retInt);
			return retInt;
		}
		#endregion

		#region web

		

		#endregion

		#region parser

		public static bool IsTrue(this string value)
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

		#endregion

		

		

		#region Query
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
        #endregion


        public static void GracefulSPListItemUpdate(SPListItem curListItem, bool generateNewVersion)
        {
            var itemID = curListItem.ID;
            bool tryAgain = false;
            #region Prevent message "has been modified blah-blah-blah"
            do
            {
                try
                {
                    if (generateNewVersion)
                        curListItem.Update();
                    else
                        curListItem.SystemUpdate(false);

                    tryAgain = false;
                }
                catch (SPException ex2)
                {
                    COMException ex = ex2.InnerException as COMException;
                    // 0x81020037
                    if (ex != null && (uint)ex.ErrorCode == 0x81020037)
                    {
                        curListItem = curListItem.ParentList.GetItemById(itemID);
                        System.Threading.Thread.Sleep(1000 * 2);
                        tryAgain = true;
                    }
                    else
                        throw ex2;
                }
            } while (tryAgain == true);
            #endregion
        }

        public static void GracefulSPListItemUpdate(SPListItem curListItem, bool generateNewVersion, System.Action<SPListItem> doUpdate)
        {
            var itemID = curListItem.ID;
            bool tryAgain = false;
            #region Prevent message "has been modified blah-blah-blah"
            do
            {
                try
                {
                    doUpdate(curListItem);

                    if (generateNewVersion)
                        curListItem.Update();
                    else
                        curListItem.SystemUpdate(false);

                    tryAgain = false;
                }
                catch (SPException ex2)
                {
                    ExceptionHelper.DUmpException(ex2);
                    COMException ex = ex2.InnerException as COMException;
                    // 0x81020037
                    //if (ex != null && (uint)ex.ErrorCode == 0x81020037)
                    //{
                    curListItem = curListItem.ParentList.GetItemById(itemID);
                    System.Threading.Thread.Sleep(1000);
                    tryAgain = true;
                    //}
                    //else
                    //    throw ex2;
                }
            } while (tryAgain == true);
            #endregion
        }
    }
}
