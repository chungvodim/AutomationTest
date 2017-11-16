using log4net;
using log4net.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutomationTest.WorkFlow;

namespace AutomationTest.MX
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(typeof(Program));
            Worker worker = null;
            int timeout = Convert.ToInt32(ConfigurationManager.AppSettings["timeout"]);
            try
            {
                // option headless, window-size=1200x600,..........
                args = new string[] { "-b", "chrome", "-f", "MX_Test_Login.json", "-o", "window-size=1920x1080" };
                string browser = Helper.GetParam(args, "-b");
                string filePath = Helper.GetParam(args, "-f");
                string[] options = Helper.GetParams(args, "-o");
                log.InfoFormat("Start testing testing file {0} with browser {1} and options {2}", filePath, browser, string.Join("|", options));
                //var driver = Helper.GenerateWebDriver(browser, options);
                FlowConfiguration flowConfiguration = new FlowConfiguration(browser, options, log, timeout, filePath);
                worker = new Worker(flowConfiguration);
                worker.Excute();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            finally
            {
                worker.Dispose();
            }
        }
    }
}
