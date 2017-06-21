namespace Assets.Scripts.Extensions
{

    using System;
    using System.Linq;

    public static class ObjectExtensions
    {
        public static bool IsImplemetingInterface(this object obj, Type interfaceType)
        {
            return obj.GetType()
                .GetInterfaces()
                .Contains(interfaceType);
        }

        public static bool IsImplementingInterface<T>(this object obj)
        {
            return obj.IsImplemetingInterface(typeof(T));
        }
    }
}
