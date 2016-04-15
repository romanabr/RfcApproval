using System;
using System.Configuration;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using System.Web.Util;

using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using Newtonsoft.Json;

namespace PSE.CustomUpload
{
    public class MultipleUploadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        QueryString _mainQS;
        
        public void ProcessRequest(HttpContext context)
        {

            try
            {
                _mainQS = new QueryString(context.Request.Url.AbsoluteUri);
                var fc = context.Request.Files;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    var spurl = context.Request.Url.AbsoluteUri;
                    using (SPSite oSite = new SPSite(spurl))
                    {
                        using (SPWeb oWeb = oSite.OpenWeb())
                        {
                            //context.Response.Clear();
                            //context.Response.ContentEncoding = System.Text.Encoding.GetEncoding(1251);
                            //context.Response.Charset = "windows-1251";
                            //context.Response.Buffer = false;
                            //context.Response.ContentType = "text/json";

                            //// Clear the content of the response  
                            //context.Response.ClearContent();
                            //context.Response.ClearHeaders();


                            //// Buffer response so that page is sent  
                            //// after processing is complete.  
                            //context.Response.BufferOutput = true;
                        }
                    }
                });

                using (StringWriter tw = new StringWriter())
                {
                    var cps = JsonConvert.SerializeObject(fc[fc.AllKeys[0]].FileName);
                    tw.Write(cps);
                    tw.Write(Environment.NewLine);
                    var s = tw.ToString();
                    context.Response.AddHeader("Content-Length", context.Response.ContentEncoding.GetByteCount(s).ToString());
                    context.Response.Write(s);
                }
                context.Response.Flush();
                context.Response.Close();
                context.Response.End();
            }
            catch (ThreadAbortException ex)
            {
            }
            catch (HttpException ex)
            {
                // skip weird exception on context.Response.End();
                if (ex.InnerException != null && ex.InnerException.Message == "Value does not fall within the expected range.")
                {
                }
                else
                    ExceptionHelper.DUmpException(ex);
            }
            catch (Exception ex)
            {
                ExceptionHelper.DUmpException(ex);
            }
        }
    }
}
