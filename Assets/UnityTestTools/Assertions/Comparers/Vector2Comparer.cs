namespace UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class Vector2Comparer : VectorComparerBase<Vector2>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public float floatingPointError = 0.0001f;

        protected override bool Compare(Vector2 a, Vector2 b)
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
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
