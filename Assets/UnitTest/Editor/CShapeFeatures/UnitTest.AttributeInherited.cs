using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using UExtension;

namespace UnitTest.CShapeFeatures
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class InheritedFalseAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class InheritedTrueAttribute : Attribute {}

    [InheritedFalse]
    public class InheritedFalseUBase { }

    public class InheritedFlaseUChild : InheritedFalseUBase { }

    [InheritedTrue]
    public class InheritedTrueUBase { }

    public class InheritedTrueUChild : InheritedTrueUBase { }

    public class AttributeInheritedUnitTest
    {
        [Test]
        public void Inherited()
        {
            Assert.IsTrue(typeof(InheritedFalseUBase).IsDefined(typeof(InheritedFalseAttribute), true));
            Assert.IsTrue(typeof(InheritedFlaseUChild).IsDefined(typeof(InheritedFalseAttribute), true));
            Assert.IsTrue(typeof(InheritedTrueUBase).IsDefined(typeof(InheritedTrueAttribute), true));
            Assert.IsTrue(typeof(InheritedTrueUChild).IsDefined(typeof(InheritedTrueAttribute), true));

            Assert.IsTrue(typeof(InheritedFalseUBase).IsApplyAttr(typeof(InheritedFalseAttribute), true));
            Assert.IsFalse(typeof(InheritedFlaseUChild).IsApplyAttr(typeof(InheritedFalseAttribute), true));
            Assert.IsTrue(typeof(InheritedTrueUBase).IsApplyAttr(typeof(InheritedTrueAttribute), true));
            Assert.IsTrue(typeof(InheritedTrueUChild).IsApplyAttr(typeof(InheritedTrueAttribute), true));
        }
    }
}