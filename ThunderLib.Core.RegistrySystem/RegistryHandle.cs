namespace ThunderLib.Core.RegistrySystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public abstract class RegistryHandle
    {
        private protected abstract Registry _target { get; }

        internal Boolean initCompleted => _target._stage == Stage.Finalized;
        internal MetaRegistry.RegistrationToken? regToken => _target.regToken;
        internal IEnumerable<RegistryHandle> dependencies => _target.dependencies;
        internal SByte priority => _target.priority;

        internal Registry target => _target;
    }
}