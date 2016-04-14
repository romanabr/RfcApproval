
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Core.Helper
{
	public static class ExceptionHelper
	{
		const string _sourceName = "PSE Foundation";
		const string _logName = "Sharepoint log";

		public static void DUmpException(Exception ex, string alertMessage, System.Web.UI.Control contorl)
		{
			alertMessage = alertMessage.Replace("'", "\"");
			contorl.Controls.Add(new System.Web.UI.LiteralControl(
				$"<script>_spBodyOnLoadFunctionNames.push('CustomAlertError'); function CustomAlertError(){{alert('{alertMessage}');window.location.href = window.location.href;}}</script>"));
			if (ex != null)
				DUmpException(ex);
		}

		public static void DUmpException(Exception ex)
		{


			EventLog objEventLog = new EventLog();
			try
			{
				// поднимаем себе привилегии для записи в системный лог 
				WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
				if (!(EventLog.SourceExists(_sourceName)))
				{
					EventLog.CreateEventSource(_sourceName, _logName);
				}

				StringBuilder sb = new StringBuilder();
				Exception ex2 = ex;
				while (ex2 != null)
				{
					sb.AppendLine(ex2.ToString());
					ex2 = ex2.InnerException;
				}

				EventLog.WriteEntry(_sourceName, string.Format("Error: {0}\nStacktrace: {1}", sb, ex.StackTrace),
					EventLogEntryType.Error);

				// отменяем свои повышенные привилегии 
				wic.Undo();
			}
			catch (Exception)
			{
				// обработка прерывания ... 
			}
		}

		public static void DUmpMessage(string message)
		{
			EventLog objEventLog = new EventLog();
			try
			{
				// поднимаем себе привилегии для записи в системный лог 
				WindowsImpersonationContext wic = WindowsIdentity.Impersonate(IntPtr.Zero);
				if (!(EventLog.SourceExists(_sourceName)))
				{
					EventLog.CreateEventSource(_sourceName, _logName);
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine(message);
				EventLog.WriteEntry(_sourceName, sb.ToString(), EventLogEntryType.Information);

				// отменяем свои повышенные привилегии 
				wic.Undo();
			}
			catch (Exception)
			{
				// обработка прерывания ... 
			}
		}


		public static void DUmpExceptionWithJsDependentAndNoRedirect(Exception ex, string AlertMessage, System.Web.UI.Control contorl, string jsname)
		{
			contorl.Controls.Add(new System.Web.UI.LiteralControl("<script>_spBodyOnLoadFunctionNames.push('CustomAlertErrorNoRedirect' ,'" + jsname + "');" +
			"\n\r function CustomAlertErrorNoRedirect(){alert('" + AlertMessage + "');}</script>"));
			if (ex != null)
				DUmpException(ex);
		}

	}
}
