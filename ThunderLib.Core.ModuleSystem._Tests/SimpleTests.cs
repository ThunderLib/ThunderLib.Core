namespace ThunderLib.Core.ModuleSystem._Tests
{
    using System;

    using NUnit.Framework;

    public class InitProcessTesting
    {
        [SetUp]
        public void Setup()
        {
            ModuleInitializer.Init<TestModule1>(true);
        }

        [Test]
        public void Main()
        {
            Assert.That(TestModule1.loaded == false);
            RegistrySystem.MetaRegistry.InitAll();
            Assert.That(TestModule1.loaded == true);
        }
    }



    public sealed class TestModule1 : Module<TestModule1>
    {
        //TODO: Need to test that these are called correctly
        protected override void Disable() { }
        protected override void Enable() { }
    }
}