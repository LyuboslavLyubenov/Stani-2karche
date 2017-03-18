namespace Zenject.Source.Providers.SubContainerCreators
{

    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Main;

    public interface ISubContainerCreator
    {
        DiContainer CreateSubContainer(List<TypeValuePair> args);
    }
}
