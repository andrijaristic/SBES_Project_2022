using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Utilities.CertificateManager;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9000/Service"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (ClientProxy proxy = new ClientProxy(binding, address))
            {
                string databaseName = "";

                while (true)
                {
                    int input = ClientUIHelper.Menu(databaseName);
                    bool finished = false;
                    string region;
                    string city;
                    int year;
                    double amount;

                    switch (input)
                    {
                        case 1:
                            databaseName = ClientUIHelper.GetDatabase();
                            break;
                        case 2:
                            proxy.CreateDatabase(databaseName);
                            break;
                        case 3:
                            proxy.DeleteDatabase(databaseName);
                            break;
                        case 4:
                            proxy.ArchiveDatabase(databaseName); 
                            break;
                        case 5:
                            ClientUIHelper.GetParameters(out region, out city, out year, out amount);
                            proxy.AddConsumer(databaseName, region, city, year, amount);
                            break;
                        case 6:
                            ClientUIHelper.GetParameters(out region, out city, out year, out amount);
                            proxy.EditConsumer(databaseName, region, city, year, amount);
                            break;
                        case 7:
                            break;
                        case 8:
                            break;
                        case 9:
                            break;
                        case 0:
                            Console.WriteLine("Exiting");
                            finished = true;
                            break;
                        default:
                            Console.WriteLine("Unknown option");
                            break;
                    }

                    if (finished)
                    {
                        break;
                    }
                }
            }
            Console.WriteLine("Connection closed, press enter to close the window");
            Console.ReadLine();
        }
    }
}
