using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
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

            Type x509IdentityType = identities[0].GetType();
            FieldInfo certificateField = x509IdentityType.GetField("certificate", BindingFlags.Instance | BindingFlags.NonPublic);
            var cert = (X509Certificate2)certificateField.GetValue(identities[0]);
            string identity = (cert.Subject.Split(',')[0].Split('=')[1].Split('_'))[0];

            try
            {
                Audit.AuthenticationSuccess(Formatter.ParseName(identity));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            evaluationContext.Properties["Principal"] =
                new CustomPrincipal(identities[0]);
            return true;
        }
    }
}
