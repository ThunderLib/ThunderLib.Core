namespace ThunderLib.Core.RegistrySystem
{
    using System;
    using System.Runtime.CompilerServices;


    internal static partial class _Module
    {
        [ModuleInitializer]
        internal static void Init()
        {
            
        }

    }
}

//Need to forward declare the moduleinitializer attribute
//It is a compiler only attribute so this will have zero impact at runtime
//Tested and verified that forward declaration works
//Better to leave as internal to avoid aliasing issues in other assemblies that forward declare it.
namespace System.Runtime.CompilerServices
{
    using System;
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class ModuleInitializerAttribute : Attribute { }
}
