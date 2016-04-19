using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SPMeta2.Definitions;
using System.Reflection;
using RedSys.RFC.Data.ContentTypes;
using SPMeta2.Enumerations;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Core.Mail;
using RedSys.RFC.Data.Lists;
using RedSys.RFC.Data.Const;

namespace RedSys.RFC.Data.Code
{
	public class RFCEventReceiver: SPItemEventReceiver
	{
		public override void ItemAdded(SPItemEventProperties properties)
		{
			base.ItemAdded(properties);
			SPListItem listItem = properties.ListItem;
			SPWeb web = properties.Web;
			if (listItem.FileSystemObjectType == SPFileSystemObjectType.Folder && listItem.ContentType.Name != RFCContentType.RfcDocSet.Name)
			{
				

			}
			else if (listItem.FileSystemObjectType == SPFileSystemObjectType.File)
			{
				//MailSender.SendMail(listItem, MailType.UPLOADNEWFILEREPORTITEM);
			}
			else
			if (listItem.FileSystemObjectType == SPFileSystemObjectType.Folder && listItem.ContentType.Name == RFCContentType.RfcDocSet.Name)
			{
				listItem[BuiltInInternalFieldNames.Title] = "Запрос на изменение № "+ listItem.ID.ToString();
				listItem[BuiltInInternalFieldNames.Name] = "Запрос на изменение № " + listItem.ID.ToString();

				SPFieldLookupValue typeLookupValue = listItem.GetFieldValueLookup(RFCFields.Type.InternalName);
				SPList typeList = web.GetListExt(RFCLists.RFCManagerList.CustomUrl);

				SPListItem typeItem = typeList.GetItemById(typeLookupValue.LookupId);
				SPUser userManager = typeItem.GetFieldValueUser(RFCFields.Manager.InternalName);

				if (userManager != null)
				{
					listItem[RFCFields.Manager.InternalName] = new SPFieldUserValue(web, userManager.ID, userManager.Name);
				}
                using (EventReceiverManager erm = new EventReceiverManager(true))
                {
                    listItem.Update();
                }

                RFCEntity rfcEntity = new RFCEntity(listItem);
                rfcEntity.SetDocSetPermissionMain();

                MailGenerator newMailGenerator = new MailGenerator(listItem, MailType.NEWRFC);
				
				if (userManager != null)
				{
					newMailGenerator.To = new List<SPUser> { userManager };
				}
				else
				{
					newMailGenerator.To = new List<SPUser> { listItem.GetFieldValueUser(BuiltInInternalFieldNames.Author) };
				}
				
				newMailGenerator.SendMail();


                
			}
		}

		public override void ItemUpdated(SPItemEventProperties properties)
		{
			base.ItemUpdated(properties);

			SPListItem listItem = properties.ListItem;
			
		}

		public static List<EventReceiverDefinition> Receiver = new List<EventReceiverDefinition> {
			new EventReceiverDefinition
					{
						Name = "RedSys.RFC.Data.Code.RFCEventReceiver.Added",
						Synchronization = BuiltInEventReceiverSynchronization.Synchronous,
						 Type = BuiltInEventReceiverType.ItemAdded,
						 Assembly =  Assembly.GetExecutingAssembly().FullName,
						 Class = "RedSys.RFC.Data.Code.RFCEventReceiver",
						 SequenceNumber = 500
					},
			new EventReceiverDefinition
			{
						Name = "RedSys.RFC.Data.Code.RFCEventReceiver.Updated",
						Synchronization = BuiltInEventReceiverSynchronization.Synchronous,
						 Type = BuiltInEventReceiverType.ItemUpdated,
						 Assembly =  Assembly.GetExecutingAssembly().FullName,
						 Class = "RedSys.RFC.Data.Code.RFCEventReceiver",
						 SequenceNumber = 500
					}
			};

	}
}

