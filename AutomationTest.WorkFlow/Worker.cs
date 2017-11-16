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
                    log.InfoFormat(stepResult.Message);
                }
                else
                {
                    log.WarnFormat(stepResult.Message);
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
                if (verification.IsVisible.HasValue)
                {
                    var element = GetElementWithWait(verification);
                    if (element != null && element.Displayed != verification.IsVisible.Value)
                    {
                        return StepResult.Fail(string.Format("element visibility is not match"));
                    }
                }

                if (verification.IsExisted.HasValue)
                {
                    var element = GetElementWithWait(verification);
                    if ((element != null) != verification.IsExisted.Value)
                    {
                        return StepResult.Fail(string.Format("element existence is not match"));
                    }
                }

                if (verification.AreEqual.HasValue)
                {
                    if ((_flowConfiguration.WebDriver.Url == verification.Value) != verification.AreEqual.Value)
                    {
                        return StepResult.Fail(string.Format("values are not match"));
                    }
                }
            }
            return StepResult.Success(string.Format("Step {0} passed!", step.StepName));
        }

        private IWebElement GetElement(string id, string name, string xPath, string className)
        {
            var driver = _flowConfiguration.WebDriver;
            IWebElement element = null;
            if (!string.IsNullOrWhiteSpace(id))
            {
                element = driver.FindElement(By.Id(id));
            }
            if (element == null)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    element = driver.FindElement(By.Name(name));
                }
                if (element == null)
                {
                    if (!string.IsNullOrWhiteSpace(xPath))
                    {
                        element = driver.FindElement(By.XPath(xPath));
                    }
                    if (element == null)
                    {
                        if (!string.IsNullOrWhiteSpace(className))
                        {
                            element = driver.FindElement(By.ClassName(className));
                        }
                    }
                }
            }

            return element;
        }

        private IWebElement GetElementWithWait(Input input)
        {
            return GetElementWithWait(input.ID, input.Name, input.XPath, input.Class);
        }

        private IWebElement GetElementWithWait(Verification verification)
        {
            return GetElementWithWait(verification.ID, verification.Name, verification.XPath, verification.Class);
        }

        private IWebElement GetElementWithWait(string id, string name, string xPath, string className)
        {
            var waitDriver = _flowConfiguration.WebDriverWait;
            IWebElement element = null;
            if (!string.IsNullOrWhiteSpace(id))
            {
                element = waitDriver.Until(ExpectedConditions.ElementExists(By.Id(id)));
            }
            if (element == null)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    element = waitDriver.Until(ExpectedConditions.ElementExists(By.Name(name)));
                }
                if (element == null)
                {
                    if (!string.IsNullOrWhiteSpace(xPath))
                    {
                        element = waitDriver.Until(ExpectedConditions.ElementExists(By.XPath(xPath)));
                    }
                    if (element == null)
                    {
                        if (!string.IsNullOrWhiteSpace(className))
                        {
                            element = waitDriver.Until(ExpectedConditions.ElementExists(By.ClassName(className)));
                        }
                    }
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
            var javaScriptExecutor = _flowConfiguration.JavaScriptExecutor;
            IWebElement element = GetElementWithWait(input);

            if (element == null)
            {
                throw new ArgumentException($"Unable to determine input: ID-{input.ID}, Name-{input.Name}, XPath-{input.XPath} ");
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
                case ActionType.Check:
                    var child = element.FindElement(By.XPath("//input[@value='" + input.Value + "']"));
                    if(child.Displayed)
                    {
                        child.Click();
                    }
                    else
                    {
                        javaScriptExecutor.ExecuteScript("$('input[Value="+ input.Value + "]').click()");
                    }
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
