using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.AuditManager;
using Utilities.AuthorizationManager;

namespace Service
{
    public class AuditHelper
    {
        public static void AuthorizationSuccess(IPrincipal principal, string serviceName)
        {
            string userName = Formatter.ParseName(((CustomPrincipal)principal).Identity.Name);
            userName = userName.Split(',')[0].Split('=')[1];
            try
            {
                Audit.AuthorizationSuccess(userName, serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void AuthorizationFailure(IPrincipal principal, string serviceName, string reason)
        {
            string userName = Formatter.ParseName(((CustomPrincipal)principal).Identity.Name);
            userName = userName.Split(',')[0].Split('=')[1];
            try
            {
                Audit.AuthorizationFailure(userName, serviceName, reason);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ExecutionSuccess(IPrincipal principal, string serviceName)
        {
            string userName = Formatter.ParseName(((CustomPrincipal)principal).Identity.Name);
            userName = userName.Split(',')[0].Split('=')[1];
            try
            {
                Audit.ExecutionSuccess(userName, serviceName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ExecutionFailure(IPrincipal principal, string serviceName, string reason)
        {
            string userName = Formatter.ParseName(((CustomPrincipal)principal).Identity.Name);
            userName = userName.Split(',')[0].Split('=')[1];
            try
            {
                Audit.ExecutionFailure(userName, serviceName, reason);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void Replication(IPrincipal principal)
        {
            string userName = Formatter.ParseName(principal.Identity.Name);
            try
            {
                Audit.Replication();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
