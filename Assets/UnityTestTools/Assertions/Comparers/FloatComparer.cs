namespace Assets.UnityTestTools.Assertions.Comparers
{

    using System;

    public class FloatComparer : ComparerBaseGeneric<float>
    {
        public enum CompareTypes
        {
            Equal,
            NotEqual,
            Greater,
            Less
        }

        public CompareTypes compareTypes;
        public double floatingPointError = 0.0001f;

        protected override bool Compare(float a, float b)
        {
            switch (this.compareTypes)
            {
                case CompareTypes.Equal:
                    return Math.Abs(a - b) < this.floatingPointError;
                case CompareTypes.NotEqual:
                    return Math.Abs(a - b) > this.floatingPointError;
                case CompareTypes.Greater:
                    return a > b;
                case CompareTypes.Less:
                    return a < b;
            }
            throw new Exception();
        }
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
