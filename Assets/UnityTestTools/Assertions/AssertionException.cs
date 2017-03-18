namespace UnityTestTools.Assertions
{

    using System;

    public class AssertionException : Exception
    {
        private readonly AssertionComponent m_Assertion;

        public AssertionException(AssertionComponent assertion) : base(assertion.Action.GetFailureMessage())
        {
            this.m_Assertion = assertion;
        }

        public override string StackTrace
        {
            get
            {
                return "Created in " + this.m_Assertion.GetCreationLocation();
            }
        }
    }
}
