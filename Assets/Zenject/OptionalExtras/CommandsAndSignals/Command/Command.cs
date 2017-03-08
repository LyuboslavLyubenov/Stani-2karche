namespace Assets.Zenject.OptionalExtras.CommandsAndSignals.Command
{

    using System;

    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Usage;

    public interface ICommand
    {
    }

    // Zero params
    [ZenjectAllowDuringValidation]
    public abstract class Command : ICommand
    {
        Action _handler;

        [Inject]
        public void Construct(Action handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler();
            }
        }

        public void Execute()
        {
            this._handler();
        }
    }

    // One param
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1> : ICommand
    {
        Action<TParam1> _handler;

        [Inject]
        public void Construct(Action<TParam1> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1));
            }
        }

        public void Execute(TParam1 param1)
        {
            this._handler(param1);
        }
    }

    // Two params
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1, TParam2> : ICommand
    {
        Action<TParam1, TParam2> _handler;

        [Inject]
        public void Construct(Action<TParam1, TParam2> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1), default(TParam2));
            }
        }

        public void Execute(TParam1 param1, TParam2 param2)
        {
            this._handler(param1, param2);
        }
    }

    // Three params
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1, TParam2, TParam3> : ICommand
    {
        Action<TParam1, TParam2, TParam3> _handler;

        [Inject]
        public void Construct(Action<TParam1, TParam2, TParam3> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1), default(TParam2), default(TParam3));
            }
        }

        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            this._handler(param1, param2, param3);
        }
    }

    // Four params
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1, TParam2, TParam3, TParam4> : ICommand
    {
        Action<TParam1, TParam2, TParam3, TParam4> _handler;

        [Inject]
        public void Construct(Action<TParam1, TParam2, TParam3, TParam4> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1), default(TParam2), default(TParam3), default(TParam4));
            }
        }

        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            this._handler(param1, param2, param3, param4);
        }
    }

    // Five params
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1, TParam2, TParam3, TParam4, TParam5> : ICommand
    {
        Action<TParam1, TParam2, TParam3, TParam4, TParam5> _handler;

        [Inject]
        public void Construct(Action<TParam1, TParam2, TParam3, TParam4, TParam5> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5));
            }
        }

        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            this._handler(param1, param2, param3, param4, param5);
        }
    }

    // Six params
    [ZenjectAllowDuringValidation]
    public abstract class Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : ICommand
    {
        Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _handler;

        [Inject]
        public void Construct(Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> handler, DiContainer container)
        {
            this._handler = handler;

            if (container.IsValidating)
            // During validation, we want to instantiate whatever class we're calling immediately
            {
                handler(default(TParam1), default(TParam2), default(TParam3), default(TParam4), default(TParam5), default(TParam6));
            }
        }

        public void Execute(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            this._handler(param1, param2, param3, param4, param5, param6);
        }
    }
}
