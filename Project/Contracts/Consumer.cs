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
        private double[] amounts;   // 12 vrednosti, po jedna za svaki mesec

        public Consumer(string region, string city, int year)
        {
            this.Id = Guid.NewGuid();
            this.Region = region;
            this.City = city;
            this.Year = year;
            amounts = new double[12];
            
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
        public double[] Amounts { get => amounts; set => amounts = value; }


        public static string ToString(Consumer consumer)
        {
            if(consumer == null)
            {
                return "";
            }
            string consumerString = consumer.Id + ";" + consumer.Region + ";" + consumer.City + ";" + consumer.Year + ";";
            for(int i = 0; i < 12; i++)
            {
                consumerString += consumer.Amounts[i] + ";";
            }
            consumerString = consumerString.Remove(consumerString.Length - 1, 1);
            return consumerString;
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
                double[] amounts = new double[12];
                for(int i = 4; i < 15; i++)
                {
                    amounts[i-4] = double.Parse(properties[i]);
                }
                Consumer consumer = new Consumer(region, city, year);
                consumer.id = id;
                consumer.Amounts = amounts;
                return consumer;
            }
            catch
            {
                return null;
            }
        }
    }
}
