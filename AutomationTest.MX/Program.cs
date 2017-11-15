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
using WorkFlow;

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
                FlowConfiguration flowConfiguration = new FlowConfiguration(new ChromeDriver(), log, timeout, "MX_Test_Login.json");
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
