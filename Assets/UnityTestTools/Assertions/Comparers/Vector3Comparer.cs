namespace Assets.UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class Vector3Comparer : VectorComparerBase<Vector3>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public double floatingPointError = 0.0001f;

        protected override bool Compare(Vector3 a, Vector3 b)
        {
            switch (this.compareType)
            {
                case CompareType.MagnitudeEquals:
                    return this.AreVectorMagnitudeEqual(a.magnitude,
                                                   b.magnitude, this.floatingPointError);
                case CompareType.MagnitudeNotEquals:
                    return !this.AreVectorMagnitudeEqual(a.magnitude,
                                                    b.magnitude, this.floatingPointError);
            }
            throw new Exception();
        }
    }
}
