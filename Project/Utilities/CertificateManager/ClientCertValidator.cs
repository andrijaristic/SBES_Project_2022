using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.CertificateManager
{
    public class ClientCertValidator : X509CertificateValidator
    {
        string companyName = "CN=TestCA";
        public override void Validate(X509Certificate2 certificate)
        {
            if (!certificate.Issuer.Equals(companyName))
            {
                throw new Exception("Certificate is self-issued.");
            }
        }
    }
}
