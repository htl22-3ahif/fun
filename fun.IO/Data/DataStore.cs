using fun.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using Environment = fun.Core.Environment;

namespace fun.IO.Data
{
    internal sealed class DataStore : IEnvironmentDataStore
    {
        private List<Assembly> assemblys;
        private List<Environment> environments;
        private Environment currEnvironment;
        private Entity currEntity;
        private Element currElement;

        public Environment[] Envionments { get { return environments.ToArray(); } }
        public Assembly[] Assemblys { get { return assemblys.ToArray(); } }
        public Element Element { get { return currElement; } }

        public DataStore()
        {
            assemblys = new List<Assembly>();
            environments = new List<Environment>();
        }

        public void PushEnvironment()
        {
            currEnvironment = new Environment();
        }

        public void AddPushedEnvirionment()
        {
            environments.Add(currEnvironment);
        }

        public void DepushEnvironment()
        {
            currEnvironment = null;
        }

        public void PushEntity(string name)
        {
            currEntity = new Entity(name, currEnvironment);
        }

        public void DepushEntity()
        {
            currEntity = null;
        }

        public void AddPushedEntity()
        {
            currEnvironment.AddEntity(currEntity);
        }

        public void AddEntity(string name)
        {
            var entity = new Entity(name, currEnvironment);
            currEnvironment.AddEntity(entity);
        }

        public void PushElement(Type type)
        {
            currEntity.AddElement(type);
            currElement = currEntity.GetElement(type);
        }

        public void DepushElement()
        {
            currElement = null;
        }

        public void AddLibary(Assembly assembly)
        {
            assemblys.Add(assembly);
        }
    }
}
