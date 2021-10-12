using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension.Voxel
{
    [Serializable]
    [BlockRuntimeImpl(typeof(BlockRTPrefab))]
    public class BlockPrefabData : BlockData
    {
        public GameObject Prefab;
    }
    [Serializable]
    public class BlockRTPrefab: BlockRT
    {

    }
}
