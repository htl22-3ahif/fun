using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fun.IO.Data
{
    internal interface IParamDataStore
    {
        void PushParam(object param);
        Assembly[] Assemblys { get; }
    }
}
