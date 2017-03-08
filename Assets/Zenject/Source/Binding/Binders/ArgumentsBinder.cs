namespace Assets.Zenject.Source.Binding.Binders
{

    using System.Collections.Generic;
    using System.Linq;

    using Assets.Zenject.Source.Binding.BindInfo;
    using Assets.Zenject.Source.Injection;

    public class ArgumentsBinder : ConditionBinder
    {
        public ArgumentsBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ConditionBinder WithArguments(params object[] args)
        {
            this.BindInfo.Arguments = InjectUtil.CreateArgList(args);
            return this;
        }

        public ConditionBinder WithArgumentsExplicit(IEnumerable<TypeValuePair> extraArgs)
        {
            this.BindInfo.Arguments = extraArgs.ToList();
            return this;
        }
    }
}
