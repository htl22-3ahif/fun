using System;
using System.Reflection;

namespace fun.IO.Data
{
    interface ILibaryDataStore
    {
        void AddLibary(Assembly assembly);
    }
}
