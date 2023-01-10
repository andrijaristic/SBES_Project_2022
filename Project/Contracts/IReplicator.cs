using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IReplicator
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void CreateDatabase(byte[] encryptedDatabaseName, byte[] signature);
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void DeleteDatabase(byte[] encryptedDatabaseName, byte[] signature);
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void ArchiveDatabase(byte[] encryptedDatabaseName, byte[] signature);
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void AddConsumer(byte[] encryptedData, byte[] signature);
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void EditConsumer(byte[] encryptedData, byte[] signature);
    }
}
