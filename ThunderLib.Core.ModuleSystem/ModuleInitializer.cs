namespace ThunderLib.Core.ModuleSystem
{
    using System;
    public static class ModuleInitializer
    {
        public static void Init<TModule>(Boolean registerImmediately)
            where TModule : Module<TModule>, new()
        {
            ModuleRegistry.Add<TModule>(registerImmediately);
        }

    }
}