namespace ThunderLib.Core.ModuleSystem
{
    using System;

    using ThunderLib.Core.RegistrySystem;


    //TODO: There needs to be a phased init process for registry system. Modules need to init before user code, but other registries need to init after user code.
    public class ModuleRegistry : Registry<ModuleRegistry, Module>
    {
        protected override void OnInitFinished()
        {
            base.OnInitFinished();

            foreach(var d in ModuleRegistry.backend.EnumerateElements())
            {
                var def = d?.regToken?.def;
                if(def is null) continue;
                if(def.shouldEnable)
                {
                    def.Enable();
                }
            }
        }
    }

    internal static class RequestedModuleLoads
    {
        internal static void Request<T>()
        {
            Storage<T>.shouldEnable = true;
        }

        internal static Boolean Requested<T>()
        {
            return Storage<T>.shouldEnable;
        }

        private static class Storage<T>
        {
            internal static Boolean shouldEnable = false;
        }
    }


    public abstract class Module : ModuleRegistry.IRegistryDef
    {
        public abstract String guid { get; }
        protected internal abstract void Enable();
        protected internal abstract void Disable();
        
        internal Boolean _loaded { get; set; }



        internal abstract Boolean shouldEnable { get; }

        //TODO: See if there is a way to handle this without using a null forgiven null or suppressing the warning
        public ModuleRegistry.RegistrationToken regToken { get; set; } = null!;
    }


    public abstract class Module<TSelf> : Module
        where TSelf : Module<TSelf>
    {
        public static void RequestLoad()
        {
            if(!instance.regToken.isRegistered)
            {
                instance.regToken.Register();
            }

            RequestedModuleLoads.Request<TSelf>();
        }


        public static Boolean loaded => instance._loaded;


        internal sealed override Boolean shouldEnable => RequestedModuleLoads.Requested<TSelf>();
        private static TSelf instance = null!;
        private Module()
        {
            //TODO: Exception message
            if(instance is not TSelf) throw new InvalidOperationException();
            instance = (this as TSelf)!;
        }
    }
}
