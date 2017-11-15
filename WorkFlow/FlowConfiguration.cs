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

namespace WorkFlow
{
    public class FlowConfiguration : IDisposable
    {
        public IWebDriver WebDriver { get; set; }
        public WebDriverWait WebDriverWait { get; set; }
        public IEnumerable<Step> Steps { get; set; }
        public ILog Log { get; set; }

        public FlowConfiguration(IWebDriver webDriver, ILog log, int timeOut, string testingStepsFile)
        {
            this.WebDriver = webDriver;
            this.WebDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeOut));

            this.Log = log;
            var testingStepsContent = File.ReadAllText(testingStepsFile);
            if (!string.IsNullOrEmpty(testingStepsContent))
            {
                Steps = JsonConvert.DeserializeObject<IEnumerable<Step>>(testingStepsContent);
            }
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
