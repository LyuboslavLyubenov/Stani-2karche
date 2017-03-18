namespace Zenject.OptionalExtras.CommandsAndSignals.Command.Binders
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Binding.Finalizers;
    using Zenject.Source.Main;

    public class CommandBinderBase<TCommand, TAction>
        where TCommand : ICommand
        where TAction : class
    {
        readonly DiContainer _container;
        readonly BindFinalizerWrapper _finalizerWrapper;
        readonly BindInfo _bindInfo;

        public CommandBinderBase(string identifier, DiContainer container)
        {
            this._container = container;

            this._bindInfo = new BindInfo();
            this._bindInfo.Identifier = identifier;
            this._bindInfo.ContractTypes = new List<Type>()
                {
                    typeof(TCommand),
                };

            this._finalizerWrapper = container.StartBinding();
        }

        protected BindInfo BindInfo
        {
            get
            {
                return this._bindInfo;
            }
        }

        protected IBindingFinalizer Finalizer
        {
            set
            {
                this._finalizerWrapper.SubFinalizer = value;
            }
        }

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }
    }
}

