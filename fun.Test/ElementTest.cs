using System;
using fun.Core;
using Environment = fun.Core.Environment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace fun.Test
{
    [TestClass]
    public class ElementTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            #region Testing if constructor is remiting the given parameters

            var env = new Environment();
            var entity = new Entity("test", env);

            var element = new Element(env, entity);

            Assert.IsTrue(element.Environment == env);
            Assert.IsTrue(element.Entity == entity);

            #endregion

            #region Testing for argument is null

            try
            {
                element = new Element(null, null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                element = new Element(env, null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }
            try
            {
                element = new Element(null, entity);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }

            #endregion
        }
    }
}
