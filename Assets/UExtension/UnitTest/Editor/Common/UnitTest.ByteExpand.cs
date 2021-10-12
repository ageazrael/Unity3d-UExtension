using NUnit.Framework;

using UExtension;

namespace UnitTest.Common
{

    public class ByteExpandUnitTest
    {
        [Test]
        public void ToHEXString()
        {
            Assert.AreEqual(new byte[] { 0xFF, 0xF0, 0xF1 }.ToHEXString(), "FFF0F1");
        }
    }

}