using log4net;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.WorkFlow
{
    public class FlowConfiguration : IDisposable
    {
        public IJavaScriptExecutor JavaScriptExecutor { get; set; }
        public IWebDriver WebDriver { get; set; }
        public WebDriverWait WebDriverWait { get; set; }
        public IEnumerable<Step> Steps { get; set; }
        public ILog Log { get; set; }

        private FlowConfiguration(ILog log, string testingStepsFile)
        {
            this.Log = log;
            var testingStepsContent = File.ReadAllText(testingStepsFile);
            if (!string.IsNullOrEmpty(testingStepsContent))
            {
                Steps = JsonConvert.DeserializeObject<IEnumerable<Step>>(testingStepsContent);
            }
        }

        public FlowConfiguration(IWebDriver webDriver, ILog log, int timeOut, string testingStepsFile) : this(log, testingStepsFile)
        {
            this.WebDriver = webDriver;
            this.WebDriverWait = new WebDriverWait(this.WebDriver, TimeSpan.FromSeconds(timeOut));
            this.JavaScriptExecutor = (IJavaScriptExecutor)this.WebDriver;
        }

        public FlowConfiguration(string browser, string[] options, ILog log, int timeOut, string testingStepsFile) : this(log, testingStepsFile)
        {
            this.WebDriver = Helper.GenerateWebDriver(browser, options);
            this.WebDriverWait = new WebDriverWait(this.WebDriver, TimeSpan.FromSeconds(timeOut));
            this.JavaScriptExecutor = (IJavaScriptExecutor)this.WebDriver;
        }

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    WebDriver.Quit();
                }
                disposed = true;
            }
        }
    }
}
