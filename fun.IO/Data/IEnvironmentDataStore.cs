using System;

namespace fun.IO.Data
{
    interface IEnvironmentDataStore : IEntityDataStore, ILibaryDataStore
    {
        void PushEnvironment();
        void AddPushedEnvirionment();
        void DepushEnvironment();
    }
}
