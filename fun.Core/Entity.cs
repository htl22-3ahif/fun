using System;
using System.Collections.Generic;
using System.Linq;

namespace fun.Core
{
    /// <summary>
    /// Describes an existing object in the environment.
    /// </summary>
    [Serializable]
    public sealed class Entity
    {
        private readonly List<Element> elements;
        private readonly List<Entity> children;

        /// <summary>
        /// This is the entitys name.
        /// </summary>
        public string Name { get; private set; }

        public Environment Environment { get; private set; }

        public Element[] Elements { get { return elements.ToArray(); } }

        public bool Enable { get; set; }

        /// <summary>
        /// Parent-Property
        /// for the parenting system.
        /// </summary>
        public Entity Parent { get; private set; }

        /// <summary>
        /// Child-Collection-Property
        /// for the parenting system
        /// </summary>
        public Entity[] Children { get { return children.ToArray(); } }

        /// <summary>
        /// Creates an entity-Object.
        /// </summary>
        /// <param name="name">name of our entity</param>
        /// <param name="environment">environment of our entity</param>
        public Entity(string name, Environment environment)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (environment == null)
                throw new ArgumentNullException("environment");

            this.Name = name;
            this.Environment = environment;
            this.elements = new List<Element>();
            this.children = new List<Entity>();
            this.Enable = true;
        }

        /// <summary>
        /// Adds an element of the given type to the entity.
        /// </summary>
        /// <param name="elementType">type of element</param>
        public void AddElement(Type elementType)
        {
            if (!elementType.IsSubclassOf(typeof(Element)))
                throw new ArgumentException(string.Format("You can not add an element of type \"{0}\"", elementType.Name));

            // check if the given type is declared
            foreach (var individualElement in elements)
                if (individualElement.GetType() == elementType)
                    throw new ArgumentException("");

            // Create an instance of the given type with its params
            var element = Activator.CreateInstance(elementType, Environment, this);

            // check for validity (if element actually inherits from the Element-Class)
            if (element is Element)
                // append to list should it be valid
                elements.Add(element as Element);
            else
                // throw exception if it turns out to be invalid
                throw new ArgumentException(element.GetType().Name + " does not inherit from Element");

            foreach (var _element in elements)
                _element.OnElementAdded(element as Element);
        }

        /// <summary>
        /// Removes an element of the given type from the entity.
        /// </summary>
        /// <typeparam name="T">type of element</typeparam>
        public void AddElement<T>() where T : Element
        {
            AddElement(typeof(T));
        }

        public void RemoveElement(Type elementType)
        {
            if (!elementType.IsSubclassOf(typeof(Element)))
                throw new ArgumentException(string.Format("You can not remove an element of type \"{0}\"", elementType.Name));

            elements.Remove(elements.First(e => e.GetType() == elementType));
        }

        /// <summary>
        /// Returns an element of the given type.
        /// </summary>
        /// <param name="elementType">the wanted element type is needed to distinguish between our elements</param>
        /// <returns>returns the searched element, if successful</returns>
        public Element GetElement(Type elementType)
        {
            // Search
            foreach (var element in elements)
                if (element.GetType() == elementType)
                    return element;

            // 404 not found
            throw new ArgumentException("Entity does not contain " + elementType.Name);
        }
        
        /// <summary>
        /// Returns an element of the given type.
        /// </summary>
        /// <typeparam name="T">the wanted element type is needed to distinguish between our elements</typeparam>
        /// <returns>returns the searched element, if successful</returns>
        public T GetElement<T>() where T : Element
        {
            return (T)GetElement(typeof(T));
        }

        /// <summary>
        /// Returns true if the entity contains the given element type.
        /// </summary>
        /// <param name="elementType">type of element</param>
        /// <returns></returns>
        public bool ContainsElement(Type elementType)
        {
            foreach (var element in elements)
                if (element.GetType() == elementType)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true if the entity contains the given element type.
        /// </summary>
        /// <param name="elementType">type of element</param>
        /// <returns></returns>
        public bool ContainsElement<T>()
            where T : Element
        {
            return ContainsElement(typeof(T));
        }

        /// <summary>
        /// Adds a parent due to our new parenting system policy.
        /// </summary>
        /// <param name="parent">a parent object</param>
        public void AddParent(Entity parent)
        {
            // Make sure the same parent doesn't get added again
            if (this.Parent == parent)
                // Throw expection if, for whatever reason, the same parent would be going to be added again
                throw new ArgumentException("Parent " + parent.Name + " already exists");

            // Set parent-Property
            this.Parent = parent;
            // Ultimately let the parent acknowledge its children
            parent.children.Add(this);
        }

        /// <summary>
        /// Adds children due to our new parenting system policy.
        /// </summary>
        /// <param name="children"></param>
        public void AddChildren(params Entity[] children)
        {
            // Make sure the same child doesn't get added again
            foreach (var child in children)
                if (this.children.Contains(child))
                    // Throw expection if, for whatever reason, the same child would be going to be added again 
                    throw new ArgumentException("Child " + child.Name + " already exists");

            // Set children
            this.children.AddRange(children);

            // Ultimately let everyone of the children acknowledge its parent
            foreach (var child in children)
                child.Parent = this;
        }

        public void Close()
        {
            foreach (var element in elements)
                element.OnClose();
        }

        // Basic methods
        public void Initialize()
        {
            foreach (var element in elements)
                element.Initialize();
        }
        public void Update(double time)
        {
            foreach (var element in elements)
                element.Update(time);
        }
    }
}
