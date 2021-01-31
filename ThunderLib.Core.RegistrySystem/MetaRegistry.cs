namespace ThunderLib.Core.RegistrySystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class MetaRegistry : Registry<MetaRegistry, Registry>
    {
        internal static void CreateIfNeeded() => _instance ??= new();
        protected override Boolean acceptsProcedural => false;
        protected override Boolean autoRegisterTokens => false;

        protected override Boolean OnStandardTokensCreated()
        {
            // TODO: Should add some form of system here to detect what regs should be loaded.

            var tokens = FindRegTokens(static a => true, false);

            static void RegisterDeps(RegistryHandle? dep)
            {
                dep.regToken.Register();
                foreach(var d in dep.dependencies)
                {
                    RegisterDeps(d);
                }
            }
            foreach(var tok in tokens)
            {
                foreach(var dep in tok.def.dependencies)
                {
                    RegisterDeps(dep);
                }
            }


            return true;
        }
        public static void InitAll()
        {
            if(!instance.Init())
            {
                //TODO: Log fatal error
                throw new Exception("MetaRegistry.instance.Init() failed");
                return;
            }

            static Registry GetReg(Element a) => a?.regToken?.def!;
            var defs = MakeInitOrder(backend.EnumerateElements().Select(GetReg).Where(NotNull));

            foreach(var d in defs)
            {
                if(!d.Init())
                {
                    //TODO: Log error
                    throw new Exception($"{d.guid}.Init() failed");
                    continue;
                }
            }

            //Ensure OnStandardDefsCreated is called

            while(defs.Any(a => a.hasPendingProcedural))
            {
                foreach(var d in defs)
                {
                    //TODO: Need to log that procedural defs are being processed to identify loops
                    if(d.hasPendingProcedural && !d.ProcessProcedural())
                    {
                        //TODO: Log error
                        throw new Exception($"{d.guid}.ProcessProcedural() failed");
                        continue;
                    }
                }
            }

            foreach(var d in defs)
            {
                try
                {
                    d.OnInitFinished();
                } catch(Exception e)
                {
                    throw e;
                    //TODO: Log error
                }
            }
        }

        //TODO: Finish impl of hot reloading
        //public static void ReloadAll()
        //{
        //    if(stage == Stage.PreInit)
        //    {
        //        InitAll();
        //    } else if(stage == Stage.Finalized)
        //    {
        //        InternalReloadAll();
        //    } else
        //    {
        //        // TODO: Fatal Invalid state error
        //        return;
        //    }
        //}

        //private static void InternalReloadAll()
        //{
        //    static Registry GetReg(Element a) => a?.regToken?.def;
        //    static Boolean NotNull(Registry a) => a is not null;
        //    var defs = MakeInitOrder(backend.EnumerateElements().Select(GetReg).Where(NotNull));

        //    foreach(var d in defs)
        //    {
        //        //if(!d.Unload())
        //        //{
        //        //    //TODO: Log fatal error
        //        //    continue;
        //        //}
        //    }

        //    foreach(var d in defs)
        //    {
        //        //if(!d.Reload())
        //        //{
        //        //
        //        //}
        //    }
        //}

        //TODO: Write tests for this specific function
        private static IEnumerable<Registry> MakeInitOrder(IEnumerable<Registry> input)
        {
            var initList = new List<Registry>();
            var initSet = new HashSet<RegistryHandle>();

            foreach(var reg in input.OrderBy(Priority))
            {
                void AddToLoad(RegistryHandle handle, HashSet<RegistryHandle> curChain)
                {
                    if(initSet.Contains(handle)) return;
                    if(!curChain.Add(handle))
                    {
                        //TODO: Log circular dependency error
                        throw new Exception($"Circular dependency involving:\n {String.Join("\n", curChain.Select(h => h.target.guid))}");
                    }

                    foreach(var dep in handle.dependencies.OrderBy(Priority))
                    {
                        var ch = new HashSet<RegistryHandle>(curChain);
                        //TODO: Can pool these sets
                        AddToLoad(dep, ch);
                    }

                    if(initSet.Add(handle))
                    {
                        //TODO: Log priority being bypassed here
                        initList.Add(handle.target);
                    }
                }

                foreach(var dep in reg.dependencies.OrderBy(Priority))
                {
                    AddToLoad(dep, new HashSet<RegistryHandle>(new[] { reg._handle }));
                }

                if(initSet.Add(reg._handle))
                {
                    //TODO: Log priority being bypassed here
                    initList.Add(reg);
                }
            }
            return initList;
        }

        private static String Guid(Registry a) => a.guid;
        private static SByte Priority(Registry a) => a.priority;
        private static SByte Priority(RegistryHandle a) => a.priority;
        private static SByte Key(IGrouping<SByte, Registry> a) => a.Key;
        private static IEnumerable<Registry> WithDeps(IGrouping<SByte, Registry> a) => a.SelectMany(NotNullDependencies).Concat(a);
        private static IEnumerable<Registry> NotNullDependencies(Registry a) => a.dependencies.Select((Func<RegistryHandle, Registry?>)Registry).Where(NotNull)!;
        private static Registry? Registry(RegistryHandle a) => (a?.regToken?.def);
        private static Boolean NotNull(Registry? a) => a is not null;
    }
}