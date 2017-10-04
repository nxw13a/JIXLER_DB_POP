using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.PhantomJS;
using MongoDB.Driver;
using MongoDB.Bson;
namespace ConsoleApp5
{
    public class Location_
    {
        public string _id { get; set; }
        public string jixName { get; set; }
        public string longitude { get; set; }
        public string lattitude { get; set; }
        public string JIX_TELL_ABOUT { get; set; }
        public string Location { get; set; }
        public string START_DATE { get; set; }
        public double[] currentLocation { get; set; }
    }

    class Program
    {
        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds,String sign)
        {
            //Decimal degrees = 
            //   whole number of degrees, 
            //   plus minutes divided by 60, 
            //   plus seconds divided by 3600
            int number = 1;
            if (sign == "S" || sign == "W")
                number *= -1;


            return (degrees + (minutes / 60) + (seconds / 3600))*number;
        }

        static void Main(string[] args)
        {
            string connectionString = "mongodb://52.33.219.239:27017";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            MongoClient client = new MongoClient(settings);
#pragma warning disable CS0618 // Type or member is obsolete
            MongoServer server = client.GetServer();
#pragma warning restore CS0618 // Type or member is obsolete
            MongoDatabase db = server.GetDatabase("dev");
            MongoCollection<Location_> collection = db.GetCollection<Location_>("JixInfo");

            IWebDriver driver = new ChromeDriver(@"C:\Users\natta\Downloads\");
            driver.Navigate().GoToUrl("https://www.hmdb.org//");
            driver.FindElement(By.XPath("//*[@title='Markers near your present location']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//*[@value='List of Markers Near Your Location']")).Click();
            driver.FindElement(By.PartialLinkText("Company I   7th Texas Infantry")).Click();

            Thread.Sleep(2000);
            String name = driver.FindElement(By.XPath("//h1")).Text.ToString();

            String hold_long = driver.FindElement(By.XPath("(//*[@class='bodyserif'])")).Text.ToString();
            int begin = hold_long.IndexOf("Location.");
            int holder = hold_long.IndexOf("Erected");
            int begin1 = hold_long.IndexOf("Inscription. ");

            String longi = hold_long.Substring(begin + 10,13);
            String lat = hold_long.Substring(begin + 10 + 15, 13);
            String info = hold_long.Substring(begin1 + 13, holder - begin1 - 13);

            double degree = Convert.ToDouble(longi.Substring(0, 2));
            double minutes = Convert.ToDouble(longi.Substring(4, 6));
            double total = ConvertDegreeAngleToDouble(degree,minutes,0,longi.Substring(longi.Length-1,1));
            longi = System.Convert.ToString(total);
            double[] hold_num1 = new double[2];
            hold_num1[1] = total;
            double degree1 = Convert.ToDouble(lat.Substring(0, 2));
            double minutes1 = Convert.ToDouble(lat.Substring(4, 6));
            double total1 = ConvertDegreeAngleToDouble(degree1, minutes1, 0, lat.Substring(lat.Length - 1, 1));
            lat = System.Convert.ToString(total1);
            hold_num1[0] = total1;
            Console.WriteLine(name);
            Console.WriteLine(longi);
            Console.WriteLine(lat);
            Console.WriteLine(info);

            driver.Navigate().Back();
            Thread.Sleep(2000);
            driver.FindElement(By.PartialLinkText("Abilene Woman's Club Building")).Click();

            String name1 = driver.FindElement(By.XPath("//h1")).Text.ToString();
            String hold_long1 = driver.FindElement(By.XPath("(//*[@class='bodyserif'])")).Text.ToString();
            int begin2 = hold_long1.IndexOf("Location.");
            int holder1 = hold_long1.IndexOf("Erected");
            int begin11 = hold_long1.IndexOf("Inscription. ");

            String longi1 = hold_long1.Substring(begin2 + 10, 13);
            String lat1 = hold_long1.Substring(begin2 + 10 + 15, 13);
            String info1 = hold_long1.Substring(begin11 + 13, holder1 - begin11 - 13);

            double degree11 = Convert.ToDouble(longi1.Substring(0, 2));
            double minutes11 = Convert.ToDouble(longi1.Substring(4, 6));
            double total11 = ConvertDegreeAngleToDouble(degree11, minutes11, 0, longi1.Substring(longi1.Length - 1, 1));
            longi1 = System.Convert.ToString(total11);
            double[] hold_num = new double[2];
            hold_num[1] = total11;

            double degree12 = Convert.ToDouble(lat1.Substring(0, 2));
            double minutes12 = Convert.ToDouble(lat1.Substring(4, 6));
            double total12 = ConvertDegreeAngleToDouble(degree12, minutes12, 0, lat1.Substring(lat1.Length - 1, 1));
            lat1 = System.Convert.ToString(total12);
            hold_num[0] = total12;

            Console.WriteLine(name1);
            Console.WriteLine(longi1);
            Console.WriteLine(lat1);
            Console.WriteLine(info1);



            Location_ a = new Location_
            {
                _id = ObjectId.GenerateNewId().ToString(),
                jixName = name,
                longitude = lat,
                lattitude = longi,
                JIX_TELL_ABOUT = info,
                Location = lat + "feet," + longi + "feet",
                START_DATE = "Sept 25, 2017 at 10:45 AM",
                currentLocation = hold_num1
            };
            Location_ b = new Location_
            {
                _id = ObjectId.GenerateNewId().ToString(),
                jixName = name1,
                longitude = lat1,
                lattitude = longi1,
                JIX_TELL_ABOUT = info1,
                Location = lat1 + "feet," + longi1+"feet",
                START_DATE = "Sept 25, 2017 at 10:45 AM",
                currentLocation = hold_num
                
           
            };

            collection.Save(a);
            collection.Save(b);
            /*
            driver.Navigate().GoToUrl("http://www.realtor.com/");
            driver.FindElement(By.Id("searchBox")).SendKeys("10024");
            driver.FindElement(By.XPath("//*[@class='btn btn-primary js-searchButton']")).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.Id("srp-sortby")).Click();
            driver.FindElement(By.Id("srp-sortby")).SendKeys(Keys.Down + Keys.Enter);
            String price = driver.FindElement(By.XPath("//*[@class='data-price-display']")).Text.ToString();
            String address = driver.FindElement(By.XPath("//*[@class='srp-item-address ellipsis']")).Text.ToString();
            String price2 = driver.FindElement(By.XPath("(//*[@class='data-price-display'])[2]")).Text.ToString();
            String address2 = driver.FindElement(By.XPath("(//*[@class='srp-item-address ellipsis'])[2]")).Text.ToString();
            Console.WriteLine();
            Console.WriteLine(price);
            Console.WriteLine(address);
            Console.WriteLine(price2);
            Console.WriteLine(address2);
            House a = new House
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Value = price,
                Address = address
            };
            House b = new House
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Value = price2,
                Address = address2
            };
            collection.Save(a);
            collection.Save(b);

            */
            Console.ReadKey();
            driver.Close();
        }
    }
}
