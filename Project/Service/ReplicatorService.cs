using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.CertificateManager;
using Utilities.Cryptography;

namespace Service
{
    public class ReplicatorService : IReplicator
    {
        private string serviceFolder = "ReplicatorData/";

        public void AddConsumer(byte[] encryptedData, byte[] signature)
        {
            //string clienName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            //string clientNameSign = clienName + "_sign";
            string clientNameSign = "wcfservice_sign";

            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(encryptedData, signature, certificate))
            {
                Console.WriteLine("Sign is valid");

                IPrincipal principal = Thread.CurrentPrincipal;
                string decryptedData = AES_CBC_Algorithm.DecryptString(encryptedData, SecretKey.LoadKey("AES/SecretKey.txt"));
                string decryptedDatabaseName = decryptedData.Split(':')[0];
                string decryptedRegion = decryptedData.Split(':')[1];
                string decryptedCity = decryptedData.Split(':')[2];
                int decryptedYear = Convert.ToInt32(decryptedData.Split(':')[3]);
                Guid decryptedId = Guid.Parse(decryptedData.Split(':')[4]);

                List<Consumer> consumers;
                bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + decryptedDatabaseName + ".txt", out consumers);
                if (!readingSuccessful)
                {
                    string reason = "Database doesnt exist, is archived or is in faulted state";
                    AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                foreach (Consumer consumer in consumers)
                {
                    if (String.Equals(consumer.Region, decryptedRegion) && String.Equals(consumer.City, decryptedCity) && consumer.Year == decryptedYear)
                    {
                        string reason = "Consumer with that region, city and year already exists";
                        AuditHelper.ExecutionFailure(principal, "AddConsumer", reason);

                        throw new FaultException<DatabaseException>(new DatabaseException(reason));
                    }
                }

                Consumer c = new Consumer(decryptedRegion, decryptedCity, decryptedYear);
                c.Id = decryptedId;
                consumers.Add(c);
                DatabaseHelper.SaveConsumers(serviceFolder + decryptedDatabaseName + ".txt", consumers);
                AuditHelper.Replication(principal);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                //AuditHelper.ExecutionFailure(principal, "CreateDatabase", "Digital signature is invalid.");
            }        
        }

