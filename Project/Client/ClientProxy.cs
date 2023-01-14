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
            //cltCertCN = "wcfwriter";    // zakomentarisi ako pokreces sa naloga koje se zove wcfreader, wcfwriter, ili wcfadmin
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }
        public void AddConsumer(string databaseName, string region, string city, int year)
        {
            try
            {
                factory.AddConsumer(databaseName, region, city, year);
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
            try
            {
                factory.ArchiveDatabase(databaseName);
                Console.WriteLine("Database archived successfully");
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

        public double AverageConsumptionForCity(string databaseName, string city)
        {

            try
            {
                double avg = factory.AverageConsumptionForCity(databaseName, city);
                if (avg != 0)
                    Console.WriteLine("Average consumption for city {0}: {1}", city, avg);
                else
                    Console.WriteLine("There are no records with that city in database.");
                return 0;
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return 0;
            }
        }

        public double AverageConsumptionForRegion(string databaseName, string region)
        {
            try
            {
                double avg = factory.AverageConsumptionForRegion(databaseName, region);
                if(avg!=0)
                    Console.WriteLine("Average consumption for region {0}: {1}", region, avg);
                else
                    Console.WriteLine("There are no records with that region in database.");
                return 0;
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return 0;
            }
        }

        public void CreateDatabase(string databaseName)
        {
            try
            {
                factory.CreateDatabase(databaseName);
                Console.WriteLine("Database created successfully");
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

        public void DeleteDatabase(string databaseName)
        {
            try
            {
                factory.DeleteDatabase(databaseName);
                Console.WriteLine("Database deleted successfully");
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
        /*public void DeleteConsumer(string databaseName, string region, string city, int year)
        {
            try
            {
                factory.DeleteConsumer(databaseName, region, city, year);
                Console.WriteLine("Consumer deleted successfully");
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
        }*/
        public void EditConsumer(string databaseName, string region, string city, int year, Months month, double amount)
        {
            try
            {
                factory.EditConsumer(databaseName, region, city, year, month, amount);
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
            try
            {
                string s = factory.MaxConsumerForRegion(databaseName,region);
                if(string.IsNullOrEmpty(s))
                    Console.WriteLine("There are no records with that region in database.");
                else Console.WriteLine(s);
                return "";
            }
            catch (FaultException<DatabaseException> e)
            {
                Console.WriteLine("Error: {0}", e.Detail.Message);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return "";
            }
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
