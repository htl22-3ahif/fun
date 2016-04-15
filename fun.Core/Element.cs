using System;

namespace fun.Core
{
    /// <summary>
    /// This class exists to ensure entitys uniqueness.
    /// </summary>
    [Serializable]
    public class Element
    {
        public Environment Environment { get; private set; }
        public Entity Entity { get; private set; }

        /// <summary>
        /// Creates an element-Object.
        /// </summary>
        /// <param name="environment">environment of our entity</param>
        /// <param name="entity">element of our element</param>
        public Element(Environment environment, Entity entity)
        {
            if (environment == null)
                throw new ArgumentNullException("environment");

            if (entity == null)
                throw new ArgumentNullException("entity");

            this.Environment = environment;
            this.Entity = entity;
        }

        // Event Methods
        /// <summary>
        /// Triggers when new entity joins the environment after inizialization
        /// </summary>
        /// <param name="entity">specifies the new entity</param>
        public virtual void OnEntityAdded(Entity entity) { }
        /// <summary>
        /// Triggers when new element joins the entity, where this element is contained after inizialization
        /// </summary>
        /// <param name="element">spedifies the new element</param>
        public virtual void OnElementAdded(Element element) { }
        /// <summary>
        /// Triggers when environment is closing
        /// </summary>
        public virtual void OnClose() { }

        // Basic methods
        public virtual void Initialize() { }
        public virtual void Update(double time) { }
    }
}
