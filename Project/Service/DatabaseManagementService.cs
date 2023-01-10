using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Utilities;
using Utilities.CertificateManager;
using Utilities.Cryptography;

namespace Service
{
    public class DatabaseManagementService : IDatabaseManagement
    {
        private string serviceFolder = "ServiceData/";
       
        //[PrincipalPermission(SecurityAction.Demand, Role = "Add")]
        public void AddConsumer(string databaseName, string region, string city, int year)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Add", "AddConsumer"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call AddConsumer method without having Add permissions.");
            }

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            if (String.IsNullOrWhiteSpace(region))
            {
                string reason = "Region cannot be empty";
                AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(region));
            }

            if (String.IsNullOrWhiteSpace(city))
            {
                string reason = "City cannot be empty";
                AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if(!readingSuccessful)
            {
                string reason = "Database doesnt exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            foreach(Consumer consumer in consumers)
            {
                if(String.Equals(consumer.Region,region) && String.Equals(consumer.City, city) && consumer.Year == year)
                {
                    string reason = "Consumer with that region, city and year already exists";
                    AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }
            }
            Consumer c = new Consumer(region, city, year);
            consumers.Add(c);
            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
            AuditHelper.ExecutionSuccess(principal, "AddConsumer");

            using (ReplicatorProxy replicatorProxy = new ReplicatorProxy(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9001/ReplicatorService"))))
            {
                string eSecretKeyAes = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKeyAes, "AES/SecretKey.txt");

                byte[] encryptedData = null;
                string data = databaseName + ":" + region + ":" + city + ":" + year + ":" + c.Id;

                try
                {
                    encryptedData = AES_CBC_Algorithm.EncryptString(data, eSecretKeyAes);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encryption failed. Reason: {0}", e.Message);
                }

                //string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                string signCertCN = "wcfservice_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(encryptedData, certificateSign);

                replicatorProxy.AddConsumer(encryptedData, signature);
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Archive")]
        public void ArchiveDatabase(string databaseName)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Archive", "ArchiveDatabase"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call ArchiveDatabase method without having Archive permissions.");
            }

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Consumer with that region, city and year already exists";
                AuditHelper.ExecutionFailure(principal, "ArchiveDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "ArchiveDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            if (!DatabaseHelper.ArchiveDatabase(serviceFolder + databaseName + ".txt", consumers))
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "ArchiveDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            AuditHelper.ExecutionSuccess(principal, "ArchiveDatabase");

            using (ReplicatorProxy replicatorProxy = new ReplicatorProxy(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9001/ReplicatorService"))))
            {
                string eSecretKeyAes = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKeyAes, "AES/SecretKey.txt");

                byte[] encryptedData = null;

                try
                {
                    encryptedData = AES_CBC_Algorithm.EncryptString(databaseName, eSecretKeyAes);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encryption failed. Reason: {0}", e.Message);
                }

                //string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                string signCertCN = "wcfservice_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(encryptedData, certificateSign);

                replicatorProxy.ArchiveDatabase(encryptedData, signature);
            }
        }

        public double AverageConsumptionForCity(string databaseName, string city)
        {
            // ako je databaseName prazan string baci exception
            // iz baze ucita sve entitete, vec ima metoda DatabaseHelper.GetAllConsumers(), i onda ovde odradi logiku
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Read", "AverageConsumptionForCity"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call AverageConsumptionForCity method without having Read permissions.");
            }
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "AverageConsumptionForCity", reason);

                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }
            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "AverageConsumptionForCity", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }
            double s = 0;
            double b = 0;
            double t = 0;
            foreach (Consumer consumer in consumers)
            {
                if(consumer.City.Equals(city))
                {
                    t = 0;
                    foreach (double amount in consumer.Amounts)
                    {
                        t += amount;
                    }
                    s += t / consumer.Amounts.Length;
                    b++;
                }
            }
            AuditHelper.ExecutionSuccess(principal, "AverageConsumptionForCity");
            if(b == 0)
            {
                return 0;
            }
            return s / b;

        }

        public double AverageConsumptionForRegion(string databaseName, string region)
        {
            // ako je databaseName prazan string baci exception
            // iz baze ucita sve entitete, vec ima metoda DatabaseHelper.GetAllConsumers(), i onda ovde odradi logiku
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Read", "AverageConsumptionForRegion"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call AverageConsumptionForRegion method without having Read permissions.");
            }
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "AverageConsumptionForRegion", reason);

                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }
            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "AverageConsumptionForRegion", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }
            double s = 0;
            double b = 0;
            double t = 0;
            foreach (Consumer consumer in consumers)
            {
                if (consumer.Region.Equals(region))
                {
                    t = 0;
                    foreach (double amount in consumer.Amounts)
                    {
                        t += amount;
                    }
                    s += t / consumer.Amounts.Length;
                    b++;
                }
            }
            AuditHelper.ExecutionSuccess(principal, "AverageConsumptionForRegion");
            if (b == 0)
            {
                return 0;
            }
            return s / b;
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Create")]
        public void CreateDatabase(string databaseName)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Create", "CreateDatabase"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call CreateDatabase method without having Create permissions.");
            }

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "CreateDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            if (!DatabaseHelper.CreateDatabase(serviceFolder + databaseName + ".txt"))
            {
                string reason = "Database already exists";
                AuditHelper.ExecutionFailure(principal, "CreateDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            AuditHelper.ExecutionSuccess(principal, "CreateDatabase");
            using (ReplicatorProxy replicatorProxy = new ReplicatorProxy(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9001/ReplicatorService"))))
            {
                string eSecretKeyAes = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKeyAes, "AES/SecretKey.txt");

                byte[] encryptedData = null;

                try
                {
                    encryptedData = AES_CBC_Algorithm.EncryptString(databaseName, eSecretKeyAes);                   
                }
                catch(Exception e)
                {
                    Console.WriteLine("Encryption failed. Reason: {0}", e.Message);
                }

                //string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                string signCertCN = "wcfservice_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(encryptedData, certificateSign);

                replicatorProxy.CreateDatabase(encryptedData, signature);
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public void DeleteDatabase(string databaseName)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Delete", "DeleteDatabase"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call DeleteDatabase method without having Delete permissions.");
            }

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "DeleteDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            if (!DatabaseHelper.DeleteDatabase(serviceFolder + databaseName + ".txt"))
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "DeleteDatabase", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            AuditHelper.ExecutionSuccess(principal, "DeleteDatabase");

            using (ReplicatorProxy replicatorProxy = new ReplicatorProxy(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9001/ReplicatorService"))))
            {
                string eSecretKeyAes = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKeyAes, "AES/SecretKey.txt");

                byte[] encryptedData = null;

                try
                {
                    encryptedData = AES_CBC_Algorithm.EncryptString(databaseName, eSecretKeyAes);                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encryption failed. Reason: {0}", e.Message);
                }

                //string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                string signCertCN = "wcfservice_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(encryptedData, certificateSign);

                replicatorProxy.DeleteDatabase(encryptedData, signature);
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Edit")]
        public void EditConsumer(string databaseName, string region, string city, int year, Months month, double amount)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Edit", "EditConsumer"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call EditConsumer method without having Edit permissions.");
            }

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "EditConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                string reason = "Database doesnt exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "EditConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            bool consumerExists = false;
            foreach (Consumer consumer in consumers)
            {
                if (String.Equals(consumer.Region, region) && String.Equals(consumer.City, city) && consumer.Year == year)
                {
                    consumerExists = true;
                    consumer.Amounts[(int)month] = amount;
                }
            }

            if(!consumerExists)
            {
                string reason = "Consumer doesnt exist";
                AuditHelper.ExecutionFailure(principal, "EditConsumer", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }

            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
            AuditHelper.ExecutionSuccess(principal, "EditConsumer");

            using (ReplicatorProxy replicatorProxy = new ReplicatorProxy(new NetTcpBinding(), new EndpointAddress(new Uri("net.tcp://localhost:9001/ReplicatorService"))))
            {
                string eSecretKeyAes = SecretKey.GenerateKey();
                SecretKey.StoreKey(eSecretKeyAes, "AES/SecretKey.txt");

                byte[] encryptedData = null;
                string data = databaseName + ":" + region + ":" + city + ":" + year + ":" + month + ":" + amount;

                try
                {
                    encryptedData = AES_CBC_Algorithm.EncryptString(data, eSecretKeyAes);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Encryption failed. Reason: {0}", e.Message);
                }

                //string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign";
                string signCertCN = "wcfservice_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(encryptedData, certificateSign);

                replicatorProxy.EditConsumer(encryptedData, signature);
            }
        }
        
        public string MaxConsumerForRegion(string databaseName, string region)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Read", "MaxConsumerForRegion"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call MaxConsumerForRegion method without having Read permissions.");
            }
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                string reason = "Database name cannot be empty";
                AuditHelper.ExecutionFailure(principal, "MaxConsumerForRegion", reason);

                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }
            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                string reason = "Database doesn't exist, is archived or is in faulted state";
                AuditHelper.ExecutionFailure(principal, "MaxConsumerForRegion", reason);

                throw new FaultException<DatabaseException>(new DatabaseException(reason));
            }
            Dictionary<string, double> cityConsumptions = new Dictionary<string, double>();
            foreach (Consumer consumer in consumers)
            {
                if(consumer.Region.Equals(region))
                {
                    if(cityConsumptions.ContainsKey(consumer.City))
                    {
                        double consumption = 0;
                        foreach (double amount in consumer.Amounts)
                        {
                            consumption += amount;
                        }
                        cityConsumptions[consumer.City] += consumption;
                    }
                    else
                    {
                        double consumption = 0;
                        foreach (double amount in consumer.Amounts)
                        {
                            consumption += amount;
                        }
                        cityConsumptions.Add(consumer.City, consumption);
                    }
                }
            }
            string city = "";
            double maxConsumption = 0;
            foreach (var item in cityConsumptions)
            {
                if (item.Value > maxConsumption)
                {
                    maxConsumption = item.Value;
                    city = item.Key;
                }
            }
            AuditHelper.ExecutionSuccess(principal, "MaxConsumerForRegion");
            if(city.Equals("") && maxConsumption==0)
            {
                return "";
            }
            return String.Format("Consumer with highest consumption in region {0} is {1} with consumption {2}.",region,city,maxConsumption);
        }

        private bool CheckPermission(IPrincipal principal, string permission, string serviceName)
        {
            if (principal.IsInRole(permission))
            {
                AuditHelper.AuthorizationSuccess(principal, serviceName);
                return true;
            } else
            {
                AuditHelper.AuthorizationFailure(principal, serviceName, $"Insufficient {permission} permissions");
                return false;
            }
        }
    }
}
