using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;

namespace RedSys.Common.Workflow
{

    public class WFData
    {
        protected static List<Step> GetStepList(SPListItemCollection stageCol)
        {
            var lst = new List<Step>();
            try
            {
                foreach (SPListItem item in stageCol)
                {
                    var data = new Step(item);

                    lst.Add(data);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lst;
        }

        public static List<Step> GetStagesByDocType(SPWeb web, SPListItem item)
        {
            string docType = item.ContentType.Name;
            SPList stageList = web.Lists[Constant.StageListName];
            SPQuery query = new SPQuery();

            query.Query = string.Format("<Where><Eq><FieldRef ID='{0}' /><Value Type='Text'>{1}</Value></Eq></Where>", stageList.Fields["Тип документа"].Id, docType);
            
            SPListItemCollection stageCol = null;
            var lst = new List<Step>();
            try
            {
                stageCol = stageList.GetItems(query);
                if (stageCol != null && stageCol.Count != 0)
                {
                    foreach (SPListItem items in stageCol)
                    {
                        var data = new Step(items);

                        lst.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lst.OrderBy(c => c.StageNumber).ToList<Step>();
        }

        public static List<Step> GetStages(SPWeb web, SPListItem item)
        {

            string docType = item.ContentType.Name;
            SPList stageList = web.Lists[Constant.StageListName];
            SPFieldLookupValue kindNameLookup = item.GetFieldValueLookup(Constant.DocumentKind);
            int docKind = 0;
            if (kindNameLookup != null)
            {
                docKind = kindNameLookup.LookupId;
            }

            SPQuery query = new SPQuery();
            if (docKind != 0)
            {
                query.Query = string.Format(@"<Where><And><Eq><FieldRef ID='{0}' /><Value Type='Text'>{1}</Value></Eq><Eq><FieldRef ID='{2}' LookupId='TRUE' /><Value Type='Lookup'>{3}</Value></Eq></And></Where>", 
                    stageList.Fields["Тип документа"].Id, docType, 
                    stageList.Fields[Constant.DocumentKind].Id, docKind);
            }
            else
            {
                query.Query = string.Format("<Where><Eq><FieldRef ID='{0}' /><Value Type='Text'>{1}</Value></Eq></Where>", 
                    stageList.Fields["Тип документа"].Id, docType);
            }


            SPListItemCollection stageCol = null;
            var lst = new List<Step>();
            try
            {
                stageCol = stageList.GetItems(query);
                if (stageCol != null && stageCol.Count != 0)
                {
                    foreach (SPListItem items in stageCol)
                    {
                        var data = new Step(items);

                        lst.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (lst.Count > 0)
                return lst.OrderBy(c => c.StageNumber).ToList<Step>();
            else
                return GetStagesByDocType(web, item);
        }

        public static Step GetStepByNumber(SPWeb web, SPListItem item, int stepNumber)
        {
            var lst = new List<Step>();
            lst = GetStages(web, item);
            var data = (from c in lst
                        where c.StageNumber == stepNumber
                        select c).FirstOrDefault();
            if (data == null)
            {
                lst = GetStagesByDocType(web, item);
                data = (from c in lst
                        where c.StageNumber == stepNumber
                        select c).FirstOrDefault();
            }
            return data;
        }

        public WFData()
        {
        }
    }
}
