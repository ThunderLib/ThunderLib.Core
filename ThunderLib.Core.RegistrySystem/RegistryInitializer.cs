using System;

namespace ThunderLib.Core.RegistrySystem
{
    public static class RegistryInitializer
    {
        public static void Init<TRegistry, TDef, TBackend>(Boolean registerImmediately)
            where TRegistry : Registry<TRegistry, TDef, TBackend>, new()
            where TDef : Registry<TRegistry, TDef, TBackend>.IRegistryDef
            where TBackend : struct, Registry<TRegistry, TDef, TBackend>.IRegistryBackend
        {
            MetaRegistry.CreateIfNeeded();
            //MetaRegistry is given special treatment because registering it with itself would mean adding numerous checks within it to ensure it does not initialize itself.
            //While special treatment is bad, this case is a decent exception to the rule.
            if(typeof(TRegistry) == typeof(MetaRegistry)) return;
            try
            {
                var cat = new TRegistry();
                cat.regToken = MetaRegistry.Add(cat);
                if(registerImmediately)
                {
                    cat.regToken.Register();
                }
            } catch { }
            //TODO: This does not log any caught errors



        }

        public static void Init<TRegistry, TDef, TBackend>()
            where TRegistry : Registry<TRegistry, TDef, TBackend>, new()
            where TDef : Registry<TRegistry, TDef, TBackend>.IRegistryDef
            where TBackend : struct, Registry<TRegistry, TDef, TBackend>.IRegistryBackend
        {
            Init<TRegistry, TDef, TBackend>(false);
        }
        public static void Init<TRegistry, TDef>(Boolean registerImmediately)
            where TRegistry : Registry<TRegistry, TDef>, new()
            where TDef : Registry<TRegistry, TDef>.IRegistryDef
        {
            Init<TRegistry, TDef, SimpleBackend<TRegistry, TDef>>(registerImmediately);
        }
        public static void Init<TRegistry, TDef>()
            where TRegistry : Registry<TRegistry, TDef>, new()
            where TDef : Registry<TRegistry, TDef>.IRegistryDef
        {
            Init<TRegistry, TDef>(false);
        }


    }
}