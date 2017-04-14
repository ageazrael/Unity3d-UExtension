using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace UExtension
{
    public class HashAlgorithmGenerate
    {
        public static string ByString(string rText, string rHashName, Encoding rEncoding)
        {
            var rHashAlgorithm  = HashAlgorithm.Create(rHashName);
            var rTextBytes      = rEncoding.GetBytes(rText);
            rHashAlgorithm.TransformFinalBlock(rTextBytes, 0, rTextBytes.Length);
            return rHashAlgorithm.Hash.ToHEXString();
        }
    }
}