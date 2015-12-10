using fun.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
