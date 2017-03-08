namespace Assets.Zenject.Source.Validation
{

    using System;

    public class ValidationMarker
    {
        public ValidationMarker(Type markedType)
        {
            this.MarkedType = markedType;
        }

        public Type MarkedType
        {
            get;
            private set;
        }
    }
}

