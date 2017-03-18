namespace UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    public class ColliderComparer : ComparerBaseGeneric<Bounds>
    {
        public enum CompareType
        {
            Intersects,
            DoesNotIntersect
        };

        public CompareType compareType;

        protected override bool Compare(Bounds a, Bounds b)
        {
            switch (this.compareType)
            {
                case CompareType.Intersects:
                    return a.Intersects(b);
                case CompareType.DoesNotIntersect:
                    return !a.Intersects(b);
            }
            throw new Exception();
        }
    }
}
