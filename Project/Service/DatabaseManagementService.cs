using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DatabaseManagementService : IDatabaseManagement
    {
        private string serviceFolder = "ServiceData/";
        // dodaj polje koje je proxy ka servisu za replikaciju, i pozivaj ga u metodama gde se uspesno izmene podaci

        [PrincipalPermission(SecurityAction.Demand, Role = "Add")]
        public void AddConsumer(string databaseName, string region, string city, int year, double amount)
        {
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if(!readingSuccessful)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesnt exist, is archived or is in faulted state"));
            }

            foreach(Consumer consumer in consumers)
            {
                if(String.Equals(consumer.Region,region) && String.Equals(consumer.City, city) && consumer.Year == year && consumer.Amount == amount)
                {
                    throw new FaultException<DatabaseException>(new DatabaseException("Consumer with that region, city and year already exists"));
                }
            }

            consumers.Add(new Consumer(region, city, year, amount));
            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Archive")]
        public void ArchiveDatabase(string databaseName)
        {
            // ako je databaseName prazan string baci exception
            // ako nije onda napravi u DatabaseHelper metodu koja ce da nadje postojecu bazu koja nije arhivirana
            // i arhivira je, entiteti ne treba da se obrisu, samo treba prvi red da se promeni iz False u True
            throw new NotImplementedException();
        }

        public double AverageConsumptionForCity(string databaseName, string city)
        {
            // ako je databaseName prazan string baci exception
            // iz baze ucita sve entitete, vec ima metoda DatabaseHelper.GetAllConsumers(), i onda ovde odradi logiku
            throw new NotImplementedException();
        }

        public double AverageConsumptionForRegion(string databaseName, string region)
        {
            // ako je databaseName prazan string baci exception
            // iz baze ucita sve entitete, vec ima metoda DatabaseHelper.GetAllConsumers(), i onda ovde odradi logiku
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Create")]
        public void CreateDatabase(string databaseName)
        {
            // ako je databaseName prazan string baci exception
            // napravi metodu u DatabaseHelper koja pravi bazu samo ako baza sa istim imenom ne postoji
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public void DeleteDatabase(string databaseName)
        {
            // ako je databaseName prazan string baci exception
            // napravi metodu u DatabaseHelper koja brise bazu samo ako baza postoji i nije arhivirana
            throw new NotImplementedException();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Edit")]
        public void EditConsumer(string databaseName, string region, string city, int year, double amount)
        {
            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesnt exist, is archived or is in faulted state"));
            }

            bool consumerExists = false;
            foreach (Consumer consumer in consumers)
            {
                if (String.Equals(consumer.Region, region) && String.Equals(consumer.City, city) && consumer.Year == year)
                {
                    consumerExists = true;
                    consumer.Amount = amount;
                }
            }

            if(!consumerExists)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Consumer doesnt exist"));
            }

            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
        }

        public string MaxConsumerForRegion(string databaseName, string region)
        {
            // ako je databaseName prazan string baci exception
            // iz baze ucita sve entitete, vec ima metoda DatabaseHelper.GetAllConsumers(), i onda ovde odradi logiku
            throw new NotImplementedException();
        }
    }
}
