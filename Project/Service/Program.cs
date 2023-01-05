using Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.AuthorizationManager;
using Utilities.CertificateManager;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            string type = string.Empty;
            type = (string)ServiceConfig.ResourceManager.GetObject("Type");
            type = type == null ? "Primary" : type;
            bool serviceIsPrimary = String.Equals(type, "Primary");
            ServiceHost host = null;
            if (serviceIsPrimary)
            {
                string srvCertCN = (string)ServiceConfig.ResourceManager.GetObject("Certificate");

                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

                string address = "net.tcp://localhost:9000/Service";
                host = new ServiceHost(typeof(DatabaseManagementService));
                host.AddServiceEndpoint(typeof(IDatabaseManagement), binding, address);

                ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
                host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
                host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

                ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
                host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

                ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
                host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

                // podesavamo custom polisu, odnosno nas objekat principala
                host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
                List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
                policies.Add(new CustomAuthorizationPolicy());
                host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

                // Podesavanje Audit Behaviour-a
                ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
                newAudit.AuditLogLocation = AuditLogLocation.Application;
                newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

                host.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
                host.Description.Behaviors.Add(newAudit);
            }
            else
            {
                // set up host for replicator service
            }

            try
            {
                string serviceType = serviceIsPrimary ? "Client" : "Replicator";
                host.Open();
                Console.WriteLine(serviceType + " service started working...");
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
