namespace UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class Vector4Comparer : VectorComparerBase<Vector4>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public double floatingPointError;

        protected override bool Compare(Vector4 a, Vector4 b)
        {
            switch (this.compareType)
            {
                case CompareType.MagnitudeEquals:
                    return this.AreVectorMagnitudeEqual(a.magnitude,
                                                   b.magnitude,
                                                   this.floatingPointError);
                case CompareType.MagnitudeNotEquals:
                    return !this.AreVectorMagnitudeEqual(a.magnitude,
                                                    b.magnitude,
                                                    this.floatingPointError);
            }
            throw new Exception();
        }
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
