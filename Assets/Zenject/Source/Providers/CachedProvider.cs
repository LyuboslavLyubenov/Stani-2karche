namespace Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;

    public class CachedProvider : IProvider
    {
        readonly IProvider _creator;

        List<object> _instances;
        bool _isCreatingInstance;

        public CachedProvider(IProvider creator)
        {
            this._creator = creator;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._creator.GetInstanceType(context);
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            if (this._instances != null)
            {
                yield return this._instances;
                yield break;
            }

            // This should only happen with constructor injection
            // Field or property injection should allow circular dependencies
            Assert.That(!this._isCreatingInstance,
            "Found circular dependency when creating type '{0}'",
            this._creator.GetInstanceType(context));

            this._isCreatingInstance = true;

            var runner = this._creator.GetAllInstancesWithInjectSplit(context, args);

            // First get instance
            bool hasMore = runner.MoveNext();

            this._instances = runner.Current;
            Assert.IsNotNull(this._instances);
            this._isCreatingInstance = false;

            yield return this._instances;

            // Now do injection
            while (hasMore)
            {
                hasMore = runner.MoveNext();
            }
        }
    }
}
