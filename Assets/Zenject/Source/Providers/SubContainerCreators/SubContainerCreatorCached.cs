namespace Zenject.Source.Providers.SubContainerCreators
{

    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class SubContainerCreatorCached : ISubContainerCreator
    {
        readonly ISubContainerCreator _subCreator;

        DiContainer _subContainer;

        public SubContainerCreatorCached(ISubContainerCreator subCreator)
        {
            this._subCreator = subCreator;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args)
        {
            // We can't really support arguments if we are using the cached value since
            // the arguments might change when called after the first time
            Assert.IsEmpty(args);

            if (this._subContainer == null)
            {
                this._subContainer = this._subCreator.CreateSubContainer(new List<TypeValuePair>());
                Assert.IsNotNull(this._subContainer);
            }

            return this._subContainer;
        }
    }
}
