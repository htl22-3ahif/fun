using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fun.IO.Data
{
    interface IEnvironmentDataStore : IEntityDataStore, ILibaryDataStore
    {
        void PushEnvironment();
        void AddPushedEnvirionment();
        void DepushEnvironment();
    }
}
