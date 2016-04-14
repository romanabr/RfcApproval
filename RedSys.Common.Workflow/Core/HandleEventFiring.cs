using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{
    public class HandleEventFiring : SPItemEventReceiver
    {
        public void AccDisableEventFiring()
        {
            EventFiringEnabled = false;
        }

        public void AccEnableEventFiring()
        {
            EventFiringEnabled = true;
        }
    }
}
