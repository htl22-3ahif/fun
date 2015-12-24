using System;
using Environment = fun.Core.Environment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using fun.Core;

namespace fun.Test
{
    [TestClass]
    public class EntityTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            var env = new Environment();

            var entity = new Entity("test", env);

            Assert.IsTrue(entity.Name == "test");
            Assert.IsTrue(entity.Environment == env);

            try
            {
                entity = new Entity(null, null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                entity = new Entity("test", null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                entity = new Entity(string.Empty, null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                entity = new Entity(null, env);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
        }

        [TestMethod]
        public void AddEntityTest()
        {
            var env = new Environment();
            var entity = new Entity("test", env);

            try
            {
                entity.AddElement(typeof(Element));
                Assert.Fail();
            } catch (ArgumentException) { }
            try
            {
                entity.AddElement<Element>();
                Assert.Fail();
            }
            catch (ArgumentException) { }

            entity.AddElement(typeof(TestElement1));
            Assert.IsTrue(entity.Elements[0].GetType() == typeof(TestElement1));
            try
            {
                entity.AddElement(typeof(TestElement1));
                Assert.Fail();
            }
            catch (ArgumentException) { }

            entity = new Entity("test", env);

            entity.AddElement<TestElement1>();
            Assert.IsTrue(entity.Elements[0].GetType() == typeof(TestElement1));
            try
            {
                entity.AddElement<TestElement1>();
                Assert.Fail();
            }
            catch (ArgumentException) { }
        }
    }
    #region Test Classes and Structs

    class TestElement1 : Element
    {
        public TestElement1(Environment environment, Entity entity) : base(environment, entity)
        {
        }
    }

    #endregion
}
