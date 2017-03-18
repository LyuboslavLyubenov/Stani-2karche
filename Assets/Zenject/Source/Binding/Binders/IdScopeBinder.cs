namespace Zenject.Source.Binding.Binders
{

    using Zenject.Source.Binding.BindInfo;

    public class IdScopeBinder : ScopeBinder
    {
        public IdScopeBinder(BindInfo bindInfo)
            : base(bindInfo)
        {
        }

        public ScopeBinder WithId(object identifier)
        {
            this.BindInfo.Identifier = identifier;
            return this;
        }
    }
}
