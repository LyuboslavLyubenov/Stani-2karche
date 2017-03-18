namespace Zenject.Source.Providers
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;
    using Zenject.Source.Providers.SubContainerCreators;
    using Zenject.Source.Usage;

    public class SubContainerDependencyProvider : IProvider
    {
        readonly ISubContainerCreator _subContainerCreator;
        readonly Type _dependencyType;
        readonly object _identifier;

        // if concreteType is null we use the contract type from inject context
        public SubContainerDependencyProvider(
            Type dependencyType,
            object identifier,
            ISubContainerCreator subContainerCreator)
        {
            this._subContainerCreator = subContainerCreator;
            this._dependencyType = dependencyType;
            this._identifier = identifier;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return this._dependencyType;
        }

        InjectContext CreateSubContext(
            InjectContext parent, DiContainer subContainer)
        {
            var subContext = parent.CreateSubContext(this._dependencyType, this._identifier);

            subContext.Container = subContainer;

            // This is important to avoid infinite loops
            subContext.SourceType = InjectSources.Local;

            return subContext;
        }

        public IEnumerator<List<object>> GetAllInstancesWithInjectSplit(InjectContext context, List<TypeValuePair> args)
        {
            Assert.IsNotNull(context);

            var subContainer = this._subContainerCreator.CreateSubContainer(args);

            var subContext = this.CreateSubContext(context, subContainer);

            yield return subContainer.ResolveAll(subContext).Cast<object>().ToList();
        }
    }
}
