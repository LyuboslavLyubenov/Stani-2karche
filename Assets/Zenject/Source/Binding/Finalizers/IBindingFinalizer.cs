namespace Zenject.Source.Binding.Finalizers
{

    using Zenject.Source.Main;

    public interface IBindingFinalizer
    {
        bool CopyIntoAllSubContainers
        {
            get;
        }

        void FinalizeBinding(DiContainer container);
    }
}
