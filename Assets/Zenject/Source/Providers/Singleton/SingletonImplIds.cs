namespace Zenject.Source.Providers.Singleton
{

    using System;

    using Zenject.Source.Internal;

    public static class SingletonImplIds
    {
        public class ToMethod
        {
            readonly Delegate _method;

            public ToMethod(Delegate method)
            {
                this._method = method;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 29 + this._method.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object otherObj)
            {
                var other = otherObj as ToMethod;

                if (other == null)
                {
                    return false;
                }

                return this._method.Target == other._method.Target && this._method.Method() == other._method.Method();
            }
        }

        public class ToGetter
        {
            readonly Delegate _method;
            readonly object _identifier;

            public ToGetter(object identifier, Delegate method)
            {
                this._method = method;
                this._identifier = identifier;
            }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 29 + this._method.GetHashCode();
                    hash = hash * 29 + (this._identifier == null ? 0 : this._identifier.GetHashCode());
                    return hash;
                }
            }

            public override bool Equals(object otherObj)
            {
                var other = otherObj as ToGetter;

                if (other == null)
                {
                    return false;
                }

                return object.Equals(this._identifier, other._identifier)
                    && this._method.Target == other._method.Target 
                    && this._method.Method() == other._method.Method();
            }
        }
    }
}
