using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.CertificateManager
{
    public class ServiceCertValidator : X509CertificateValidator
    {
        public override void Validate(X509Certificate2 certificate)
        {
            // CN=wcfadmin,OU=Readers_Writers_Admins,O=TestCA
            string companyName;
            try
            {
                companyName = certificate.Subject.Split(',')[2].Split('=')[1];
                companyName = "CN=" + companyName;
            }
            catch
            {
                throw new Exception("Certificate is not from the valid issuer.");
            }

            if (!String.Equals(companyName, certificate.Issuer))
            {
                throw new Exception("Certificate is not from the valid issuer.");
            }
        }
    }
}
