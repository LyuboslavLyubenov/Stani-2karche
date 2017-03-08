namespace Assets.Zenject.Source.Providers.SubContainerCreators
{

    using System.Collections.Generic;

    using Assets.Zenject.Source.Injection;
    using Assets.Zenject.Source.Main;

    public interface ISubContainerCreator
    {
        DiContainer CreateSubContainer(List<TypeValuePair> args);
    }
}
