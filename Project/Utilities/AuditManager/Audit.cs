using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.AuditManager
{
    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "AuditManager.Audit";
        const string LogName = "SBESProjekatLogs";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            } catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void AuthenticationSuccess(string userName)
        {
            if (customLog != null)
            {
                string userAuthSuccess = AuditEvents.AuthenticationSuccess;
                string message = string.Format(userAuthSuccess, userName);
                customLog.WriteEntry(message);
            } else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthenticationSuccess));
            }
        }

        public static void AuthorizationSuccess(string userName, string serviceName)
        {
            if (customLog != null)
            {
                string userAuthorizationSuccess = AuditEvents.AuthorizationSuccess;
                string message = string.Format(userAuthorizationSuccess, userName, serviceName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationSuccess));
            }
        }

        public static void AuthorizationFailure(string userName, string serviceName, string reason)
        {
            if (customLog != null)
            {
                string userAuthorizationFailure = AuditEvents.AuthorizationFailed;
                string message = string.Format(userAuthorizationFailure, userName, serviceName, reason);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.AuthorizationFailure));
            }
        }

        public static void ExecutionSuccess(string userName, string serviceName)
        {
            if (customLog != null)
            {
                string execSuccess = AuditEvents.ExecutionSuccess;
                string message = string.Format(execSuccess, userName, serviceName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ExecutionSuccess));
            }
        }

        public static void ExecutionFailure(string userName, string serviceName, string reason)
        {
            if (customLog != null)
            {
                string execFailure = AuditEvents.ExecutionFailure;
                string message = string.Format(execFailure, userName, serviceName, reason);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.",
                    (int)AuditEventTypes.ExecutionFailure));
            }
        }

        public void Dispose()
        {
            if (customLog != null) 
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
