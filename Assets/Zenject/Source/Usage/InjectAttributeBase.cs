namespace Assets.Zenject.Source.Usage
{

    using Assets.Zenject.Source.Internal;

    public abstract class InjectAttributeBase : PreserveAttribute
    {
        public bool Optional
        {
            get;
            set;
        }

        public object Id
        {
            get;
            set;
        }

        public InjectSources Source
        {
            get;
            set;
        }
    }
}
