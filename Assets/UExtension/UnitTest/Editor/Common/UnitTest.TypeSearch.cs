using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UExtension;

namespace UnitTest.Common
{
    public class InterfaceExtension
    {
        public static explicit operator bool(InterfaceExtension rBase)
        {
            return true;
        }
    }

    public interface InterfaceBase
    {
        void Print();

        bool IsValid { get; }
    }

    public delegate InterfaceBase DelegateInterfaceCreate();

    public class InterfaceTypes : TypeSearchDefault<InterfaceBase>
    {
        public static void RegionFactor()
        {
        }
    }

    public class Impl1 : InterfaceBase
    {
        public void Print()
        {
            Debug.Log("Impl1.Print...");
        }

        public bool IsValid
        {
            get { return true; }
        }
    }

    public class Impl2 : InterfaceBase
    {
        public void Print()
        {
            Debug.Log("Impl2.Print...");
        }

        public bool IsValid
        {
            get { return false; }
        }
    }

    public class ReflectFactor : TSingleton<ReflectFactor>
    {

    }
    

    public class TypeSearchUnitTest
    {
        public void MainTest()
        {
            //var rObject = new Impl1();

            //if (rObject)
            //    Debug.Log("o~~~~");
        }
    }
}