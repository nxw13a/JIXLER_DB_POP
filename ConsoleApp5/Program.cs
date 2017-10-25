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
    //Database Data
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

        public string END_DATE { get; set; }
        public string _p_FromUser { get; set; }
        public string accessType { get; set; }
        public string jixCategory { get; set; }
        public string jixRadius { get; set; }
        public string _p_userid { get; set; }
        //public string jixMessage { get; set; }
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
        private static String error_ad = "\r\nPaid Advertisement\r\nClick on the ad for more information.\r\nPlease report objectionable advertising to the Editor.";
        public static String remove_ad(String sentences)
        {
            int index = sentences.IndexOf(error_ad);
            if(index != - 1)
            {
                return sentences.Remove(index);
            }
            return sentences;
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
            driver.Navigate().GoToUrl("https://www.hmdb.org/search.asp");
  
            driver.FindElement(By.XPath("//*[@name='Zip']")).SendKeys("79605");
            driver.FindElement(By.XPath("//*[@name='Zip']")).SendKeys(Keys.Enter);
            Thread.Sleep(2000);
            
            IList<IWebElement> all = driver.FindElements(By.XPath("//*/article/table/tbody/tr/td/a"));

            String[] allText = new String[all.Count];
            int i = 0;
            Console.Write("Number of Item(s): ");
            Console.WriteLine(all.Count / 2);
            Console.Write("Number of Wanted Item(s): ");
            int require_num = Convert.ToInt32(Console.ReadLine()); ;
            
            foreach (IWebElement element in all)
            {
                    allText[i++] = element.Text;
                    //Console.WriteLine(allText[i - 1]);
            }
            //Console.WriteLine(all.Count);
            String[] certainText = new String[all.Count/2];
            for (int y = 0; y < all.Count / 2; y++)
            {
                certainText[y] = allText[2 * y + 1];
                //Console.WriteLine(allText[2*y + 1]);               
            }
            for(int x = 0; x < require_num; x++)
            {
                //Console.WriteLine(certainText[x]);
                driver.FindElement(By.PartialLinkText(certainText[x])).Click();

                Thread.Sleep(2000);
                String name = driver.FindElement(By.XPath("//h1")).Text.ToString();

                String hold_long = driver.FindElement(By.XPath("(//*[@class='bodyserif'])")).Text.ToString();
                int begin = hold_long.IndexOf("Location.");
                int holder = hold_long.IndexOf("Erected");
                int begin1 = hold_long.IndexOf("Inscription. ");
                int number = begin + 10;
                int last_n = number;
                while(true)
                {
                    if (hold_long.ElementAt(number) == 'N' || hold_long.ElementAt(number) == 'E' || hold_long.ElementAt(number) == 'S' || hold_long.ElementAt(number) == 'W')
                        break;
                    number++;
                }
                //Console.WriteLine(number - last_n);
                String longi = hold_long.Substring(begin + 10, number - last_n + 1);
                number = begin + 10;
                while (true)
                {
                    if (hold_long.ElementAt(number) == ',')
                        break;
                    number++;
                }
                //Console.WriteLine(number - last_n);
                String lat = hold_long.Substring(begin + 10 + number - last_n + 2, 13);

                String info = hold_long.Substring(begin1 + 13, holder - begin1 - 13);
                info = remove_ad(info);
                //Console.WriteLine(longi); Console.WriteLine(lat);
                number = 0;
                last_n = number;
                while (true)
                {
                    if (longi.ElementAt(number) == '°')
                        break;
                    number++;
                }
                double degree = Convert.ToDouble(longi.Substring(0, number));
                number = number + 2;
                last_n = number;
                while (true)
                {
                    //Console.WriteLine(lat.ElementAt(number));
                    if (longi.ElementAt(number) == ' ')
                        break;
                    number++;
                }
                double minutes = Convert.ToDouble(longi.Substring(last_n, number - last_n - 1));
                double total = ConvertDegreeAngleToDouble(degree, minutes, 0, longi.Substring(longi.Length - 1, 1));
                longi = System.Convert.ToString(total);
                double[] hold_num1 = new double[2];
                hold_num1[1] = total;
                number = 0;
                last_n = number;
                while (true)
                {
                    if (lat.ElementAt(number) == '°')
                        break;
                    number++;
                }
                //Console.WriteLine(number);
                //Console.WriteLine(lat);
                double degree1 = Convert.ToDouble(lat.Substring(0, number));
                number = number + 2;
                last_n = number;
                while (true)
                {
                    //Console.WriteLine(lat.ElementAt(number));
                    if (lat.ElementAt(number) == ' ')
                        break;
                    number++;
                }
                double minutes1 = Convert.ToDouble(lat.Substring(last_n, number - last_n - 1));
                //Console.WriteLine(minutes); Console.WriteLine(minutes1);
                double total1 = ConvertDegreeAngleToDouble(degree1, minutes1, 0, lat.Substring(lat.Length - 1, 1));
                lat = System.Convert.ToString(total1);
                hold_num1[0] = total1;

                ///////////////////////just added
                number = 0;
                while(true)
                {
                    if (info.ElementAt(number) == '.')
                        break;
                    number++;
                }
                string message = info.Substring(0, number) + "...";
                ///////////////////////just added

                Console.WriteLine(name);
                Console.WriteLine(longi);
                Console.WriteLine(lat);
                Console.WriteLine(info);

                ///////////////////////just added
                Console.WriteLine(message);

                driver.Navigate().Back();
                Thread.Sleep(2000);
                Location_ a = new Location_
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    jixName = name,
                    longitude = lat,
                    lattitude = longi,
                    JIX_TELL_ABOUT = info,
                    Location = lat + "feet," + longi + "feet",
                    START_DATE = "Oct 20, 2017 at 1:50 PM",
                    currentLocation = hold_num1,

                    END_DATE = "Nil",
                    _p_FromUser = "_User$IVkaHfwjrU",
                    accessType = "Public",
                    jixCategory = "Jixwiki",
                    jixRadius = "10",
                    _p_userid = "_User$IVkaHfwjrU",
                    //jixMessage = message
                };
                collection.Save(a);
            }
   
            Console.ReadKey();
            driver.Close();
        }
    }
}
