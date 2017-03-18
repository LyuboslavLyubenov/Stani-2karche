namespace UnityTestTools.Assertions.Comparers
{

    using System;

    public class GeneralComparer : ComparerBase
    {
        public enum CompareType { AEqualsB, ANotEqualsB }

        public CompareType compareType;

        protected override bool Compare(object a, object b)
        {
            if (this.compareType == CompareType.AEqualsB)
                return a.Equals(b);
            if (this.compareType == CompareType.ANotEqualsB)
                return !a.Equals(b);
            throw new Exception();
        }
    }
}
