namespace Zenject.Source.Internal
{

    using System;

    public class ValuePair<T1, T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public ValuePair()
        {
            this.First = default(T1);
            this.Second = default(T2);
        }

        public ValuePair(T1 first, T2 second)
        {
            this.First = first;
            this.Second = second;
        }

        public override bool Equals(Object obj)
        {
            var that = obj as ValuePair<T1, T2>;

            if (that == null)
            {
                return false;
            }

            return this.Equals(that);
        }

        public bool Equals(ValuePair<T1, T2> that)
        {
            if (that == null)
            {
                return false;
            }

            return object.Equals(this.First, that.First) && object.Equals(this.Second, that.Second);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (this.First == null ? 0 : this.First.GetHashCode());
                hash = hash * 29 + (this.Second == null ? 0 : this.Second.GetHashCode());
                return hash;
            }
        }
    }

    public static class ValuePair
    {
        public static ValuePair<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            return new ValuePair<T1, T2>(first, second);
        }
    }
}
