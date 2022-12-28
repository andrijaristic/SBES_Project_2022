using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void AutorizeLog(string serviceName)
        {
            string userName = Formatter.ParseName(((CustomPrincipal)Thread.CurrentPrincipal).Identity.Name);
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
    }
}
