using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class Consumer
    {
        private Guid id;
        private string region;
        private string city;
        private int year;
        private double amount;

        public Consumer(string region, string city, int year, double amount)
        {
            this.Id = Guid.NewGuid();
            this.Region = region;
            this.City = city;
            this.Year = year;
            this.Amount = amount;
        }

        [DataMember]
        public Guid Id { get => id; set => id = value; }
        [DataMember]
        public string Region { get => region; set => region = value; }
        [DataMember]
        public string City { get => city; set => city = value; }
        [DataMember]
        public int Year { get => year; set => year = value; }
        [DataMember]
        public double Amount { get => amount; set => amount = value; }


        public static string ToString(Consumer consumer)
        {
            if(consumer == null)
            {
                return "";
            }
            return consumer.Id + ";" + consumer.Region + ";" + consumer.City + ";" + consumer.Year + ";" + consumer.Amount;
        }

        public static Consumer FromString(string consumerString)
        {
            try
            {
                string[] properties = consumerString.Split(';');
                Guid id = Guid.Parse(properties[0]);
                string region = properties[1];
                string city = properties[2];
                int year = int.Parse(properties[3]);
                double amount = double.Parse(properties[4]);
                Consumer consumer = new Consumer(region, city, year, amount);
                consumer.id = id;
                return consumer;
            }
            catch
            {
                return null;
            }
        }
    }
}
