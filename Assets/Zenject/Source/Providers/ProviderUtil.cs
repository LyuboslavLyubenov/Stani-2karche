namespace Assets.Zenject.Source.Providers
{

    using System;

    using Assets.Zenject.Source.Internal;

    public static class ProviderUtil
    {
        public static Type GetTypeToInstantiate(Type contractType, Type concreteType)
        {
            if (concreteType.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract());
                Assert.That(contractType.GetGenericTypeDefinition() == concreteType);
                return contractType;
            }

            Assert.DerivesFromOrEqual(concreteType, contractType);
            return concreteType;
        }
    }
}

