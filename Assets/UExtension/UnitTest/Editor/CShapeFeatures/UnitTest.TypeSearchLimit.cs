using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UnitTest.CShapeFeatures
{

    public class TypeSearchLimitUnitTest
    {
        public class ReflectSearchBase {
            public void f() { }
        }
        public class TemplateChild<T> : ReflectSearchBase { }
        public class TemplateInstance : TemplateChild<float> { }

        public class MianUse
        {
            public void Main()
            {
                var v = new TemplateChild<int>();
                v.f();
            }
        }

        public class RelfectSearchTypes : UExtension.TypeSearchDefault<ReflectSearchBase> { }

        [Test]
        public void Main()
        {
            // 无法搜索到TemplateChild<int>但却能搜索到TemplateInstance
            // ::模板实例无法被当成是一个类型被搜索到
            foreach(var rTypeFullName in RelfectSearchTypes.TypeFullNames)
                Debug.Log(rTypeFullName);
        }
    }

}