using Microsoft.SharePoint;
using System;

namespace RedSys.RFC.Core.Helper
{
	public class EventReceiverManager : SPEventReceiverBase, IDisposable
	{
		public EventReceiverManager(bool disableImmediately)
		{
			EventFiringEnabled = !disableImmediately;
		}

		public void StopEventReceiver()
		{
			EventFiringEnabled = false;
		}
		public void StartEventReceiver()
		{
			EventFiringEnabled = true;
		}

		public void Dispose()
		{
			EventFiringEnabled = true;
		}
	}
}
