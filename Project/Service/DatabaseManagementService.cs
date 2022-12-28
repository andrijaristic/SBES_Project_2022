﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

namespace Service
{
    public class DatabaseManagementService : IDatabaseManagement
    {
        private string serviceFolder = "ServiceData/";
        // dodaj polje koje je proxy ka servisu za replikaciju, i pozivaj ga u metodama gde se uspesno izmene podaci

        [PrincipalPermission(SecurityAction.Demand, Role = "Add")]
        public void AddConsumer(string databaseName, string region, string city, int year, double amount)
        {
            AuditHelper.AutorizeLog("AddConsumer");

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
            AuditHelper.AutorizeLog("ArchiveDatabase");

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if (!readingSuccessful)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesn't exist, is archived or is in faulted state"));
            }

            if (!DatabaseHelper.ArchiveDatabase(serviceFolder + databaseName + ".txt", consumers))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesn't exist, is already archived or is in faulted state"));
            }
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
            AuditHelper.AutorizeLog("CreateDatabase");

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            if (!DatabaseHelper.CreateDatabase(serviceFolder + databaseName + ".txt"))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database already exists"));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Delete")]
        public void DeleteDatabase(string databaseName)
        {
            AuditHelper.AutorizeLog("DeleteDatabase");

            if (String.IsNullOrWhiteSpace(databaseName))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            if (!DatabaseHelper.DeleteDatabase(serviceFolder + databaseName + ".txt"))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesn't exist, is archived or is in faulted state"));
            }
        }

        [PrincipalPermission(SecurityAction.Demand, Role = "Edit")]
        public void EditConsumer(string databaseName, string region, string city, int year, double amount)
        {
            AuditHelper.AutorizeLog("EditConsumer");

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
