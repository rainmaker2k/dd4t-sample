﻿// -----------------------------------------------------------------------
// <copyright file="BinaryDistributionModule.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using DD4T.Utils;
using Microsoft.Practices.Unity;

namespace DD4T.Web.Mvc.Modules
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Configuration;
    using DD4T.Web.Mvc.Unity;


    /// <summary>
    /// HttpModule intercepting a request to a static resource, caches the resource to the file-system from the Broker DB
    /// </summary>
    public class BinaryDistributionModule : IHttpModule
    {
        #region IHttpModule

        /// <summary>
        /// Initialize this module. Attach the worker method to the BeginRequest event.
        /// </summary>
        /// <param name="application">Current HttpApplication</param>
        public void Init(HttpApplication application)
        {
            application.PreRequestHandlerExecute += new EventHandler(DistributionModule_OnPreRequestHandlerExecute);
            application.BeginRequest += new EventHandler(DistributionModule_OnBeginRequest);
        }

        /// <summary>
        /// Main method handling requests to the specified resource.
        /// </summary>
        /// <param name="o">Current HttpApplication</param>
        /// <param name="eventArgs">Current event arguments</param>
        public void DistributionModule_OnPreRequestHandlerExecute(object o, EventArgs eventArgs)
        {
            DateTime start = DateTime.Now;
            HttpApplication application = (HttpApplication) o;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string urlPath = request.Url.AbsolutePath;

            Regex binaryRegex = new Regex(ConfigurationManager.AppSettings["BinaryUrlPattern"]);
            if (!binaryRegex.IsMatch(urlPath))
            {
                LoggerService.Debug("url {0} does not match binary url pattern, ignoring it", urlPath);
                return;
            }

            if (!BinaryFileManager.ProcessRequest(request))
            {
                LoggerService.Debug("Url {0} not found. Returning 404 Not Found.", urlPath);
                //the 404 should be handled by the default handler
                return;
            }
            // if we got here, the file was successfully created on file-system
            DateTime ifModifiedSince = Convert.ToDateTime(request.Headers["If-Modified-Since"]);
            LoggerService.Debug("If-Modified-Since: " + ifModifiedSince);

            DateTime fileLastModified = File.GetLastWriteTime(request.PhysicalPath);
            LoggerService.Debug("File last modified: " + fileLastModified);

            if (fileLastModified.Subtract(ifModifiedSince).TotalSeconds < 1)
            {
                LoggerService.Debug("Sending 304 Not Modified");
                response.StatusCode = 304;
                response.SuppressContent = true;
                application.CompleteRequest();
            }

            // Note: if the file was just created, an empty dummy might still be served by IIS
            // To make sure the right file is sent, we will transmit the file directly within the first second of the creation
            if (fileLastModified.AddSeconds(1).CompareTo(DateTime.Now) > 0)
            {
                LoggerService.Debug("file was created less than 1 second ago, transmitting content directly");
                response.Clear();
                response.TransmitFile(request.PhysicalPath);
            }
        }


        public static void DistributionModule_OnBeginRequest(Object source, EventArgs e)
        {
            try
            {
                IBinaryFileManager fileManager = BinaryFileManager;
            }
            catch
            {
                // this is just to force an error right after IIS has been reset
            }

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            string urlPath = request.Url.AbsolutePath;
            LoggerService.Information(">>DistributionModule_OnBeginRequest ({0})", urlPath);

            Regex binaryRegex = new Regex(ConfigurationManager.AppSettings["BinaryUrlPattern"]);
            if (!binaryRegex.IsMatch(urlPath))
            {
                LoggerService.Debug("url {0} does not match binary url pattern, ignoring it", urlPath);
                LoggerService.Information("<<DistributionModule_OnBeginRequest ({0})", urlPath);
                return;
            }

            string realPath = request.PhysicalApplicationPath + "BinaryData" + request.Path.Replace("/", "\\");
                // request.PhysicalPath;
            context.RewritePath("/BinaryData" + request.Path);

            if (!File.Exists(realPath))
            {
                LoggerService.Debug("Dir path: " + realPath.Substring(0, realPath.LastIndexOf("\\")));
                try
                {
                    string dir = realPath.Substring(0, realPath.LastIndexOf("\\"));

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    FileStream file = File.Create(realPath);
                    StreamWriter sw = new StreamWriter(file);
                    sw.Write("");
                    sw.Close();
                    file.Close();
                }
                catch (Exception exception)
                {
                    LoggerService.Information("IIS empty file could not be created." + exception.Message);
                }
            }

            LoggerService.Information("<<DistributionModule_OnBeginRequest ({0})", urlPath);
        }

        /// <summary>
        /// Do nothing
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region private

        private static IBinaryFileManager _binaryFileManager = null;

        private static IBinaryFileManager BinaryFileManager
        {
            get
            {
                if (_binaryFileManager == null)
                    _binaryFileManager = UnityHelper.Container.Resolve<IBinaryFileManager>();
                return _binaryFileManager;
            }
        }

        #endregion
    }
}
