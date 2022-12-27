using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.CertificateManager;

namespace Client
{
    public class ClientProxy : ChannelFactory<IDatabaseManagement>, IDatabaseManagement, IDisposable
    {
        IDatabaseManagement factory;

        public ClientProxy(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            cltCertCN = "wcfwriter";    // zakomentarisi ako pokreces sa naloga koje se zove wcfreader, wcfwriter, ili wcfadmin
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }
        public void AddConsumer(string databaseName, string region, string city, int year, double amount)
        {
            try
            {
                factory.AddConsumer(databaseName, region, city, year, amount);
                Console.WriteLine("New consumer added successfully");
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public void ArchiveDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public double AverageConsumptionForCity(string databaseName, string city)
        {
            throw new NotImplementedException();
        }

        public double AverageConsumptionForRegion(string databaseName, string region)
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void DeleteDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void EditConsumer(string databaseName, string region, string city, int year, double amount)
        {
            try
            {
                factory.EditConsumer(databaseName, region, city, year, amount);
                Console.WriteLine("Consumer edited successfully");
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }

        public string MaxConsumerForRegion(string databaseName, string region)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
    }
}
