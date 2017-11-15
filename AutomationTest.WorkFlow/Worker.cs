using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.WorkFlow
{
    public class Worker : IDisposable
    {
        FlowConfiguration _flowConfiguration;
        public Worker(FlowConfiguration flowConfiguration)
        {
            _flowConfiguration = flowConfiguration;
        }

        public void Excute()
        {
            var log = _flowConfiguration.Log;
            foreach (var step in _flowConfiguration.Steps)
            {
                StepResult stepResult = this.RunStep(step);
                if (stepResult.IsSuccess)
                {
                    log.InfoFormat("Step {0} passed: {1}", step.StepName, stepResult.Message);
                }
                else
                {
                    log.WarnFormat("Step {0} failed: {1}", step.StepName, stepResult.Message);
                    break;
                }
            }
        }

        private StepResult RunStep(Step step)
        {
            StepResult result = new StepResult();

            try
            {
                _flowConfiguration.WebDriver.Navigate().GoToUrl(step.StepLink);
                PerformInputs(step.Inputs);
                result = VerifyStep(step);
            }
            catch (Exception ex)
            {
                result = StepResult.Fail(ex.Message);
            }

            return result;
        }

        private StepResult VerifyStep(Step step)
        {
            foreach (var verification in step.Verifications)
            {
                switch (verification.ElementType)
                {
                    case ElementType.WebElement:
                        var element = GetElementWithWait(verification.ID, verification.Name, verification.XPath);
                        if ((verification.IsExisted && element == null) || (!verification.IsExisted && element != null))
                        {
                            return StepResult.Fail(string.Format("Step {0} failed!", step.StepName));
                        }
                        break;
                    case ElementType.URL:
                        if((verification.AreEqual && _flowConfiguration.WebDriver.Url != verification.Value)|| 
                            (!verification.AreEqual && _flowConfiguration.WebDriver.Url == verification.Value))
                        {
                            return StepResult.Fail(string.Format("Step {0} failed!", step.StepName));
                        }
                        break;
                    default:
                        break;
                }
            }
            return StepResult.Success(string.Format("Step {0} passed!", step.StepName));
        }

        private IWebElement GetElement(string id, string name, string xPath)
        {
            var driver = _flowConfiguration.WebDriver;
            var element = driver.FindElement(By.Id(id));
            if(element == null)
            {
                element = driver.FindElement(By.Name(name));
                if(element == null)
                {
                    element = driver.FindElement(By.XPath(xPath));
                }
            }

            return element;
        }

        private IWebElement GetElementWithWait(string id, string name, string xPath)
        {
            var waitDriver = _flowConfiguration.WebDriverWait;
            var element = waitDriver.Until(ExpectedConditions.ElementExists(By.Id(id)));
            if (element == null)
            {
                element = waitDriver.Until(ExpectedConditions.ElementExists(By.Name(name)));
                if (element == null)
                {
                    element = waitDriver.Until(ExpectedConditions.ElementExists(By.XPath(xPath)));
                }
            }

            return element;
        }

        private void PerformInputs(IEnumerable<Input> inputs)
        {
            foreach (var input in inputs)
            {
                PerformInput(input);
            }

        }

        private void PerformInput(Input input)
        {
            var driver = _flowConfiguration.WebDriver;
            IWebElement element = null;

            if (!string.IsNullOrEmpty(input.ID))
            {
                element = GetElementWithWait(input.ID, input.Name, input.XPath);
            }

            if(element == null)
            {
                throw new ArgumentException("Unable to determine input");
            }

            switch (input.ActionType)
            {
                case ActionType.SendKeys:
                    element.SendKeys(input.Value);
                    break;
                case ActionType.Select:
                    var selectElement = new SelectElement(element);
                    selectElement.SelectByValue(input.Value);
                    break;
                case ActionType.Click:
                    element.Click();
                    break;
                default:
                    break;
            }
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _flowConfiguration.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public class StepResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        public StepResult() { }
        public StepResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static StepResult Fail(string message)
        {
            return new StepResult(false, message);
        }

        public static StepResult Success(string message = "")
        {
            return new StepResult(true, message);
        }
    }
}