        public void ArchiveDatabase(byte[] encryptedDatabaseName, byte[] signature)
        {
            //string clienName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            //string clientNameSign = clienName + "_sign";
            string clientNameSign = "wcfservice_sign";

            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(encryptedDatabaseName, signature, certificate))
            {
                Console.WriteLine("Sign is valid");

                IPrincipal principal = Thread.CurrentPrincipal;
                string decryptedDatabaseName = AES_CBC_Algorithm.DecryptString(encryptedDatabaseName, SecretKey.LoadKey("AES/SecretKey.txt"));

                List<Consumer> consumers;
                bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + decryptedDatabaseName + ".txt", out consumers);
                if (!readingSuccessful)
                {
                    string reason = "Database doesn't exist, is archived or is in faulted state";
                    AuditHelper.ExecutionFailure(principal, "ArchiveDatabase", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                if (!DatabaseHelper.ArchiveDatabase(serviceFolder + decryptedDatabaseName + ".txt", consumers))
                {
                    string reason = "Database doesn't exist, is archived or is in faulted state";
                    AuditHelper.ExecutionFailure(principal, "ArchiveDatabase", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                AuditHelper.Replication(principal);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                //AuditHelper.ExecutionFailure(principal, "CreateDatabase", "Digital signature is invalid.");
            }         
        }

        public void CreateDatabase(byte[] encryptedDatabaseName, byte[] signature)
        {
            //string clienName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            //string clientNameSign = clienName + "_sign";
            string clientNameSign = "wcfservice_sign";

            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(encryptedDatabaseName, signature, certificate))
            {
                Console.WriteLine("Sign is valid");

                IPrincipal principal = Thread.CurrentPrincipal;
                string decryptedDatabaseName = AES_CBC_Algorithm.DecryptString(encryptedDatabaseName, SecretKey.LoadKey("AES/SecretKey.txt"));

                if (!DatabaseHelper.CreateDatabase(serviceFolder + decryptedDatabaseName + ".txt"))
                {
                    string reason = "Database already exists";
                    AuditHelper.ExecutionFailure(principal, "CreateDatabase", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                AuditHelper.Replication(principal);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                //AuditHelper.ExecutionFailure(principal, "CreateDatabase", "Digital signature is invalid.");
            }           
        }

        public void DeleteDatabase(byte[] encryptedDatabaseName, byte[] signature)
        {
            //string clienName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            //string clientNameSign = clienName + "_sign";
            string clientNameSign = "wcfservice_sign";

            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(encryptedDatabaseName, signature, certificate))
            {
                Console.WriteLine("Sign is valid");

                IPrincipal principal = Thread.CurrentPrincipal;
                string decryptedDatabaseName = AES_CBC_Algorithm.DecryptString(encryptedDatabaseName, SecretKey.LoadKey("AES/SecretKey.txt"));

                if (!DatabaseHelper.DeleteDatabase(serviceFolder + decryptedDatabaseName + ".txt"))
                {
                    string reason = "Database doesn't exist, is archived or is in faulted state";
                    AuditHelper.ExecutionFailure(principal, "DeleteDatabase", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                AuditHelper.Replication(principal);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                //AuditHelper.ExecutionFailure(principal, "CreateDatabase", "Digital signature is invalid.");
            }           
        }

        public void EditConsumer(byte[] encryptedData, byte[] signature)
        {
            //string clienName = Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
            //string clientNameSign = clienName + "_sign";
            string clientNameSign = "wcfservice_sign";

            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);

            if (DigitalSignature.Verify(encryptedData, signature, certificate))
            {
                Console.WriteLine("Sign is valid");

                IPrincipal principal = Thread.CurrentPrincipal;
                string decryptedData = AES_CBC_Algorithm.DecryptString(encryptedData, SecretKey.LoadKey("AES/SecretKey.txt"));
                string decryptedDatabaseName = decryptedData.Split(':')[0];
                string decryptedRegion = decryptedData.Split(':')[1];
                string decryptedCity = decryptedData.Split(':')[2];
                int decryptedYear = Convert.ToInt32(decryptedData.Split(':')[3]);
                Months decryptedMonth = (Months)Enum.Parse(typeof(Months), decryptedData.Split(':')[4], true);
                double decryptedAmount = Convert.ToDouble(decryptedData.Split(':')[5]);

                List<Consumer> consumers;
                bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + decryptedDatabaseName + ".txt", out consumers);
                if (!readingSuccessful)
                {
                    string reason = "Database doesnt exist, is archived or is in faulted state";
                    AuditHelper.ExecutionFailure(principal, "EditConsumer", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                bool consumerExists = false;
                foreach (Consumer consumer in consumers)
                {
                    if (String.Equals(consumer.Region, decryptedRegion) && String.Equals(consumer.City, decryptedCity) && consumer.Year == decryptedYear)
                    {
                        consumerExists = true;
                        consumer.Amounts[(int)decryptedMonth] = decryptedAmount;
                    }
                }

                if (!consumerExists)
                {
                    string reason = "Consumer doesnt exist";
                    AuditHelper.ExecutionFailure(principal, "EditConsumer", reason);

                    throw new FaultException<DatabaseException>(new DatabaseException(reason));
                }

                DatabaseHelper.SaveConsumers(serviceFolder + decryptedDatabaseName + ".txt", consumers);
                AuditHelper.Replication(principal);
            }
            else
            {
                Console.WriteLine("Sign is invalid");
                //AuditHelper.ExecutionFailure(principal, "CreateDatabase", "Digital signature is invalid.");
            }         
        }
    }
}
