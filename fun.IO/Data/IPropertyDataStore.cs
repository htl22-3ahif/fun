using fun.Core;
using System;

namespace fun.IO.Data
{
    internal interface IPropertyDataStore : IParamDataStore
    {
        Element Element { get; }
        object[] Params { get; }
        void FlushParams();
    }
}
