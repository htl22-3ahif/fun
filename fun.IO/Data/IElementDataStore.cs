using fun.Core;
using System;
using System.Reflection;

namespace fun.IO.Data
{
    interface IElementDataStore : IFieldDataStore
    {
        Assembly[] Assemblys { get; }
        Element Element { get; }
        void PushElement(Type type);
        void DepushElement();
    }
}
