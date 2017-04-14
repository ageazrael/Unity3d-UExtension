using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UExtension;

namespace UExtension
{
	[System.Serializable]
	public class RefTypeNext
	{
		public RTexture2D Next;
	}
	
	[System.Serializable]
	public class RefType
	{
		public RTexture2D 	Texture;
		public RefTypeNext 	Next;
	}

	public class PrefabAsset : MonoBehaviour
	{
		public RTexture2D 		A;
		public RTexture2D[] 	Array;
		public List<RTexture2D> List;
		public RefType 			AClass;

		public RefType[]		ArrayClass;
		public List<RefType>	ListClass;
	}

}