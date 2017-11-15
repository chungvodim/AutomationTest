using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlow
{
    public class Step
    {
        public string StepName { get; set; }
        public string StepLink { get; set; }
        public IEnumerable<Input> Inputs;
        public IEnumerable<Verification> Verifications;
    }

    public class Input
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string XPath { get; set; }
        public string Value { get; set; }
        public ActionType ActionType { get; set; }
    }

    public class Verification
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string XPath { get; set; }
        public string Value { get; set; }
        public ElementType ElementType { get; set; }
        public bool IsExisted { get; set; }
        public bool AreEqual { get; set; }
    }

    public enum ActionType
    {
        SendKeys,
        Select,
        Click,
    }

    public enum ElementType
    {
        WebElement,
        URL,
    }
}
