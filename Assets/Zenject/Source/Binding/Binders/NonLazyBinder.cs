namespace Assets.Zenject.Source.Binding.Binders
{

    using Assets.Zenject.Source.Binding.BindInfo;

    public class NonLazyBinder
    {
        public NonLazyBinder(BindInfo bindInfo)
        {
            this.BindInfo = bindInfo;
        }

        protected BindInfo BindInfo
        {
            get;
            private set;
        }

        public void NonLazy()
        {
            this.BindInfo.NonLazy = true;
        }
    }
}
