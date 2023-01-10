using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities.CertificateManager;

namespace Service
{
    public class ReplicatorProxy : ChannelFactory<IReplicator>, IReplicator, IDisposable
    {
        IReplicator factory;

        public ReplicatorProxy(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            factory = this.CreateChannel();
        }
        public void AddConsumer(byte[] encryptedData, byte[] signature)
        {
            try
            {
                factory.AddConsumer(encryptedData, signature);
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

        public void ArchiveDatabase(byte[] encryptedDatabaseName, byte[] signature)
        {
            try
            {
                factory.ArchiveDatabase(encryptedDatabaseName, signature);
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

        public void CreateDatabase(byte[] encryptedDatabaseName, byte[] signature)
        {
            try
            {
                factory.CreateDatabase(encryptedDatabaseName, signature);
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

        public void DeleteDatabase(byte[] databaseName, byte[] signature)
        {
            try
            {
                factory.DeleteDatabase(databaseName, signature);
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

        public void EditConsumer(byte[] encryptedData, byte[] signature)
        {
            try
            {
                factory.EditConsumer(encryptedData, signature);
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
    }
}
