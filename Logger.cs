// Copyright © Uwe Gradenegger <uwe@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.ServiceModel;
using System.Web;

namespace TameMyCerts.WSTEP
{
    /// <summary>
    ///     A simple class to write to the Windows Event log.
    /// </summary>
    internal class Logger
    {
        private readonly EventLog _eventLog;

        public Logger()
        {
            const string logName = "Application";

            var assembly = Assembly.GetExecutingAssembly();

            _eventLog = new EventLog(logName)
            {
                Source = CreateEventSource(
                    ((AssemblyTitleAttribute)assembly.GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title,
                    logName)
            };
        }

        public void Log(Exception ex)
        {
            if (ex is FaultException)
            {
                _eventLog.WriteEntry(
                    string.Format(LocalizedStrings.ERR_REQUEST_DENIED, ex.Message,
                        HttpContext.Current.User.Identity.Name, GetClientIpAddress(),
                        HttpContext.Current.Request.UserAgent,
                        HttpContext.Current.Request.Url.AbsoluteUri, HttpContext.Current.Request.HttpMethod),
                    EventLogEntryType.Warning, 2);
            }
            else
            {
                _eventLog.WriteEntry(
                    string.Format(LocalizedStrings.ERR_UNHANDLED_EXCEPTION, ex.Message,
                        HttpContext.Current.User.Identity.Name, GetClientIpAddress(),
                        HttpContext.Current.Request.UserAgent,
                        HttpContext.Current.Request.Url.AbsoluteUri, HttpContext.Current.Request.HttpMethod, ex),
                    EventLogEntryType.Error, 1);
            }
        }

        private static string CreateEventSource(string currentAppName, string logName)
        {
            var eventSource = currentAppName;

            try
            {
                if (!EventLog.SourceExists(eventSource))
                {
                    EventLog.CreateEventSource(eventSource, logName);
                }
            }
            catch (SecurityException)
            {
                eventSource = "Application";
            }

            return eventSource;
        }

        private static string GetClientIpAddress()
        {
            var ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ipAddress;
        }
    }
}