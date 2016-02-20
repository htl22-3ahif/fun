using fun.Core;
using System;
using System.Reflection;

namespace fun.IO.Data
{
    interface IElementDataStore : IPropertyDataStore
    {
        void PushElement(Type type);
        void DepushElement();
    }
}
