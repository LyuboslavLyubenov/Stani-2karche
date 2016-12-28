namespace Assets.Scripts.Utils
{

    public class ValueWrapper<T>
    {

        public T Value
        { 
            get; 
            set; 
        }

        public ValueWrapper()
        {
        }

        public ValueWrapper(T value)
        { 
            this.Value = value;
        }

        public static implicit operator T(ValueWrapper<T> wrapper)
        {
            if (wrapper == null)
            {
                return default(T);
            }

            return wrapper.Value;
        }

    }

}