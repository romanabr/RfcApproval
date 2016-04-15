using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Helper
{
    public static class Security
    {
        public static void AddUsers(this SPGroup group, List<SPUser> users)
        {
            foreach (SPUser user in users)
            {
                group.AddUser(user);
            }
            group.Update();
        }

        public static void RemoveUsers(this SPGroup group, List<SPUser> users)
        {
            if (users.Count > 0)
            {
                foreach (SPUser user in users)
                {
                    group.RemoveUser(user);
                }
                group.Update();
            }
        }

        public static void ClearUsers(this SPGroup group)
        {
            if (group == null || group.Users.Count == 0) return;
            foreach (SPUser user in group.Users)
            {
                group.RemoveUser(user);
            }
            group.Update();
        }

        public static bool InGroup(this SPUser user, SPGroup group)
        {
            return user.Groups.Cast<SPGroup>()
              .Any(g => g.ID == group.ID);
        }


        public static void SetUsers(this SPGroup group, List<SPUser> users)
        {
            if (users.Count == 0)
                group.ClearUsers();
            else
            {
                if (group.Users.Count == 0)
                {
                    group.AddUsers(users);
                }
                else
                {
                    List<SPUser> groupUser = new List<SPUser>();
                    List<SPUser> addUsers = new List<SPUser>();
                    foreach (SPUser user in group.Users)
                    {
                        groupUser.Add(user);
                    }

                    foreach (SPUser user in users)
                    {
                        if (user.InGroup(group))
                        {
                            SPUser removeUser = groupUser.Find(u => u.ID == user.ID);
                            if (removeUser != null) groupUser.Remove(removeUser);
                        }
                        else
                        {
                            addUsers.Add(user);
                        }
                    }
                    group.AddUsers(addUsers);
                    group.RemoveUsers(groupUser);
                }
            }


        }

        public static List<SPUser> GetListSPUser(this SPFieldUserValueCollection item)
        {
            List<SPUser> returnList = new List<SPUser>();
            foreach (SPFieldUserValue uv in item)
            {
                if (uv.User != null)
                {
                    SPUser user = uv.User;
                    returnList.Add(user);
                }
                else
                {

                }
                // Process user
            }
            return returnList;
        }

        public static List<SPUser> GetListSPUser(this SPUserCollection item)
        {
            List<SPUser> returnList = new List<SPUser>();
            foreach (SPUser uv in item)
            {
                if (uv != null)
                {
                    returnList.Add(uv);
                }
                else
                {

                }
                // Process user
            }
            return returnList;
        }



        public static SPGroup CreateGroup(this SPWeb rootWeb, string title)
        {
            SPGroup group = null;

            // Check if the group exists
            try
            {
                group = rootWeb.SiteGroups[title];
            }
            catch { }

            // If it doesn't, add it
            if (group == null)
            {
                rootWeb.SiteGroups.Add(title, rootWeb.Author, rootWeb.Author, "Office group");
                group = rootWeb.SiteGroups[title];

                // Add the group's permissions
                SPRoleDefinition roleDefinition = rootWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                SPRoleAssignment roleAssignment = new SPRoleAssignment(group);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                rootWeb.RoleAssignments.Add(roleAssignment);
                rootWeb.Update();
            }
            return group;
        }

        public static SPGroup ChangeGroupName(this SPWeb rootWeb, string beforeTitle, string afterTitle)
        {
            SPGroup group = null;

            // Check if the group exists
            try
            {
                group = rootWeb.SiteGroups[beforeTitle];
            }
            catch { }

            // If it doesn't, add it
            if (group == null)
            {
                rootWeb.CreateGroup(afterTitle);
            }
            else
            {
                group.Name = afterTitle;
                group.Update();
            }
            return group;
        }

        public static void AssignPermissionsToItem(this SPListItem item, SPPrincipal obj, SPRoleType roleType)
        {
            if (obj == null) return;
            if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(false, true);
            }

            SPRoleAssignment roleAssignment = new SPRoleAssignment(obj);
            SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            item.RoleAssignments.Add(roleAssignment);
        }

        public static void AssignPermissionsToItem(this SPListItem item, List<SPUser> objs, SPRoleType roleType)
        {
            if (objs == null || objs.Count == 0) return;
            if (!item.HasUniqueRoleAssignments)
            {
                item.BreakRoleInheritance(false, true);
            }
            foreach (var obj in objs)
            {
                SPRoleAssignment roleAssignment = new SPRoleAssignment(obj);
                SPRoleDefinition roleDefinition = item.Web.RoleDefinitions.GetByType(roleType);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

                item.RoleAssignments.Add(roleAssignment);
            }
        }

        public static void AssignPermissionsToList(this SPList list, SPPrincipal obj, SPRoleType roleType)
        {
            if (!list.HasUniqueRoleAssignments)
            {
                list.BreakRoleInheritance(true, false);
            }

            SPRoleAssignment roleAssignment = new SPRoleAssignment(obj);
            SPRoleDefinition roleDefinition = list.ParentWeb.RoleDefinitions.GetByType(roleType);
            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            list.RoleAssignments.Add(roleAssignment);
            list.Update();

        }
    }
}
