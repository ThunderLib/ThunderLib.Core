namespace ThunderLib.Core.RegistrySystem._Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

    using NUnit.Framework;

    using ThunderLib.Core.RegistrySystem;

    [TestFixture]
    public class RegistryTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            RegistryInitializer.Init<TestRegistry1, TestDef>(true);
            RegistryInitializer.Init<TestRegistry2, TestDef>(true);
            RegistryInitializer.Init<TestRegistry3, TestDef>(true);
            RegistryInitializer.Init<TestRegistry4, TestDef>(true);

            MetaRegistry.InitAll();
        }

        //TODO: Test does not cover priority sorting

        [Test]
        public void TestInit()
        {
            static IEnumerable<String> ForType<TReg>(IEnumerable<String> log)
            {
                var n = $"{typeof(TReg).FullName}.";
                return log
                    .Where(s => s.StartsWith(n))
                    .Select(s => s.Substring(n.Length));
            }
            

            var tr1 = typeof(TestRegistry1).FullName!;
            var tr2 = typeof(TestRegistry2).FullName!;
            var tr3 = typeof(TestRegistry3).FullName!;
            var tr4 = typeof(TestRegistry4).FullName!;

            var fti = "FirstTimeInit";
            var cad = "CreateAdditionalDefs";
            var ostc = "OnStandardTokensCreated";
            var oif = "OnInitFinished";


            String[] Order(params String[] names) => names
                .SelectMany(name => new[] { $"{name}.{fti}", $"{name}.{cad}", $"{name}.{ostc}" })
                .Concat(names.Select(s => $"{s}.{oif}"))
                .ToArray();

            foreach(var v in Events.log)
            {
                Console.WriteLine(v);
            }

            Console.WriteLine("Desired order:");
            var ord = Order(tr1, tr4, tr3, tr2);
            foreach(var v in ord)
            {
                Console.WriteLine(v);
            }

            Assert.That(Events.log.SequenceEqual(ord));
        }
    }




    public sealed class TestRegistry1 : Registry<TestRegistry1, TestDef>
    {
        protected override IEnumerable<RegistryHandle> dependencies => base.dependencies;
        protected override SByte priority => base.priority;

        protected override Boolean acceptsProcedural => Events.Add(base.acceptsProcedural);

        protected override Boolean autoRegisterTokens => Events.Add(base.autoRegisterTokens);

        protected override Boolean CreateAdditionalDefs() => Events.Add(base.CreateAdditionalDefs());
        protected override Boolean FirstTimeInit() => Events.Add(base.FirstTimeInit());
        protected override void OnInitFinished()
        {
            Events.Add();
            base.OnInitFinished();
        }
        protected override Boolean OnStandardTokensCreated() => Events.Add(base.OnStandardTokensCreated());
        protected override void OnTokenRegister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenRegister(token);
        }
        protected override void OnTokenUnregister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenUnregister(token);
        }
    }

    public class TestDef : TestRegistry1.IRegistryDef, TestRegistry2.IRegistryDef, TestRegistry3.IRegistryDef, TestRegistry4.IRegistryDef
    {
        public TestDef(string guid)
        {
            this.guid = guid;
        }

        public String guid { get; }

        TestRegistry1.RegistrationToken TestRegistry1.IRegistryDef.regToken { get; set; }
        TestRegistry2.RegistrationToken TestRegistry2.IRegistryDef.regToken { get; set; }
        TestRegistry3.RegistrationToken TestRegistry3.IRegistryDef.regToken { get; set; }
        TestRegistry4.RegistrationToken TestRegistry4.IRegistryDef.regToken { get; set; }
    }

    public sealed class TestRegistry2 : Registry<TestRegistry2, TestDef>
    {
        protected override IEnumerable<RegistryHandle> dependencies => base.dependencies.Append(TestRegistry1.handle).Append(TestRegistry3.handle);
        protected override SByte priority => base.priority;

        protected override Boolean acceptsProcedural => Events.Add(base.acceptsProcedural);

        protected override Boolean autoRegisterTokens => Events.Add(base.autoRegisterTokens);

        protected override Boolean CreateAdditionalDefs() => Events.Add(base.CreateAdditionalDefs());
        protected override Boolean FirstTimeInit() => Events.Add(base.FirstTimeInit());
        protected override void OnInitFinished()
        {
            Events.Add();
            base.OnInitFinished();
        }
        protected override Boolean OnStandardTokensCreated() => Events.Add(base.OnStandardTokensCreated());
        protected override void OnTokenRegister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenRegister(token);
        }
        protected override void OnTokenUnregister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenUnregister(token);
        }
    }
    public sealed class TestRegistry3 : Registry<TestRegistry3, TestDef>
    {
        protected override IEnumerable<RegistryHandle> dependencies => base.dependencies.Append(TestRegistry4.handle);
        protected override SByte priority => base.priority;

        protected override Boolean acceptsProcedural => Events.Add(base.acceptsProcedural);

        protected override Boolean autoRegisterTokens => Events.Add(base.autoRegisterTokens);

        protected override Boolean CreateAdditionalDefs() => Events.Add(base.CreateAdditionalDefs());
        protected override Boolean FirstTimeInit() => Events.Add(base.FirstTimeInit());
        protected override void OnInitFinished()
        {
            Events.Add();
            base.OnInitFinished();
        }
        protected override Boolean OnStandardTokensCreated() => Events.Add(base.OnStandardTokensCreated());
        protected override void OnTokenRegister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenRegister(token);
        }
        protected override void OnTokenUnregister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenUnregister(token);
        }
    }

    public sealed class TestRegistry4 : Registry<TestRegistry4, TestDef>
    {
        protected override IEnumerable<RegistryHandle> dependencies => base.dependencies;
        protected override SByte priority => base.priority;

        protected override Boolean acceptsProcedural => Events.Add(base.acceptsProcedural);

        protected override Boolean autoRegisterTokens => Events.Add(base.autoRegisterTokens);

        protected override Boolean CreateAdditionalDefs() => Events.Add(base.CreateAdditionalDefs());
        protected override Boolean FirstTimeInit() => Events.Add(base.FirstTimeInit());
        protected override void OnInitFinished()
        {
            Events.Add();
            base.OnInitFinished();
        }
        protected override Boolean OnStandardTokensCreated() => Events.Add(base.OnStandardTokensCreated());
        protected override void OnTokenRegister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenRegister(token);
        }
        protected override void OnTokenUnregister(RegistrationToken token)
        {
            Events.Add();
            base.OnTokenUnregister(token);
        }
    }


    public static class Events
    {
        public static List<String> log = new();

        public static void Add()
        {
            var tr = new StackTrace();
            var fr = tr.GetFrame(1);
            var method = fr.GetMethod();

            log.Add($"{method.DeclaringType.FullName}.{method.Name}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Add<T>(T input)
        {
            var tr = new StackTrace();
            var fr = tr.GetFrame(1);
            var method = fr.GetMethod();

            log.Add($"{method.DeclaringType.FullName}.{method.Name}");
            return input;
        }
    }
}