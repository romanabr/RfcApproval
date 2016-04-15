using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Reflection;
using RedSys.RFC.Core.Helper;

namespace PSE.CustomUpload.Features.UrlRedirectFeatureWEBAPP
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e5984593-84a2-4139-95d9-9cc0c8e9a18c")]
    public class UrlRedirectFeatureWEBAPPEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
                SPWebConfigModification modification = new SPWebConfigModification("add[@name='URLRedirectHttpModule']", "configuration/system.webServer/modules");
                modification.Sequence = 0;
                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
                modification.Value = string.Format(@"<add name=""URLRedirectHttpModule"" type=""PSE.CustomUpload.URLRedirectHttpModule, {0}"" />", Assembly.GetExecutingAssembly().FullName);

                webApp.WebConfigModifications.Add(modification);
                webApp.Update();

                webApp.WebService.ApplyWebConfigModifications();
            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex);
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
                SPWebConfigModification modification = new SPWebConfigModification("add[@name='URLRedirectHttpModule']", "configuration/system.webServer/modules");
                modification.Sequence = 0;
                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
                modification.Value = string.Format(@"<add name=""URLRedirectHttpModule"" type=""PSE.CustomUpload.URLRedirectHttpModule, {0}"" />", Assembly.GetExecutingAssembly().FullName);

                webApp.WebConfigModifications.Remove(modification);
                webApp.Update();

                webApp.WebService.ApplyWebConfigModifications();

            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex);
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
