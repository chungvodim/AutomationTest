using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationTest.WorkFlow
{
    public class Helper
    {
        public static void WaitCancelKey(CancellationTokenSource tokenSource)
        {
            while (true)
            {
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'c' || ch == 'C')
                {
                    tokenSource.Cancel();
                    Console.WriteLine("\nTask cancellation requested.");
                    break;
                }
            }
            //Console.ReadLine();
        }

        public static void WaitCancelKey()
        {
            Console.WriteLine("Waiting all tasks");
            while (true)
            {
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'c' || ch == 'C')
                {
                    Console.WriteLine("\nTask cancellation requested.");
                    break;
                }
            }
            //Console.ReadLine();
        }
        /// <summary>
        /// Get command date time from arguments
        /// </summary>
        /// <param name="args">default DateTime.UtcNo</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string[] args, string key = "-d")
        {
            var dateTime = DateTime.UtcNow;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if (arg == key)
                    {
                        try
                        {
                            var date = args[i + 1];
                            var time = "00:00:00";
                            if (args.Length > i + 2 && args[i + 2][0] != '-')
                            {
                                time = args[i + 2];
                            }
                            dateTime = DateTime.ParseExact(string.Format("{0} {1}", date, time), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to parse date, please use correct format:{0}", ex.Message);
                        }

                        break;
                    }
                }
            }

            return dateTime;
        }
        /// <summary>
        /// Get command type from arguments
        /// </summary>
        /// <param name="args">default empty</param>
        /// <returns></returns>
        public static string GetParam(string[] args, string key = "-t")
        {
            var result = string.Empty;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if (arg == key)
                    {
                        try
                        {
                            var param = args[i + 1].Trim().ToLower();
                            if(!string.IsNullOrEmpty(param) && !param.StartsWith("-"))
                            {
                                result = param;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("please use correct format {0} [type]:{1}", key, ex.Message);
                        }

                        break;
                    }
                }
            }

            return result;
        }

        public static string[] GetParams(string[] args, string key = "-t")
        {
            List<string> parameters = new List<string>();

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if (arg == key)
                    {
                        try
                        {
                            bool isNotLast = true;
                            var index = i;
                            while (isNotLast)
                            {
                                try
                                {
                                    index++;
                                    var parameter = args[index].Trim().ToLower();
                                    if (!string.IsNullOrEmpty(parameter) && !parameter.StartsWith("-"))
                                    {
                                        parameters.Add(parameter);
                                    }
                                    else
                                    {
                                        isNotLast = false;
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    isNotLast = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("please use correct format {0} [type]:{1}", key, ex.Message);
                        }

                        break;
                    }
                }
            }

            return parameters.ToArray();
        }

        public static IWebDriver GenerateWebDriver(string browser, params string[] browserOptions)
        {
            IWebDriver driver = null;
            browser = browser.ToLower();

            if (browser == "chrome" || string.IsNullOrWhiteSpace(browser))
            {
                var options = new ChromeOptions();
                options.AddArguments(browserOptions);
                driver = new ChromeDriver(options);
            }

            if (browser == "firefox")
            {
                var options = new FirefoxOptions();
                options.AddArguments(browserOptions);
                driver = new FirefoxDriver(options);
            }

            if (browser == "ie")
            {
                driver = new InternetExplorerDriver();
            }

            if (browser == "edge")
            {
                driver = new EdgeDriver();
            }

            if (browser == "phantomjs")
            {
                driver = new PhantomJSDriver();
            }

            return driver;
        }
    }
}
