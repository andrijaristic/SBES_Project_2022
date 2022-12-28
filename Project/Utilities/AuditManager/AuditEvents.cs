using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.AuditManager
{

    public enum AuditEventTypes
    {
        AuthenticationSuccess = 0,
        AuthorizationSuccess = 1,
        AuthorizationFailure = 2,
        ExecutionSuccess = 3,
        ExecutionFailure = 4,
    }
    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock(resourceLock) 
                {
                    if (resourceManager == null) 
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
                    }

                    return resourceManager;
                }
            }
        }

        public static string AuthenticationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthenticationSuccess.ToString());
            }
        }

        public static string AuthorizationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthorizationSuccess.ToString());
            }
        }

        public static string AuthorizationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AuthorizationFailure.ToString());
            }
        }

        public static string ExecutionSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ExecutionSuccess.ToString());
            }
        }

        public static string ExecutionFailure
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.ExecutionFailure.ToString());
            }
        }
    }
}
