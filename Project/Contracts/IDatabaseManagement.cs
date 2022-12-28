using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IDatabaseManagement
    {
        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void CreateDatabase(string databaseName);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void ArchiveDatabase(string databaseName);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void DeleteDatabase(string databaseName);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void AddConsumer(string databaseName, string region, string city, int year);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void EditConsumer(string databaseName, string region, string city, int year, Months month, double amount);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        void DeleteConsumer(string databaseName, string region, string city, int year);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        double AverageConsumptionForCity(string databaseName, string city);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        double AverageConsumptionForRegion(string databaseName, string region);

        [OperationContract]
        [FaultContract(typeof(DatabaseException))]
        string MaxConsumerForRegion(string databaseName, string region);
    }
}
