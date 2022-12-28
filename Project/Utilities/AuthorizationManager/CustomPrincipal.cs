using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Utilities.CertificateManager;

namespace Utilities.AuthorizationManager
{
    public class CustomPrincipal : IPrincipal
    {
        IIdentity identity = null;
        public CustomPrincipal(IIdentity identity)
        {
            this.identity = identity;
        }

        public IIdentity Identity
        {
            get { return identity as IIdentity; }
        }

        public bool IsInRole(string permission)
        {
            try
            {
                Type x509IdentityType = identity.GetType();
                FieldInfo certificateField = x509IdentityType.GetField("certificate", BindingFlags.Instance | BindingFlags.NonPublic);
                var cert = (X509Certificate2)certificateField.GetValue(identity);
                // CN=wcfadmin,OU=Readers_Writers_Admins,O=TestCA
                string[] groups = cert.Subject.Split(',')[1].Split('=')[1].Split('_');
                foreach (var groupName in groups)
                {
                    string[] permissions;
                    if (RolesConfig.GetPermissions(groupName, out permissions))
                    {
                        if (permissions.Contains(permission))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }

        }
    }
}
