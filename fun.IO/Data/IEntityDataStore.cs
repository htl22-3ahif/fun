using fun.Core;
using System;

namespace fun.IO.Data
{
    interface IEntityDataStore : IElementDataStore
    {
        Entity Entity { get; }
        void PushEntity(string name);
        void DepushEntity();
        void AddPushedEntity();
        void AddEntity(string name);
    }
}
