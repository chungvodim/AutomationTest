using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.WorkFlow
{
    public class Step
    {
        public string StepName { get; set; }
        public string StepLink { get; set; }
        public IEnumerable<Input> Inputs;
        public IEnumerable<Verification> Verifications;
    }

    public class Element
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string XPath { get; set; }
        public string Class { get; set; }
        public string Value { get; set; }
    }

    public class Input : Element
    {
        public bool? IsVisible { get; set; }
        public ActionType ActionType { get; set; }
    }

    public class Verification : Element
    {
        public VerificationType VerificationType { get; set; }
        public bool? IsVisible { get; set; }
        public bool? IsExisted { get; set; }
        public bool? AreEqual { get; set; }
    }

    public enum ActionType
    {
        SendKeys,
        Select,
        Check,
        Click,
    }

    public enum VerificationType
    {
        Element,
        URL,
    }
}
