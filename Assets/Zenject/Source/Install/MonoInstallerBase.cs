#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Install
{

    using System;

    using Assets.Zenject.Source.Main;
    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    // We'd prefer to make this abstract but Unity 5.3.5 has a bug where references
    // can get lost during compile errors for classes that are abstract
    [System.Diagnostics.DebuggerStepThrough]
    public class MonoInstallerBase : MonoBehaviour, IInstaller
    {
        [Inject]
        DiContainer _container = null;

        protected DiContainer Container
        {
            get
            {
                return this._container;
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                return this.enabled;
            }
        }

        public virtual void Start()
        {
            // Define this method so we expose the enabled check box
        }

        public virtual void InstallBindings()
        {
            throw new NotImplementedException();
        }
    }
}

#endif

