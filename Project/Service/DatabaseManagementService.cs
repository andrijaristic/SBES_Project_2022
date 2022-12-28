using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using Utilities;

namespace Service
{
    public class DatabaseManagementService : IDatabaseManagement
    {
        private string serviceFolder = "ServiceData/";
        // dodaj polje koje je proxy ka servisu za replikaciju, i pozivaj ga u metodama gde se uspesno izmene podaci

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
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            if (String.IsNullOrWhiteSpace(region))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Region cannot be empty"));
            }

            if (String.IsNullOrWhiteSpace(city))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("City cannot be empty"));
            }

            List<Consumer> consumers;
            bool readingSuccessful = DatabaseHelper.GetAllConsumers(serviceFolder + databaseName + ".txt", out consumers);
            if(!readingSuccessful)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesnt exist, is archived or is in faulted state"));
            }

            foreach(Consumer consumer in consumers)
            {
                if(String.Equals(consumer.Region,region) && String.Equals(consumer.City, city) && consumer.Year == year)
                {
                    throw new FaultException<DatabaseException>(new DatabaseException("Consumer with that region, city and year already exists"));
                }
            }

            consumers.Add(new Consumer(region, city, year));
            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
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
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            if (!DatabaseHelper.CreateDatabase(serviceFolder + databaseName + ".txt"))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database already exists"));
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
                throw new FaultException<DatabaseException>(new DatabaseException("Database name cannot be empty"));
            }

            if (!DatabaseHelper.DeleteDatabase(serviceFolder + databaseName + ".txt"))
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Database doesn't exist, is archived or is in faulted state"));
            }
        }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Edit")]
        public void DeleteConsumer(string databaseName, string region, string city, int year)
        {
            IPrincipal principal = Thread.CurrentPrincipal;
            if (!CheckPermission(principal, "Edit", "DeleteConsumer"))
            {
                string userName = Formatter.ParseName(principal.Identity.Name).Split(',')[0].Split('=')[1];
                throw new FaultException($"User {userName} tried to call DeleteConsumer method without having Edit permissions.");
            }

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
            Consumer deleteConsumer = null;
            foreach (Consumer consumer in consumers)
            {
                if (String.Equals(consumer.Region, region) && String.Equals(consumer.City, city) && consumer.Year == year)
                {
                    consumerExists = true;
                    deleteConsumer = consumer;
                }
            }

            if (!consumerExists)
            {
                throw new FaultException<DatabaseException>(new DatabaseException("Consumer doesnt exist"));
            }

            consumers.Remove(deleteConsumer);
            DatabaseHelper.SaveConsumers(serviceFolder + databaseName + ".txt", consumers);
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
                    consumer.Amounts[(int)month] = amount;
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

        private bool CheckPermission(IPrincipal principal, string permission, string serviceName)
        {
            if (principal.IsInRole(permission))
            {
                AuditHelper.AuthorizationSuccess(serviceName);
                return true;
            } else
            {
                AuditHelper.AuthorizationFailure(serviceName, $"Insufficient {permission} permissions");
                return false;
            }
        }
    }
}
