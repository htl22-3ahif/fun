namespace fun.Core
{
    /// <summary>
    /// This class exists to ensure entitys uniqueness.
    /// </summary>
    public class Element
    {
        protected readonly Environment environment;
        protected readonly Entity entity;

        /// <summary>
        /// Creates an element-Object.
        /// </summary>
        /// <param name="environment">environment of our entity</param>
        /// <param name="entity">element of our element</param>
        public Element(Environment environment, Entity entity)
        {
            this.environment = environment;
            this.entity = entity;
        }

        // Event Methods
        /// <summary>
        /// Triggers when new entity joins the environment after inizialization
        /// </summary>
        /// <param name="entity">specifies the new entity</param>
        public virtual void OnEntityJoinedEnvironment(Entity entity) { }
        /// <summary>
        /// Triggers when new element joins the entity, where this element is contained after inizialization
        /// </summary>
        /// <param name="element">spedifies the new element</param>
        public virtual void OnElementJoinedEntity(Element element) { }
        /// <summary>
        /// Triggers when environment is closing
        /// </summary>
        public virtual void OnClose() { }

        // Basic methods
        public virtual void Initialize() { }
        public virtual void Update(double time) { }
    }
}
