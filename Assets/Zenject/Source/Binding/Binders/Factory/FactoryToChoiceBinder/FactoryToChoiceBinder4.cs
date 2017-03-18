namespace Zenject.Source.Binding.Binders.Factory.FactoryToChoiceBinder
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.Binders.Factory.FactoryFromBinder;
    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Internal;

    public class FactoryToChoiceBinder<TParam1, TParam2, TParam3, TParam4, TContract> : FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract>
    {
        public FactoryToChoiceBinder(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        // Note that this is the default, so not necessary to call
        public FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> ToSelf()
        {
            Assert.IsEqual(this.BindInfo.ToChoice, ToChoices.Self);
            return this;
        }

        public FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TConcrete> To<TConcrete>()
            where TConcrete : TContract
        {
            this.BindInfo.ToChoice = ToChoices.Concrete;
            this.BindInfo.ToTypes = new List<Type>()
            {
                typeof(TConcrete)
            };

            return new FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TConcrete>(
                this.BindInfo, this.FactoryType, this.FinalizerWrapper);
        }
    }

    public class FactoryToChoiceIdBinder<TParam1, TParam2, TParam3, TParam4, TContract> : FactoryToChoiceBinder<TParam1, TParam2, TParam3, TParam4, TContract>
    {
        public FactoryToChoiceIdBinder(
            BindInfo bindInfo, Type factoryType,
            BindFinalizerWrapper finalizerWrapper)
            : base(bindInfo, factoryType, finalizerWrapper)
        {
        }

        public FactoryToChoiceBinder<TParam1, TParam2, TParam3, TParam4, TContract> WithId(object identifier)
        {
            this.BindInfo.Identifier = identifier;
            return this;
        }
    }
}
