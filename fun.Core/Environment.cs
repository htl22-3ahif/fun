using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace fun.Core
{
    /// <summary>
    /// Describes the environment of the game and manages communication between entities.
    /// </summary>
    public sealed class Environment
    {
        private readonly List<Entity> entities;

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

        public void Initialize()
        {
            foreach (var entity in entities)
                entity.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
                entity.Update(gameTime);
        }
    }
}
