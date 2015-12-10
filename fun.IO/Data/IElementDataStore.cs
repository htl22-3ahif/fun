using fun.Core;
using System;
using System.Reflection;

namespace fun.IO.Data
{
    interface IElementDataStore : IElementPropertyDataStore
    {
        void PushElement(Type type);
        void DepushElement();
        Assembly[] Assemblys { get; }
    }
}
