namespace Assets.Zenject.Source.Binding.Binders
{

    using Assets.Zenject.Source.Binding.BindInfo;

    public class CopyIntoSubContainersBinder : NonLazyBinder
    {
        public CopyIntoSubContainersBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public NonLazyBinder CopyIntoAllSubContainers()
        {
            this.BindInfo.CopyIntoAllSubContainers = true;
            return this;
        }

        // Would this be useful?
        //public NonLazyBinder CopyIntoDirectSubContainers()
    }
}
