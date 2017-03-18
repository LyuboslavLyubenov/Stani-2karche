namespace Zenject.Source.Usage
{

    using System;

    // Add this to the classes that you want to allow being created during validation
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ZenjectAllowDuringValidationAttribute : Attribute
    {
    }
}
