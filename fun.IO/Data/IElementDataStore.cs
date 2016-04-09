using fun.Core;
using System;
using System.Reflection;

namespace fun.IO.Data
{
    interface IElementDataStore : IPropertyDataStore, IFieldDataStore
    {
        void PushElement(Type type);
        void DepushElement();
    }
}
