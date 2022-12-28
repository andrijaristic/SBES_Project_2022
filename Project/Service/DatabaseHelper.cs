using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DatabaseHelper
    {
        public static bool CreateDatabase(string fileName)
        {
            if (File.Exists(fileName))
            {
                return false;
            }

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                // first line in database file is true if database is archived, or false if it is not
                sw.WriteLine(false);
            }
            return true;
        }

        public static bool GetAllConsumers(string fileName, out List<Consumer> consumers)
        {
            consumers = new List<Consumer>();
            if (!File.Exists(fileName))
            {
                return false;
            }
            using (StreamReader sr = new StreamReader(fileName))
            {
                bool archived = bool.Parse(sr.ReadLine());
                if (archived)
                {
                    return false;
                }

                string consumerText = "";
                while ((consumerText = sr.ReadLine()) != null)
                {
                    Consumer consumer = Consumer.FromString(consumerText);
                    if(consumer == null)
                    {
                        return false;
                    }
                    consumers.Add(consumer);
                }
            }
            return true;
        }

        public static bool SaveConsumers(string fileName, List<Consumer> consumers)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            using(StreamReader sr = new StreamReader(fileName))
            {
                bool archived = bool.Parse(sr.ReadLine());
                if (archived)
                {
                    return false;
                }
            }

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine(false);
                foreach(Consumer consumer in consumers)
                {
                    sw.WriteLine(Consumer.ToString(consumer));
                }
            }
            return true;
        }

        public static bool ArchiveDatabase(string filename, List<Consumer> consumers)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(true);
                foreach(Consumer consumer in consumers)
                {
                    sw.WriteLine(Consumer.ToString(consumer));
                }
            }

            return true;
        }

        public static bool DeleteDatabase(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            using(StreamReader sr = new StreamReader(filename))
            {
                bool archived = bool.Parse(sr.ReadLine());
                if (archived) 
                {
                    return false;
                }
            }

            File.Delete(filename);

            return true;
        }
    }
}
