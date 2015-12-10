using System;

namespace fun.IO.Data
{
    interface IEntityDataStore : IElementDataStore
    {
        void PushEntity(string name);
        void DepushEntity();
        void AddPushedEntity();
        void AddEntity(string name);
    }
}
