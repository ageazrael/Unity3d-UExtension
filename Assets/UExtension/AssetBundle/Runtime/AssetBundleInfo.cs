using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace UExtension
{
	/// <summary>
	/// AB info entity.
	/// </summary>
    public class ABInfoEntity
    {
        public string                     Hash;
        public string[]                   Depend;
        public Dictionary<string, string> LoadNameToFullName = new Dictionary<string, string>();
		public List<string>				  SceneNames         = new List<string>();
    }
	/// <summary>
	/// AB info archive.
	/// </summary>
    public class ABInfoArchive : ReflectSerializerBinaryArchive<ABInfoArchive>
    {
        [SBIgnore]
        public const string BinaryFileName = "AssetBundleInfo.bin";

        public Dictionary<string, ABInfoEntity> Entitys = new Dictionary<string, ABInfoEntity>();

		public static List<string> GetDisaffinity(ABInfoArchive rBase, ABInfoArchive rNew)
		{
			var rResult = new List<string>();
			foreach (var rPair in rNew.Entitys)
			{
				if (!rBase.Entitys.ContainsKey (rPair.Key))
				{
					rResult.Add (rPair.Key);
					continue;
				}
				
				var rBaseEntity = rBase.Entitys [rPair.Key];
				var rNewEntity 	= rNew.Entitys [rPair.Key];

				if (rBaseEntity.Hash != rNewEntity.Hash)
					rResult.Add(rPair.Key);
			}
			return rResult;
		}
		public Dictionary<string, string> GenerateAssetToBundle()
		{
			var rResult = new Dictionary<string, string> ();
			foreach (var rEntityPair in this.Entitys)
			{
				foreach (var rPair in rEntityPair.Value.LoadNameToFullName)
					rResult.Add (rPair.Key, rEntityPair.Key);
			}
			return rResult;
		}
		public Dictionary<string, string> GenerateAssetToFullPath()
		{
			var rResult = new Dictionary<string, string> ();
			foreach (var rEntityPair in this.Entitys)
			{
				foreach (var rPair in rEntityPair.Value.LoadNameToFullName)
					rResult.Add (rPair.Key, rPair.Value);
			}
			return rResult;
		}
    }
}
