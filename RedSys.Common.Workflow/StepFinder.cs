using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{
    public class StepFinder
    {
        public static Step GetNexStepPositiveExecuteCode(Step CurrentStep, SPWeb web, SPListItem item)
        {
            int step = CurrentStep.StepNextGood;
            Step tempStep = WFData.GetStepByNumber(web, item, step);
            return tempStep;
        }
        public static Step GetNexStepNegativeExecuteCode(Step CurrentStep, SPWeb web, SPListItem item)
        {
            int step = CurrentStep.StepNexBad;
            Step tempStep = WFData.GetStepByNumber(web, item, step);
            return tempStep;
        }
    }
}
