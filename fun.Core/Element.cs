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

        // Basic methods
        public virtual void Initialize() { }
        public virtual void Update(double time) { }
    }
}
