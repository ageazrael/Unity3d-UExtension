using UnityEngine;
using System.Collections;
using System.IO;
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

        public static string ByFile(string rHashName, params string[] rFilenames) => ByFile(rHashName, 1024, rFilenames);

        public static string ByFile(string rHashName, int nBufferSize, params string[] rFilenames)
        {
            var rHashAlgorithm  = HashAlgorithm.Create(rHashName);
            var rBuffer     = new byte[nBufferSize];
            var nFileCount  = rFilenames.Length;
            var nOffset     = 0;
            for (int nIndex = 0; nIndex < nFileCount; ++ nIndex)
            {
                using(var rReader = new BinaryReader(File.OpenRead(rFilenames[nIndex])))
                {
                    while (rReader.BaseStream.Position != rReader.BaseStream.Length)
                    {
                        var nReadSize = rReader.Read(rBuffer, nOffset, nBufferSize - nOffset);
                        if (nReadSize + nOffset == nBufferSize)
                        {
                            rHashAlgorithm.TransformBlock(rBuffer, 0, nBufferSize, rBuffer, 0);
                            nOffset = 0;
                        }
                        else
                        {
                            nOffset = nReadSize;
                        }
                    }
                }
            }

            rHashAlgorithm.TransformFinalBlock(rBuffer, nOffset, nBufferSize - nOffset);
            return rHashAlgorithm.Hash.ToHEXString();
        }
    }
}