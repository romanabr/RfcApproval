using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.Lists;
using SPMeta2.Definitions;
using SPMeta2.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Code
{
	public class RFCKeEventReceiver : SPItemEventReceiver
	{
		public override void ItemAdded(SPItemEventProperties properties)
		{
			base.ItemAdded(properties);
			SPListItem listItem = properties.ListItem;
			SPWeb web = properties.Web;
			if (listItem.GetFieldValue(RFCFields.KeType.InternalName) == "W")
			{
				SPFieldLookupValue flv = listItem.GetFieldValueLookup(RFCFields.KeToKeLink.InternalName);
				if(flv != null)
					CreateDependentListItems(web,flv.LookupId, properties.ListItem,properties.List);
			}
		}

		private void CreateDependentListItems(SPWeb web, int lookupId, SPListItem item, SPList list)
		{
			SPList dependList = web.GetListExt(RFCLists.KeEffectList.CustomUrl);
			SPQuery query = new SPQuery();
			query.Query = string.Format("<Where><Eq><FieldRef Name='KeParent' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq></Where>",lookupId);
			SPListItemCollection dependListItems = dependList.GetItems(query);
			foreach(SPListItem it in dependListItems)
			{
				SPListItem dIt = list.AddItem();
				dIt[RFCFields.RfcToKeLink.InternalName] = item[RFCFields.RfcToKeLink.InternalName];
                dIt[RFCFields.KeToKeLink.InternalName] = it.GetFieldValueLookup(RFCFields.KeChildLink.InternalName) ;
				dIt[RFCFields.InteraptionFlag.InternalName] = item[RFCFields.InteraptionFlag.InternalName];
				dIt[RFCFields.KeType.InternalName] = 'I';
				dIt[BuiltInFieldId.Title] = item.Title + '-' + it.Title;
				dIt.Update();
			}
		}

		public override void ItemDeleting(SPItemEventProperties properties)
		{
			base.ItemDeleting(properties);
			SPListItem listItem = properties.ListItem;
			SPWeb web = properties.Web;
			if (listItem.GetFieldValue(RFCFields.KeType.InternalName) == "W")
			{
				SPFieldLookupValue flv = listItem.GetFieldValueLookup(RFCFields.KeToKeLink.InternalName);
				if (flv != null)
					DeleteDependentListItems(web, flv.LookupId, properties.ListItem, properties.List);
			}

		}

		private void DeleteDependentListItems(SPWeb web, int lookupId, SPListItem listItem, SPList list)
		{
			SPList dependList = web.GetListExt(RFCLists.KeEffectList.CustomUrl);
			SPQuery query = new SPQuery();
			query.Query = string.Format("<Where><Eq><FieldRef Name='KeParent' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq></Where>", lookupId);
			SPListItemCollection dependListItems = dependList.GetItems(query);
			StringBuilder sb = new StringBuilder();
			foreach (SPListItem it in dependListItems)
			{
				sb.AppendFormat("<Value Type='Integer'>{0}</Value>", it.GetFieldValueLookup(RFCFields.KeChildLink.InternalName).LookupId);
			}
			
			SPQuery deleteQuery = new SPQuery();
			deleteQuery.Query=string.Format("<Where><And><Eq><FieldRef Name='RFCKeType' /><Value Type='Text'>I</Value></Eq><And><Eq><FieldRef Name='RFCKeLink' LookupId='True' /><Value Type='Integer'>{0}</Value></Eq><In><FieldRef Name='KeKeLink' LookupId='True' /><Values>{1}</Values></In></And></And></Where>",lookupId, sb.ToString());
			SPListItemCollection deleteDedendListItems = list.GetItems(deleteQuery);
			foreach(SPListItem delItem in deleteDedendListItems)
			{
				delItem.Delete();
			}
		}

		public static List<EventReceiverDefinition> Receiver = new List<EventReceiverDefinition> {
			new EventReceiverDefinition
					{
						Name = "RedSys.RFC.Data.Code.RFCKeEventReceiver.Added",
						Synchronization = BuiltInEventReceiverSynchronization.Synchronous,
						 Type = BuiltInEventReceiverType.ItemAdded,
						 Assembly =  Assembly.GetExecutingAssembly().FullName,
						 Class = "RedSys.RFC.Data.Code.RFCKeEventReceiver",
						 SequenceNumber = 500
					},
			new EventReceiverDefinition
			{
						Name = "RedSys.RFC.Data.Code.RFCKeEventReceiver.Deleting",
						Synchronization = BuiltInEventReceiverSynchronization.Synchronous,
						 Type = BuiltInEventReceiverType.ItemDeleting,
						 Assembly =  Assembly.GetExecutingAssembly().FullName,
						 Class = "RedSys.RFC.Data.Code.RFCKeEventReceiver",
						 SequenceNumber = 500
					}
			};

	
	}
}
