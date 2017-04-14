using System.Collections.Generic;

using UExtension;

namespace UnitTest
{

    public enum SBEnum
    {
        Type1,
    }

    [SBGroupInerited("UnitTest")]
    public partial class SBDynamicBase : SerializerBinary
    {
        public string Text;
    }

    public partial class SBDynamicChild : SBDynamicBase
    {
        public string NextText;
    }
    [SBGroup("UnitTest")]
    public partial class SBUnitTest : SerializerBinary
    {
        public SBUnitTest[]     SBValue;
    }

    [SBGroup("UnitTest")]
    public partial class SBT1 : SerializerBinary
    {
        public string   V1;
        public string[] V2;
        public List<string> V3;
        public Dictionary<string, string> V4;
        public Dictionary<string, string[]> V5;
        public Dictionary<string, Dictionary<string, string[][]>> V6;

        public SBEnum E1;
        public SBEnum[] E2;
        public List<SBEnum> E3;
        public Dictionary<string, SBEnum> E4;
        public Dictionary<SBEnum, string> E5;

        [SBDynamic] public SBDynamicBase                        DynamicValue1 = new SBDynamicChild();
        [SBDynamic] public SBDynamicBase[]                      DynamicValue2 = { new SBDynamicChild() };
        [SBDynamic] public List<SBDynamicBase>                  DynamicValue3 = new List<SBDynamicBase>(new SBDynamicChild[] { new SBDynamicChild()});
        [SBDynamic] public Dictionary<string, SBDynamicBase>    DynamicValue4;
        [SBDynamic] public Dictionary<SBDynamicBase, string>    DynamicValue5;
        [SBDynamic] public Dictionary<string, string>           DynamicValue6;
        [SBDynamic] public List<string>                         DynamicValue7;

        [SBDynamic] public int                                  DynamicValueDummy;

        public SBUnitTest[] SBValue;
        public SBT1[] SB1Value;
    }

    public partial class SBT2 : SBT1
    {
        public string ChildV1;
    }

}