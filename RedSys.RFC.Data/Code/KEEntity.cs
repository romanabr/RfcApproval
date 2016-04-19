using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.Lists;

namespace RedSys.RFC.Data.Code
{
    public class KEEntity
    {
        private SPWeb currentWeb;
        private SPList currentList;
        private SPListItem currentListItem;

        public KEEntity(SPWeb web, int id)
        {
            currentWeb = web;
            currentList = currentWeb.GetListExt(RFCLists.RfcKeList.CustomUrl);
            currentListItem = currentList.GetItemById(id);
            if (currentListItem != null)
                FillEntity(currentListItem);
        }

        public KEEntity(SPListItem keItem)
        {
            currentWeb = keItem.Web;
            currentList = currentWeb.GetListExt(RFCLists.RfcKeList.CustomUrl);
            currentListItem = keItem;
            if (currentListItem != null)
                FillEntity(currentListItem);
        }


        public static int GetKEEntities(SPListItem parentListItem)
        {
            List<KEEntity> retEntities = new List<KEEntity>();
            SPList currentList = parentListItem.Web.GetListExt(RFCLists.RfcKeList.CustomUrl);
            SPQuery query = new SPQuery();
            query.Query = $"<Where><Eq><FieldRef Name=\"RFCKeLink\" LookupId=\"True\" /><Value Type=\"Integer\">{parentListItem.ID}</Value></Eq></Where>";
            SPListItemCollection licCollection = currentList.GetItems(query);
            if (licCollection != null && licCollection.Count > 0)
            {
                foreach (SPListItem keentity in licCollection)
                {
                    retEntities.Add(new KEEntity(keentity));
                }

            }
            return retEntities.Count;
        }

        private void FillEntity(SPListItem spListItem)
        {
            RFCID = spListItem.GetFieldValueLookup(RFCFields.RfcToKeLink.InternalName).LookupId;
            KEID = spListItem.GetFieldValueLookup(RFCFields.KeToKeLink.InternalName).LookupId;
            Flag = spListItem.GetFieldValueBoolean(RFCFields.InteraptionFlag.InternalName);
            Type = spListItem.GetFieldValue(RFCFields.KeType.InternalName);
        }

        public int RFCID;
        public int KEID;
        public bool Flag;
        public string Type;
    }
}

