using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension.Voxel
{
    [Serializable]
    [BlockRuntimeImpl(typeof(BlockRTSingle))]
    public class BlockSingleData : BlockData
    {
        public Sprite All;
    }
    [Serializable]
    public class BlockRTSingle : BlockRT
    {
        public override void GeneratorMesh(BlockGeneratorMesh rGeneratorMesh, BlockData rBaseData)
        {
            var rData = rBaseData as BlockSingleData;

            if (rGeneratorMesh.IsTopEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Top, rData.All);
            }
            if (rGeneratorMesh.IsBottomEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Bottom, rData.All);
            }
            if (rGeneratorMesh.IsFrontEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Front, rData.All);
            }
            if (rGeneratorMesh.IsBackEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Back, rData.All);
            }
            if (rGeneratorMesh.IsLeftEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Left, rData.All);
            }
            if (rGeneratorMesh.IsRightEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Right, rData.All);
            }
        }
    }
}
