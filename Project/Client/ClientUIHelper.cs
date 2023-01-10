using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientUIHelper
    {

        public static int Menu(string databaseName)
        {
            // sa klijenta se salje naziv baze sa kojom korisnik radi
            // ispise korisniku naziv baze sa kojom radi, opcije , i ucita korisnikovu opciju
            // opcije su: 1.Switch database 2. Crate database 3. Delete database 4.Archive database
            // 5. Add consumer 6. Edit consumer 7. Get average for region 8. Get average for city
            // 9. Get max for region  0. exit
            // provera da li korisnik unese validnu komandu
            if (string.IsNullOrWhiteSpace(databaseName))
                Console.WriteLine("Database is not loaded.");
            else
                Console.WriteLine("Database: "+databaseName);
            Console.WriteLine("1. Switch database");
            Console.WriteLine("2. Create database");
            Console.WriteLine("3. Delete database");
            Console.WriteLine("4. Archive database");
            Console.WriteLine("5. Add consumer");
            Console.WriteLine("6. Edit consumer");
            Console.WriteLine("7. Get average for region");
            Console.WriteLine("8. Get average for city");
            Console.WriteLine("9. Get max for region");
            Console.WriteLine("0. Exit");
            Console.WriteLine("Insert command:");
            int command;
            if (int.TryParse(Console.ReadLine(), out command) == false)
                return -1;
            return command;
        }

        public static string GetDatabase()
        {
            // poziva se ako korisnik odabere opciju Switch database
            // od korisnika ucita database
            Console.WriteLine("Enter database name:");
            return Console.ReadLine();
        }

        public static void GetAddParameters(out string region, out string city, out int year)
        {
            // poziva se ako korisnik odabere opciju Add consumer
            // treba da od korisnika ucita svaki ovaj parametar (region,city,year)
            // validacija da li je godina broj
            Console.WriteLine("Enter region: ");
            region = Console.ReadLine();
            Console.WriteLine("Enter city name: ");
            city = Console.ReadLine();
            Console.WriteLine("Enter year: ");
            int.TryParse(Console.ReadLine(),out year);
            return;
        }

        public static void GetEditParameters(out string region, out string city, out int year, out Months month, out double amount)
        {
            // poziva se ako korisnik odabere opciju Edit consumer
            // treba da od korisnika ucita svaki ovaj parametar (region,city,year, month, amount)
            // validacija da su godina i amount brojevi
            // month mapira na enum

            Console.WriteLine("Enter region: ");
            region = Console.ReadLine();
            Console.WriteLine("Enter city name: ");
            city = Console.ReadLine();
            Console.WriteLine("Enter year: ");
            int.TryParse(Console.ReadLine(), out year);
            Console.WriteLine("Enter month:");
            string hMonth = Console.ReadLine();
            hMonth = hMonth.ToLower();
            switch (hMonth)
            {
                case "1":
                case "januar":
                case "january":
                    month = Months.JANUAR;
                    break;
                case "2":
                case "februar":
                case "february":
                    month = Months.FEBRUAR;
                    break;
                case "3":
                case "mart":
                case "march":
                    month = Months.MART;
                    break;
                case "4":
                case "april":
                    month = Months.APRIL;
                    break;
                case "5":
                case "maj":
                case "may":
                    month = Months.MAJ;
                    break;
                case "6":
                case "jun":
                case "june":
                    month = Months.JUN;
                    break;
                case "7":
                case "jul":
                case "july":
                    month = Months.JUL;
                    break;
                case "8":
                case "avgust":
                case "august":
                    month = Months.AVGUST;
                    break;
                case "9":
                case "septembar":
                case "september":
                    month = Months.SEPTEMBAR;
                    break;
                case "10":
                case "oktobar":
                case "october":
                    month = Months.OKTOBAR;
                    break;
                case "11":
                case "novembar":
                case "november":
                    month = Months.NOVEMBAR;
                    break;
                case "12":
                case "decembar":
                case "december":
                    month = Months.DECEMBAR;
                    break;
                default:
                    throw new Exception("Month doensn't exist.");
                    break;
            }
            Console.WriteLine("Enter amount:");
            double.TryParse(Console.ReadLine(), out amount);
            //month = Months.JANUAR;
            return;
        }

        public static string GetRegion()
        {
            // poziva se ako korisnik odabere opciju Get average for region ili Get max for region
            // treba da od korisnika ucita region
            Console.WriteLine("Enter region:");
            return Console.ReadLine();
        }

        public static string GetCity()
        {
            // poziva se ako korisnik odabere opciju Get average for city
            // treba da od korisnika ucita city

            Console.WriteLine("Enter city:");
            return Console.ReadLine();
        }
    }
}
