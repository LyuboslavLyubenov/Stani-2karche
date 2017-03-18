namespace Zenject.Source.Util
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Zenject.Source.Injection;

    public class PostInjectableInfo
    {
        readonly MethodInfo _methodInfo;
        readonly List<InjectableInfo> _injectableInfo;

        public PostInjectableInfo(
            MethodInfo methodInfo, List<InjectableInfo> injectableInfo)
        {
            this._methodInfo = methodInfo;
            this._injectableInfo = injectableInfo;
        }

        public MethodInfo MethodInfo
        {
            get
            {
                return this._methodInfo;
            }
        }

        public IEnumerable<InjectableInfo> InjectableInfo
        {
            get
            {
                return this._injectableInfo;
            }
        }
    }

    public class ZenjectTypeInfo
    {
        readonly List<PostInjectableInfo> _postInjectMethods;
        readonly List<InjectableInfo> _constructorInjectables;
        readonly List<InjectableInfo> _fieldInjectables;
        readonly List<InjectableInfo> _propertyInjectables;
        readonly ConstructorInfo _injectConstructor;
        readonly Type _typeAnalyzed;

        public ZenjectTypeInfo(
            Type typeAnalyzed,
            List<PostInjectableInfo> postInjectMethods,
            ConstructorInfo injectConstructor,
            List<InjectableInfo> fieldInjectables,
            List<InjectableInfo> propertyInjectables,
            List<InjectableInfo> constructorInjectables)
        {
            this._postInjectMethods = postInjectMethods;
            this._fieldInjectables = fieldInjectables;
            this._propertyInjectables = propertyInjectables;
            this._constructorInjectables = constructorInjectables;
            this._injectConstructor = injectConstructor;
            this._typeAnalyzed = typeAnalyzed;
        }

        public Type Type
        {
            get
            {
                return this._typeAnalyzed;
            }
        }

        public IEnumerable<PostInjectableInfo> PostInjectMethods
        {
            get
            {
                return this._postInjectMethods;
            }
        }

        public IEnumerable<InjectableInfo> AllInjectables
        {
            get
            {
                return this._constructorInjectables.Concat(this._fieldInjectables).Concat(this._propertyInjectables).Concat(this._postInjectMethods.SelectMany(x => x.InjectableInfo));
            }
        }

        public IEnumerable<InjectableInfo> FieldInjectables
        {
            get
            {
                return this._fieldInjectables;
            }
        }

        public IEnumerable<InjectableInfo> PropertyInjectables
        {
            get
            {
                return this._propertyInjectables;
            }
        }

        public IEnumerable<InjectableInfo> ConstructorInjectables
        {
            get
            {
                return this._constructorInjectables;
            }
        }

        // May be null
        public ConstructorInfo InjectConstructor
        {
            get
            {
                return this._injectConstructor;
            }
        }
    }
}
