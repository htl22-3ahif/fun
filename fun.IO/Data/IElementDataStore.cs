﻿using fun.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fun.IO.Data
{
    interface IElementDataStore : IElementPropertyDataStore
    {
        void PushElement(Type type);
        void DepushElement();
        Assembly[] Assemblys { get; }
    }
}
