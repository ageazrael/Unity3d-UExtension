using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UExtension
{
    public static class MD5Generate
    {
        public static string ByString(string rText, Encoding rEncoding)
        {
            return HashAlgorithmGenerate.ByString(rText, "MD5", rEncoding);
        }
        public static string ByString(string rText)
        {
            return ByString(rText, Encoding.Default);
        }
    }
}