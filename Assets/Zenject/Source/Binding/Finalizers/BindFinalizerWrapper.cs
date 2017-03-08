namespace Assets.Zenject.Source.Binding.Finalizers
{

    using Assets.Zenject.Source.Internal;
    using Assets.Zenject.Source.Main;

    public class BindFinalizerWrapper : IBindingFinalizer
    {
        IBindingFinalizer _subFinalizer;

        public IBindingFinalizer SubFinalizer
        {
            set
            {
                this._subFinalizer = value;
            }
        }

        public bool CopyIntoAllSubContainers
        {
            get
            {
                return this._subFinalizer.CopyIntoAllSubContainers;
            }
        }

        public void FinalizeBinding(DiContainer container)
        {
            Assert.IsNotNull(this._subFinalizer,
                "Unfinished binding! Finalizer was not given.");

            this._subFinalizer.FinalizeBinding(container);
        }
    }
}
