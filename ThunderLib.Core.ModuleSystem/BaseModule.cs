namespace ThunderLib.Core.ModuleSystem
{
    using System;

    using ThunderLib.Core.RegistrySystem;


    //TODO: There needs to be a phased init process for registry system. Modules need to init before user code, but other registries need to init after user code.
    public class ModuleRegistry : Registry<ModuleRegistry, Module>
    {

    }


    public abstract class Module : ModuleRegistry.IRegistryDef
    {
        public abstract String guid { get; }
        protected abstract void Enable();
        protected abstract void Disable();
        
        internal Boolean _loaded { get; set; }




        //TODO: See if there is a way to handle this without using a null forgiven null or suppressing the warning
        public ModuleRegistry.RegistrationToken regToken { get; set; } = null!;
    }


    public abstract class Module<TSelf> : Module
        where TSelf : Module<TSelf>
    {
        public static Boolean loaded => instance._loaded;


        private static TSelf instance = null!;
        private Module()
        {
            //TODO: Exception message
            if(instance is not TSelf) throw new InvalidOperationException();
            instance = (this as TSelf)!;
        }
    }
}
