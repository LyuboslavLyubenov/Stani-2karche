namespace Assets.Zenject.Source.Binding.Finalizers
{

    using Assets.Zenject.Source.Main;

    public interface IBindingFinalizer
    {
        bool CopyIntoAllSubContainers
        {
            get;
        }

        void FinalizeBinding(DiContainer container);
    }
}
