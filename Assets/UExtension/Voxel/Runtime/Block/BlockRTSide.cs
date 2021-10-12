using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UExtension.Voxel
{
    [Serializable]
    [BlockRuntimeImpl(typeof(BlockRTSide))]
    public class BlockSideData : BlockData
    {
        public Sprite TopBottom;
        public Sprite Side;
    }
    [Serializable]
    public class BlockRTSide : BlockRT
    {
        public override void GeneratorMesh(BlockGeneratorMesh rGeneratorMesh, BlockData rBaseData)
        {
            var rData = rBaseData as BlockSideData;
            if (rGeneratorMesh.IsTopEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Top, rData.TopBottom);
            }
            if (rGeneratorMesh.IsBottomEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Bottom, rData.TopBottom);
            }
            if (rGeneratorMesh.IsFrontEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Front, rData.Side);
            }
            if (rGeneratorMesh.IsBackEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Back, rData.Side);
            }
            if (rGeneratorMesh.IsLeftEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Left, rData.Side);
            }
            if (rGeneratorMesh.IsRightEmpty)
            {
                rGeneratorMesh.AddFace(EBlockFace.Right, rData.Side);
            }
        }
    }
}
