using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.Const;
using RedSys.RFC.Data.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Code
{
    public class RFCEntity
    {
        private SPListItem currentItem;
        private SPWeb currentWeb;
        private RFCTasks tasks;
        public RFCTasks Tasks
        {
            get { if (tasks == null)
                    tasks = new RFCTasks(currentWeb,currentItem);
                return tasks;
            }
        }

        public RFCEntity(SPListItem item)
        {
            currentItem = item;
            currentWeb = item.Web;
            tasks = new RFCTasks(currentWeb, currentItem);
        }

        public int GetTaskCount()
        {
            return tasks.Count();
        }

        public void SetDocSetPermissionWorklow()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(currentItem.Web.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.GetListExt(currentItem.ParentList.RootFolder.Url);
                        SPListItem item = list.GetItemById(currentItem.ID);


                        item.ResetRoleInheritance();
                        item.BreakRoleInheritance(false, true);

                        SPUser manager = item.GetFieldValueUser(RFCFields.Manager.InternalName);
                        SPUser author = item.GetFieldValueUser(SPBuiltInFieldId.Author);
                        List<SPUser> users = RFCTasks.GetKEUsers(item);
                        SPGroup ownerGroup = web.AssociatedOwnerGroup;
                        SPGroup readGroup = web.AssociatedVisitorGroup;

                        item.AssignPermissionsToItem(ownerGroup, SPRoleType.Reader);
                        item.AssignPermissionsToItem(readGroup, SPRoleType.Reader);
                        item.AssignPermissionsToItem(manager, SPRoleType.Editor);
                        item.AssignPermissionsToItem(author, SPRoleType.Editor);
                        item.AssignPermissionsToItem(users, SPRoleType.Editor);
                    }
                }
            });
        }

        public  void SetDocSetPermissionMain()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(currentWeb.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.GetListExt(currentItem.ParentList.RootFolder.Url);
                        SPListItem item = list.GetItemById(currentItem.ID);

                        item.ResetRoleInheritance();
                        item.BreakRoleInheritance(false, true);

                        SPUser manager = item.GetFieldValueUser(RFCFields.Manager.InternalName);
                        SPUser author = item.GetFieldValueUser(SPBuiltInFieldId.Author);
                        SPGroup ownerGroup = web.AssociatedOwnerGroup;
                        SPGroup readGroup = web.AssociatedVisitorGroup;
                        
                        item.AssignPermissionsToItem(ownerGroup, SPRoleType.Administrator);
                        item.AssignPermissionsToItem(readGroup, SPRoleType.Reader);
                        item.AssignPermissionsToItem(manager, SPRoleType.Editor);
                        item.AssignPermissionsToItem(author, SPRoleType.Editor);
                    }
                }
            });
        }

    }
}
