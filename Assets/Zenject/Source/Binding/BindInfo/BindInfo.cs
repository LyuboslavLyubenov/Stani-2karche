namespace Zenject.Source.Binding.BindInfo
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Injection;
    using Zenject.Source.Main;

    public enum ScopeTypes
    {
        Transient,
        Singleton,
        Cached,
    }

    public enum ToChoices
    {
        Self,
        Concrete,
    }

    public enum InvalidBindResponses
    {
        Assert,
        Skip,
    }

    public class BindInfo
    {
        public BindInfo(List<Type> contractTypes)
        {
            this.Identifier = null;
            this.ContractTypes = contractTypes;
            this.ToTypes = new List<Type>();
            this.Arguments = new List<TypeValuePair>();
            this.ToChoice = ToChoices.Self;
            this.CopyIntoAllSubContainers = false;
            this.NonLazy = false;
            this.Scope = ScopeTypes.Transient;
            this.InvalidBindResponse = InvalidBindResponses.Assert;
        }

        public BindInfo(Type contractType)
            : this(new List<Type>() { contractType } )
        {
        }

        public BindInfo()
            : this(new List<Type>())
        {
        }

        public object Identifier
        {
            get;
            set;
        }

        public List<Type> ContractTypes
        {
            get;
            set;
        }

        public bool CopyIntoAllSubContainers
        {
            get;
            set;
        }

        public InvalidBindResponses InvalidBindResponse
        {
            get;
            set;
        }

        public bool NonLazy
        {
            get;
            set;
        }

        public BindingCondition Condition
        {
            get;
            set;
        }

        public ToChoices ToChoice
        {
            get;
            set;
        }

        // Only relevant with ToChoices.Concrete
        public List<Type> ToTypes
        {
            get;
            set;
        }

        public ScopeTypes Scope
        {
            get;
            set;
        }

        // Note: This only makes sense for ScopeTypes.Singleton
        public object ConcreteIdentifier
        {
            get;
            set;
        }

        public List<TypeValuePair> Arguments
        {
            get;
            set;
        }
    }
}
