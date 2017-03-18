namespace Zenject.Source.Binding.Binders
{

    using System;
    using System.Linq;

    using Zenject.Source.Binding.BindInfo;
    using Zenject.Source.Internal;
    using Zenject.Source.Main;

    public class ConditionBinder : CopyIntoSubContainersBinder
    {
        public ConditionBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public CopyIntoSubContainersBinder When(BindingCondition condition)
        {
            this.BindInfo.Condition = condition;
            return this;
        }

        public CopyIntoSubContainersBinder WhenInjectedIntoInstance(object instance)
        {
            this.BindInfo.Condition = r => ReferenceEquals(r.ObjectInstance, instance);
            return this;
        }

        public CopyIntoSubContainersBinder WhenInjectedInto(params Type[] targets)
        {
            this.BindInfo.Condition = r => targets.Where(x => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(x)).Any();
            return this;
        }

        public CopyIntoSubContainersBinder WhenInjectedInto<T>()
        {
            this.BindInfo.Condition = r => r.ObjectType != null && r.ObjectType.DerivesFromOrEqual(typeof(T));
            return this;
        }

        public CopyIntoSubContainersBinder WhenNotInjectedInto<T>()
        {
            this.BindInfo.Condition = r => r.ObjectType == null || !r.ObjectType.DerivesFromOrEqual(typeof(T));
            return this;
        }
    }
}
