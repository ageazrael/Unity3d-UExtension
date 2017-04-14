using NUnit.Framework;
using UExtension;

namespace UnitTest.Common
{

    public class PathExpandUnitTest
    {
        [Test]
        public void Combine()
        {
            Assert.AreEqual(PathExtension.Combine("Assert", "Child1", "Child2"), "Assert/Child1/Child2");
            Assert.AreEqual(PathExtension.Combine("Assert", "/Child1/", "\\Child2"), "Assert/Child1/Child2");

            Assert.AreEqual(PathExtension.Combine("Assert\\", "/Child1\\", "\\Child2/"), "Assert/Child1/Child2");
        }
    }

}