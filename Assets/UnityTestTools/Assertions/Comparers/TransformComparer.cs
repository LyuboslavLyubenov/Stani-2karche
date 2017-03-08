namespace Assets.UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class TransformComparer : ComparerBaseGeneric<Transform>
    {
        public enum CompareType { Equals, NotEquals }

        public CompareType compareType;

        protected override bool Compare(Transform a, Transform b)
        {
            if (this.compareType == CompareType.Equals)
            {
                return a.position == b.position;
            }
            if (this.compareType == CompareType.NotEquals)
            {
                return a.position != b.position;
            }
            throw new Exception();
        }
    }
}
