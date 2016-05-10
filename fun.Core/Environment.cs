using System;
using System.Collections.Generic;
using System.Linq;

namespace fun.Core
{
    /// <summary>
    /// Describes the environment of the game and manages communication between entities.
    /// </summary>
    [Serializable]
    public sealed class Environment
    {
        private readonly List<Entity> entities;

        private bool initialized;

        /// <summary>
        /// entities available for communication.
        /// </summary>
        public Entity[] Entities { get { return entities.ToArray(); } }

        /// <summary>
        /// Creates an environment-Object.
        /// </summary>
        public Environment()
        {
            entities = new List<Entity>();
            initialized = false;
        }

        /// <summary>
        /// Adds an entity to the environment.
        /// </summary>
        /// <param name="entity">our needed entity-Object</param>
        public void AddEntity(Entity entity)
        {
            // Make sure the same entity doesn't get added again
            foreach (var singleEntity in entities)
                if (singleEntity.Name == entity.Name)
                    // Throw expection if, for whatever reason, the same entity would be going to be added again
                    throw new ArgumentException("Entity " + entity.Name + " already exists.");

            // Add entity to entity-List
            entities.Add(entity);

            if (!initialized)
                return;

            entity.Initialize();

            foreach (var _entity in entities)
                if (_entity != entity)
                    foreach (var element in _entity.Elements)
                        element.OnEntityAdded(entity);
        }

        /// <summary>
        /// Removes an entity from the environment
        /// </summary>
        /// <param name="name">Name to specifie the entity</param>
        public void RemoveEntity(string name)
        {
            entities.Remove(entities.First(e => e.Name == name));
        }

        /// <summary>
        /// Returns an entity of the given type.
        /// </summary>
        /// <param name="name">name of the wanted entity</param>
        /// <returns>returns the wanted entity, if successful</returns>
        public Entity GetEntity(string name)
        {
            // Search
            foreach (var entity in entities)
                if (entity.Name == name)
                    return entity;

            // 404 not found
            throw new ArgumentException("Entity not found");
        }

        public void Close()
        {
            foreach (var entity in entities)
                entity.Close();
        }

        public void Initialize()
        {
            foreach (var entity in entities.Where(e => e.Enable))
                entity.Initialize();

            initialized = true;
        }

        public void Update(double time)
        {
            foreach (var entity in entities.Where(e => e.Enable))
                entity.Update(time);
        }
    }
}
