using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Xml;

namespace RedSys.Common.Workflow
{
    public class ItemWorkflows
    {
        public List<Workflow> AllItemWorkflows { get { return AllWFs; } }
        public Workflow CurrentWorkflow { get { return AllWFs[CurrentWFIndex]; } }
        public int CurrentWorkflowIndex { get { return CurrentWFIndex; } }
        
        protected List<Workflow> AllWFs;
        protected int CurrentWFIndex;
        protected SPListItem CurItem;

        public ItemWorkflows (SPListItem item)
        {
            AllWFs = new List<Workflow>();
            CurItem = item;
            if (CurItem["WFData"] != null && CurItem["WFData"].ToString() != "")
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(CurItem["WFData"].ToString());

                XmlElement wfs = doc["Workflows"];
                int i = 0;
                foreach (XmlElement cn in wfs.ChildNodes)
                {
                    Workflow wf = new Workflow(CurItem, false);
                    wf.LoadData(doc, cn);
                    if (!bool.Parse(cn.Attributes["InProgress"].Value))
                        CurrentWFIndex = i;
                    i++;
                    AllWFs.Add(wf);
                }
            }
        }
    }
}
