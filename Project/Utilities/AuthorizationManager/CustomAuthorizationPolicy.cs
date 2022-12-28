using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Utilities.AuditManager;

namespace Utilities.AuthorizationManager
{
    public class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        public CustomAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }
        public string Id
        {
            get;
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            if (!evaluationContext.Properties.TryGetValue("Identities", out object list))
            {
                return false;
            }

            IList<IIdentity> identities = list as IList<IIdentity>;
            if (list == null || identities.Count <= 0)
            {
                return false;
            }

            /// windowsIdentity je null ovde. Nemam pojma sto, sve prolazi lepo cak i kad je
            /// odkomentarisano, samo javlja na Service da nema reference ali cu zakomentarisati za svaki slucaj
            /// 
            //WindowsIdentity windowsIdentity = identities[0] as WindowsIdentity;
            //try
            //{
            //    Audit.AuthenticationSuccess(Formatter.ParseName(windowsIdentity.Name));

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            evaluationContext.Properties["Principal"] =
                new CustomPrincipal(identities[0]);
            return true;
        }
    }
}
