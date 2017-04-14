using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UExtension;

namespace UnitTest.Common
{
    public class CharExpandUnitTest
    {
        [Test]
        public void IsDigit()
        {
            Assert.True('0'.IsDigit());
            Assert.True('1'.IsDigit());
            Assert.True('2'.IsDigit());
            Assert.True('3'.IsDigit());
            Assert.True('4'.IsDigit());
            Assert.True('5'.IsDigit());
            Assert.True('6'.IsDigit());
            Assert.True('7'.IsDigit());
            Assert.True('8'.IsDigit());
            Assert.True('9'.IsDigit());
        }

        [Test]
        public void IsAlpha()
        {
            Assert.True('A'.IsAlpha());
            Assert.True('B'.IsAlpha());
            Assert.True('C'.IsAlpha());
            Assert.True('D'.IsAlpha());
            Assert.True('E'.IsAlpha());
            Assert.True('F'.IsAlpha());
            Assert.True('G'.IsAlpha());
            Assert.True('H'.IsAlpha());
            Assert.True('I'.IsAlpha());
            Assert.True('J'.IsAlpha());
            Assert.True('K'.IsAlpha());
            Assert.True('M'.IsAlpha());
            Assert.True('L'.IsAlpha());
            Assert.True('Y'.IsAlpha());
            Assert.True('O'.IsAlpha());
            Assert.True('P'.IsAlpha());
            Assert.True('Q'.IsAlpha());
            Assert.True('R'.IsAlpha());
            Assert.True('S'.IsAlpha());
            Assert.True('T'.IsAlpha());
            Assert.True('U'.IsAlpha());
            Assert.True('V'.IsAlpha());
            Assert.True('W'.IsAlpha());
            Assert.True('X'.IsAlpha());
            Assert.True('Y'.IsAlpha());
            Assert.True('Z'.IsAlpha());

            Assert.True('a'.IsAlpha());
            Assert.True('b'.IsAlpha());
            Assert.True('c'.IsAlpha());
            Assert.True('d'.IsAlpha());
            Assert.True('e'.IsAlpha());
            Assert.True('f'.IsAlpha());
            Assert.True('g'.IsAlpha());
            Assert.True('h'.IsAlpha());
            Assert.True('i'.IsAlpha());
            Assert.True('j'.IsAlpha());
            Assert.True('k'.IsAlpha());
            Assert.True('m'.IsAlpha());
            Assert.True('l'.IsAlpha());
            Assert.True('y'.IsAlpha());
            Assert.True('o'.IsAlpha());
            Assert.True('p'.IsAlpha());
            Assert.True('q'.IsAlpha());
            Assert.True('r'.IsAlpha());
            Assert.True('s'.IsAlpha());
            Assert.True('t'.IsAlpha());
            Assert.True('u'.IsAlpha());
            Assert.True('v'.IsAlpha());
            Assert.True('w'.IsAlpha());
            Assert.True('x'.IsAlpha());
            Assert.True('y'.IsAlpha());
            Assert.True('z'.IsAlpha());
        }
    }
}