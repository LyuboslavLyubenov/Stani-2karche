namespace UnityTestTools.Assertions.Comparers
{

    using System;

    using UnityEngine;

    using Object = System.Object;

    public abstract class ComparerBase : ActionBase
    {
        public enum CompareToType
        {
            CompareToObject,
            CompareToConstantValue,
            CompareToNull
        }

        public CompareToType compareToType = CompareToType.CompareToObject;

        public GameObject other;
        protected object m_ObjOtherVal;
        public string otherPropertyPath = "";
        private MemberResolver m_MemberResolverB;

        protected abstract bool Compare(object a, object b);

        protected override bool Compare(object objValue)
        {
            if (this.compareToType == CompareToType.CompareToConstantValue)
            {
                this.m_ObjOtherVal = this.ConstValue;
            }
            else if (this.compareToType == CompareToType.CompareToNull)
            {
                this.m_ObjOtherVal = null;
            }
            else
            {
                if (this.other == null)
                    this.m_ObjOtherVal = null;
                else
                {
                    if (this.m_MemberResolverB == null)
                        this.m_MemberResolverB = new MemberResolver(this.other, this.otherPropertyPath);
                    this.m_ObjOtherVal = this.m_MemberResolverB.GetValue(this.UseCache);
                }
            }
            return this.Compare(objValue, this.m_ObjOtherVal);
        }

        public virtual Type[] GetAccepatbleTypesForB()
        {
            return null;
        }

        #region Const value

        public virtual object ConstValue { get; set; }
        public virtual object GetDefaultConstValue()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string GetFailureMessage()
        {
            var message = this.GetType().Name + " assertion failed.\n" + this.go.name + "." + this.thisPropertyPath + " " + this.compareToType;
            switch (this.compareToType)
            {
                case CompareToType.CompareToObject:
                    message += " (" + this.other + ")." + this.otherPropertyPath + " failed.";
                    break;
                case CompareToType.CompareToConstantValue:
                    message += " " + this.ConstValue + " failed.";
                    break;
                case CompareToType.CompareToNull:
                    message += " failed.";
                    break;
            }
            message += " Expected: " + this.m_ObjOtherVal + " Actual: " + this.m_ObjVal;
            return message;
        }
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T, T>
    {
    }

    [Serializable]
    public abstract class ComparerBaseGeneric<T1, T2> : ComparerBase
    {
        public T2 constantValueGeneric = default(T2);

        public override Object ConstValue
        {
            get
            {
                return this.constantValueGeneric;
            }
            set
            {
                this.constantValueGeneric = (T2)value;
            }
        }

        public override Object GetDefaultConstValue()
        {
            return default(T2);
        }

        static bool IsValueType(Type type)
        {
#if !UNITY_METRO
            return type.IsValueType;
#else
            return false;
#endif
        }

        protected override bool Compare(object a, object b)
        {
            var type = typeof(T2);
            if (b == null && IsValueType(type))
            {
                throw new ArgumentException("Null was passed to a value-type argument");
            }
            return this.Compare((T1)a, (T2)b);
        }

        protected abstract bool Compare(T1 a, T2 b);

        public override Type[] GetAccepatbleTypesForA()
        {
            return new[] { typeof(T1) };
        }

        public override Type[] GetAccepatbleTypesForB()
        {
            return new[] {typeof(T2)};
        }

        protected override bool UseCache { get { return true; } }
    }
}
