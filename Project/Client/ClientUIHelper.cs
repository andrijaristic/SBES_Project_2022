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
            // 5. Add consumer 6. Edit consumer 7.Delete consumer 8. Get average for region 9. Get average for city
            // 10. Get max for region  0. exit
            // provera da li korisnik unese validnu komandu

            return int.Parse(Console.ReadLine());
        }

        public static string GetDatabase()
        {
            // poziva se ako korisnik odabere opciju Switch database
            // od korisnika ucita database

            return Console.ReadLine();
        }

        public static void GetAddParameters(out string region, out string city, out int year)
        {
            // poziva se ako korisnik odabere opciju Add consumer
            // treba da od korisnika ucita svaki ovaj parametar (region,city,year)
            // validacija da li je godina broj

            region = "";
            city = "";
            year = 0;
            return;
        }

        public static void GetDeleteParameters(out string region, out string city, out int year)
        {
            // poziva se ako korisnik odabere opciju Delete consumer
            // treba da od korisnika ucita svaki ovaj parametar (region,city,year)
            // validacija da li je godina broj
            // copy pase GetAddParameters

            region = "";
            city = "";
            year = 0;
            return;
        }

        public static void GetEditParameters(out string region, out string city, out int year, out Months month, out double amount)
        {
            // poziva se ako korisnik odabere opciju Edit consumer
            // treba da od korisnika ucita svaki ovaj parametar (region,city,year, month, amount)
            // validacija da su godina i amount brojevi
            // month mapira na enum

            region = "";
            city = "";
            year = 0;
            amount = 0;
            month = Months.JANUAR;
            return;
        }

        public static string GetRegion()
        {
            // poziva se ako korisnik odabere opciju Get average for region ili Get max for region
            // treba da od korisnika ucita region

            return "";
        }

        public static string GetCity()
        {
            // poziva se ako korisnik odabere opciju Get average for city
            // treba da od korisnika ucita city

            return "";
        }
    }
}
