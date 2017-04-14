using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using UExtension;

namespace UnitTest.Common
{
    public class ReflectExcuteUnitTest
    {
        [Test]
        public void Main()
        {
            var rCSC = new MonoCSC(MonoCSC.TargetType.Library, "test.dll");

            rCSC.AddCompileFile(Application.dataPath + "/UnitTest/test.cs");
            rCSC.Execute();
        }
    }

}