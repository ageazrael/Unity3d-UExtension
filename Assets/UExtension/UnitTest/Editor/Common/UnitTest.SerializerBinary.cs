using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

using UExtension;

namespace UnitTest.Common
{
    public class SerializerBinaryUnitTest
    {
        [Test]
        public void TypeCheck()
        {
            var rStream = new MemoryStream(100);
            var rWriter = new BinaryWriter(rStream);
            var rReader = new BinaryReader(rStream);

            

            rWriter.Serialize(100);
            rWriter.Serialize(string.Empty);
            rWriter.Serialize("FFF");
            rWriter.Serialize(100.0f);

            rStream.Position = 0;

            Assert.AreEqual(rReader.Deserialize(int.MinValue), 100);
            Assert.AreEqual(rReader.Deserialize(string.Empty), string.Empty);
            Assert.AreEqual(rReader.Deserialize(string.Empty), "FFF");
            Assert.AreEqual(rReader.Deserialize(float.MinValue), 100.0f);

            rStream.Position = 0;

            var rData = new SBT1();
            (rData.DynamicValue1 as SBDynamicChild).NextText = "1111";
            (rData.DynamicValue2[0] as SBDynamicChild).NextText = "1111";
            (rData.DynamicValue3[0] as SBDynamicChild).NextText = "1111";
            rWriter.Serialize(rData);

            rStream.Position = 0;
            var rNewData = rReader.Deserialize(default(SBT1));

            Assert.AreEqual((rNewData.DynamicValue1 as SBDynamicChild).NextText,       "1111");
            Assert.AreEqual((rNewData.DynamicValue2[0] as SBDynamicChild).NextText,    "1111");
            Assert.AreEqual((rNewData.DynamicValue3[0] as SBDynamicChild).NextText,    "1111");
        }
    }

}